using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Documents.DocumentElement.Interface;
using Application.Documents.Documents.Interfaces;
using Application.Documents.DocumentStructure.Interfaces;
using Application.Word.Dto;
using Application.Word.Interfaces;
using Application.Word.RenderParams;
using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.Exceptions;
using Domain.DocumentStructureModels;
using Domain.Logs;
using Domain.Logs.Configuration;
using MongoDB.Bson;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;

namespace Application.Word
{
    public class WordService : IWordService
    {
        #region Static fields 
        private static readonly string databaseConfigFile = "./configs/mongo_database_config.json";
        #endregion

        #region Database
        private readonly IDocumentService documentService;
        private readonly IDocumentElementService documentElementService;
        private readonly IDocumentStructureService documentStructureService;
        #endregion

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        #endregion

        private readonly LogConfiguration logConfiguration;
        
        public WordService(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IDocumentService>().TryGetTarget(out documentService);
            factoriesManager.GetService<IDocumentElementService>().TryGetTarget(out documentElementService);
            factoriesManager.GetService<IDocumentStructureService>().TryGetTarget(out documentStructureService);
            
            logConfiguration = new LogConfiguration(this.GetType());
        }
        
        public async Task<byte[]> Render(ObjectId documentID, RenderSettings renderSettings)
        {
            try
            {
                var document = await documentService.GetDocumentByID(documentID).ConfigureAwait(false);
                var documentStructure = await documentStructureService.GetDocumentStructureAsync(documentID)
                    .ConfigureAwait(false);

                if (document is null || documentStructure is null)
                {
                    throw new ArgumentException("Not all information collected for this document ID");
                }
                
                DocumentElementRenderDto dto = new DocumentElementRenderDto(documentStructure.Items[0]);
                GetRenderElements(documentStructure.Items[0], dto);

                return await new WordRenderer(renderSettings, document, dto.InnerElements).Render()
                    .ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occurred while rendering WORD document");
            }
        }

        private void GetRenderElements(Item item, DocumentElementRenderDto parentElement)
        {
            if (item.ItemStatus.ItemType == ItemType.Content)
            {
                var documentElements = item.ElementsIds.Select(id =>
                    documentElementService.GetCurrentDocumentElement(id).GetAwaiter().GetResult());
                parentElement.Elements.AddRange(documentElements);
            }
            else if (item.ItemStatus.ItemType == ItemType.Item)
            {
                parentElement.InnerElements.AddRange(item.Items.Select(it => new DocumentElementRenderDto(it)));

                for (int i = 0; i < item.Items.Count; ++i)
                {
                    GetRenderElements(item.Items[i], parentElement.InnerElements[i]);
                }
            }
        }
    }
}