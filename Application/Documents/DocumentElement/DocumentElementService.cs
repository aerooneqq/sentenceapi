using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Application.Documents.DocumentElement.Interface;
using Application.Documents.DocumentElement.Models;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using DataAccessLayer.Filters.Base;
using Domain.Date;
using Domain.DocumentElements;
using Domain.DocumentElements.Dto;
using Domain.DocumentElements.Image;
using Domain.DocumentElements.NumberedList;
using Domain.DocumentElements.Paragraph;
using Domain.DocumentElements.Table;
using Domain.Extensions;
using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.VersionControl;

using MongoDB.Bson;

using Newtonsoft.Json;

using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;

namespace Application.Documents.DocumentElement
{
    public class DocumentElementService : IDocumentElementService
    {
        private const string databaseConfigFile = "./configs/mongo_database_config.json";

        private readonly IDatabaseService<DocumentElementWrapper> database;

        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IDateService dateService;

        private readonly LogConfiguration logConfiguration;


        public DocumentElementService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager) 
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IDateService>().TryGetTarget(out dateService);

            databaseManager.MongoDBFactory.GetDatabase<DocumentElementWrapper>().TryGetTarget(out database);
            
            IConfigurationBuilder configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            database.CreateCollection();

            logConfiguration = new LogConfiguration(GetType());
        }

        public async Task<IEnumerable<DocumentElementDto>> GetDocumentElementsAsync(ObjectId parentItemID, ObjectId userID)
        {
            try
            {
                var elementWrappers = await database.Get(new EqualityFilter<ObjectId>("parentItemID", parentItemID))
                    .ConfigureAwait(false);

                if (elementWrappers is null)
                {
                    return null;
                }

                return elementWrappers.Select(w => 
                {
                    DocumentElementDto dto = new DocumentElementDto(w);
                    dto.SetBranches(w.Branches, userID);

                    return dto;
                });
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The erorr occured while getting the document");
            }
        }

        public async Task RenameDocumentElementAsync(DocumentElementRenameDto renameDto)
        {
            try
            {
                if (string.IsNullOrEmpty(renameDto.NewName) || string.IsNullOrWhiteSpace(renameDto.NewName))
                    throw new ArgumentException("New name length must be greater than 0");
                
                var getFilter = GetGetDocumenElWrapperByIdFilter(renameDto.DocumentElementID);

                DocumentElementWrapper document = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
                if (document is null)
                    throw new ArgumentException("There is no document with such an ID");

                Branch currentBranch = document.Branches.FirstOrDefault(b => b.BranchID == renameDto.BranchID);
                if (currentBranch is null)
                    throw new ArgumentException("Branch with such an id does not exist");

                BranchNode currBranchNode = currentBranch.BranchNodes.FirstOrDefault(bn => 
                    bn.BranchNodeID == currentBranch.CurrentBranchNodeID);

                currBranchNode.DocumentElement.Name = renameDto.NewName;

                await database.Update(document).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while updating the document element");
            }
        }

        private FilterBase GetGetDocumenElWrapperByIdFilter(ObjectId id) =>
            new EqualityFilter<ObjectId>(typeof(DocumentElementWrapper).GetBsonPropertyName("ID"), id);

        public async Task UpdateContentInBranchNodeAsync(DocumentElementContentUpdateDto updateDto) 
        {
            try
            {
                var getFilter = GetGetDocumenElWrapperByIdFilter(updateDto.DocumentElementID);
                
                DocumentElementWrapper documentElement = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();

                if (documentElement is null)
                    throw new ArgumentException("There is no document element with such an ID");

                Branch currBranch = documentElement.Branches.FirstOrDefault(b => b.BranchID == updateDto.BranchID);
                #warning add checks to access
                if (currBranch is null)
                    throw new ArgumentException("Branch with such an id does not exist");

                BranchNode currBranchNode = currBranch.BranchNodes.FirstOrDefault(bn => bn.BranchNodeID == updateDto.BranchNodeID);

                if (currBranchNode is null)
                    throw new ArgumentException("Branch node with such an id does not exist");

                currBranchNode.DocumentElement = DeserializeDocumentElement(updateDto.NewContent, documentElement.Type);

                await database.Update(documentElement).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while updating the content of the branch node");
            }
        }

        private Domain.DocumentElements.DocumentElement DeserializeDocumentElement(string content, DocumentElementType type) =>
            type switch 
            {
                DocumentElementType.Image => JsonConvert.DeserializeObject<Image>(content),
                DocumentElementType.Paragraph => JsonConvert.DeserializeObject<Paragraph>(content),
                DocumentElementType.NumberedList => JsonConvert.DeserializeObject<NumberedList>(content),
                DocumentElementType.Table => JsonConvert.DeserializeObject<Table>(content),
                _ => null
            };

        public async Task DeleteDocumentElementAsync(DocumentElementDeleteDto deleteDto) 
        {
            try
            {
                var getFilter = GetGetDocumenElWrapperByIdFilter(deleteDto.DocumentElementID);
                
                DocumentElementWrapper documentElement = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();

                if (documentElement is null)
                    throw new ArgumentException("Document element with such an id does not exist");

                documentElement.IsDeleted = true;
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while deleting the element");
            }
        }

        public async Task<ObjectId> CreateNewDocumentElementAsync(DocumentElementCreateDto createDto) 
        {
            try
            {
                var documentElementWrapper = GetEmptyDocumentWrapper(createDto);
                await database.Insert(documentElementWrapper).ConfigureAwait(false);

                return documentElementWrapper.ID;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while creationg new element");
            }
        }

        private DocumentElementWrapper GetEmptyDocumentWrapper(DocumentElementCreateDto createDto)
        {
            DateTime creationDate = dateService.Now;
            ObjectId newDocElementID = ObjectId.GenerateNewId();
            
            DocumentElementWrapper documentElement = new DocumentElementWrapper()
            {
                CreatorID = createDto.UserID,
                ID = newDocElementID,
                ParentDocumentID = createDto.ParentDocumentID,
                ParentItemID = createDto.ParentItemID,
                Type = createDto.Type,
                Branches = new List<Branch>() 
                {
                    new Branch() 
                    {
                        Author = createDto.UserID,
                        BranchID = ObjectId.GenerateNewId(),
                        UpdatedAt = creationDate,
                        BranchName = "New branch",
                        CreatedAt = creationDate,
                        Accesses = new List<BranchAccess>() 
                        {
                            new BranchAccess()
                            {
                                AccessType = BranchAccessType.ReadWrite,
                                UserID = createDto.UserID
                            }
                        },
                        BranchNodes = new List<BranchNode>()
                        {
                            new BranchNode()
                            {
                                BranchNodeID = ObjectId.GenerateNewId(),
                                Comment = string.Empty,
                                CreatedAt = creationDate,
                                CreatorID = createDto.UserID,
                                Title = "New title",
                                UpdatedAt = creationDate,
                                DocumentElement = createDto.Type switch
                                {
                                    DocumentElementType.Image => new Image(),
                                    DocumentElementType.NumberedList => new NumberedList(),
                                    DocumentElementType.Paragraph => new Paragraph(),
                                    DocumentElementType.Table => new Table(),
                                    _ => null
                                }
                            }
                        }
                    }
                }
            };

            return documentElement;
        }
    }
}