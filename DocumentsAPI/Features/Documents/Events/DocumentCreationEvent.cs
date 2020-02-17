using System;
using System.Threading.Tasks;

using Application.Documents.DocumentStructure.Interfaces;

using Domain.Logs;
using Domain.Logs.Configuration;

using MongoDB.Bson;

using SharedLibrary.Events.Exceptions;
using SharedLibrary.Events.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;


namespace DocumentsAPI.Features.Documents.Events
{
    /// <summary>
    /// Creates structure for the document, sets initial roles
    /// </summary>
    public class DocumentCreationEvent : IDomainEvent
    {
        #region Services
        private readonly IDocumentStructureService documentStructureService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        #endregion

        private readonly LogConfiguration logConfiguration;
        
        private readonly ObjectId documentID;
        private readonly string documentName;
        private readonly ObjectId userID;


        public DocumentCreationEvent(ObjectId documentID, string documentName, ObjectId userID,
                                     IFactoriesManager factoriesManager)
        {
            this.documentID = documentID;
            this.userID = userID;
            this.documentName = documentName;

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IDocumentStructureService>().TryGetTarget(out documentStructureService);
            
            logConfiguration = new LogConfiguration(GetType());
        }
        
        
        public async Task Handle()
        {
            try
            {
                await documentStructureService.CreateNewDocumentStructure(documentID, documentName, userID);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DomainEventException("The error occured while creating the document");
            }
        }
    }
}