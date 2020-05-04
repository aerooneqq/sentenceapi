using System;
using System.Linq;
using System.Threading.Tasks;

using Application.Documents.DocumentElement.Interface;
using Application.Documents.DocumentElement.Models;
using Application.Documents.DocumentStructure.Interfaces;
using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using DataAccessLayer.Filters.Base;

using Domain.Date;
using Domain.DocumentElements;
using Domain.DocumentElements.Image;
using Domain.DocumentElements.NumberedList;
using Domain.DocumentElements.Paragraph;
using Domain.DocumentElements.Table;
using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.VersionControl;

using MongoDB.Bson;
using Newtonsoft.Json;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;

namespace Application.Documents.DocumentElement
{
    public class BranchNodeService : IBranchNodeService
    {
        private const string databaseConfigFile = "./configs/mongo_database_config.json";

        private readonly IDatabaseService<DocumentElementWrapper> database;

        private readonly IDocumentStructureService documentStructureService;

        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IDateService dateService;

        private readonly LogConfiguration logConfiguration;


        public BranchNodeService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager) 
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IDateService>().TryGetTarget(out dateService);
            factoriesManager.GetService<IDocumentStructureService>().TryGetTarget(out documentStructureService);

            databaseManager.MongoDBFactory.GetDatabase<DocumentElementWrapper>().TryGetTarget(out database);
            
            IConfigurationBuilder configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            logConfiguration = new LogConfiguration(GetType());
        }

        public async Task<DocumentElementDto> CreateNewNodeAsync(ObjectId documentElementID, ObjectId branchID, 
                                                                 ObjectId userID, string nodeName, string comment) 
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                FilterBase getFilter = new EqualityFilter<ObjectId>("_id", documentElementID);
                var element = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
                if (element is null)
                {
                    throw new ArgumentException("No document element was found for id");
                }

                var branch = element.Branches.Find(b => b.BranchID == branchID);
                if (branch is null)
                {
                    throw new ArgumentException("No branch found for this id");
                }

                branch.BranchNodes.Add(BranchNode.GetEmptyNode(element.Type, dateService, userID, nodeName, comment));

                await database.Update(element).ConfigureAwait(false);

                var dto = new DocumentElementDto(element);
                dto.SetBranches(element.Branches, userID);

                return dto;
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occurred while creating new node");
            }
        }

        public async Task<DocumentElementDto> UpdateNodePropertiesAsync(ObjectId documentElementID, ObjectId branchNodeID, 
                                                                        ObjectId userID, string newName, string newComment)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                FilterBase getFilter = new EqualityFilter<ObjectId>("_id", documentElementID);
                var element = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
                if (element is null)
                {
                    throw new ArgumentException("No document element was found for id");
                }

                BranchNode node = element.FindBranchNode(branchNodeID);
                if (node is null)
                {
                    throw new ArgumentException("No node was found for this id");
                }

                node.Title = newName;
                node.Comment = newComment;

                await database.Update(element).ConfigureAwait(false);

                var dto = new DocumentElementDto(element);
                dto.SetBranches(element.Branches, userID);

                return dto;
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occurred while updating node");
            }
        }

        public async Task<DocumentElementDto> DeleteNodeAsync(ObjectId documentElementID, ObjectId branchNodeID, 
                                                              ObjectId userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                FilterBase getFilter = new EqualityFilter<ObjectId>("_id", documentElementID);
                var element = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
                if (element is null)
                {
                    throw new ArgumentException("No document element was found for id");
                }

                element.DeleteNode(branchNodeID);
                
                await database.Update(element).ConfigureAwait(false);

                var dto = new DocumentElementDto(element);
                dto.SetBranches(element.Branches, userID);

                return dto;
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occurred while deleting node");
            }
        }

        public async Task<DocumentElementDto> UpdateContentAsync(DocumentElementContentUpdateDto dto)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                FilterBase getFilter = new EqualityFilter<ObjectId>("_id", dto.DocumentElementID);
                var element = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
                if (element is null)
                {
                    throw new ArgumentException("No document element was found for id");
                }
                
                BranchNode node = element.FindBranchNode(dto.BranchNodeID);

                Console.WriteLine(dto.NewContent);
#warning Replace with IDeserailizer 
                node.DocumentElement = element.Type switch 
                {
                    DocumentElementType.Table => JsonConvert.DeserializeObject<Table>(dto.NewContent),
                    DocumentElementType.Image => JsonConvert.DeserializeObject<Image>(dto.NewContent),
                    DocumentElementType.Paragraph => JsonConvert.DeserializeObject<Paragraph>(dto.NewContent),
                    DocumentElementType.NumberedList => JsonConvert.DeserializeObject<NumberedList>(dto.NewContent),
                    _ => null,
                };

                await database.Update(element).ConfigureAwait(false);

                var elementDto = new DocumentElementDto(element);
                elementDto.SetBranches(element.Branches, dto.UserID);

                return elementDto;
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occurred while updating node;s content");
            } 
        } 
    }
}