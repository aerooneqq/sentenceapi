using System;
using System.Collections.Generic;
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
using Domain.DocumentStructureModels;
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

        private readonly IDocumentStructureService documentStructureService;

        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IDateService dateService;

        private readonly LogConfiguration logConfiguration;


        public DocumentElementService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager) 
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

        public async Task<IEnumerable<DocumentElementDto>> GetDocumentElementsAsync(ObjectId documentID, 
                                                                                    ObjectId parentItemID, ObjectId userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                var documentStructure = await documentStructureService.GetDocumentStructureAsync(documentID)
                    .ConfigureAwait(false);
                if (documentStructure is null)
                {
                    throw new ArgumentException("No document structure found");
                }

                FindItemRecursive(documentStructure.Items, parentItemID, out Item item);
                if (item is null)
                {
                    throw new ArgumentException("No item for given id");
                }

                var elementWrappers = (await database.Get(new EqualityFilter<ObjectId>("parentItemID", parentItemID))
                    .ConfigureAwait(false)).ToDictionary(w => w.ID, w => w);
                if (elementWrappers is null)
                {
                    return null;
                }

                return item.ElementsIds is null ? new DocumentElementDto[0]{} : item.ElementsIds.Select(id => 
                {
                    var wrapper = elementWrappers.GetValueOrDefault(id);

                    if (wrapper is null)
                    {
                        return null;
                    }

                    DocumentElementDto dto = new DocumentElementDto(wrapper);
                    dto.SetBranches(wrapper.Branches, userID);
                    return dto;
                }).Where(dto => dto is {});
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The erorr occured while getting the document");
            }
        }

        private void FindItemRecursive(List<Item> items, ObjectId itemID, out Item searchResult)
        {
            searchResult = items.Find(item => item.ID == itemID);
            if (searchResult is {})
            {
                return;
            }

            foreach (Item item in items)
            {
                FindItemRecursive(item.Items, itemID, out searchResult);
                if (searchResult is {})
                {
                    return;
                }
            }
        }

        public async Task RenameDocumentElementAsync(DocumentElementRenameDto renameDto)
        {
            try
            {
                await database.Connect();
                if (string.IsNullOrEmpty(renameDto.NewName) || string.IsNullOrWhiteSpace(renameDto.NewName))
                {
                    throw new ArgumentException("New name length must be greater than 0");
                }

                var getFilter = GetGetDocumentElWrapperByIdFilter(renameDto.DocumentElementID);

                DocumentElementWrapper document = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
                if (document is null)
                {
                    throw new ArgumentException("There is no document with such an ID");
                }

                Branch currentBranch = document.Branches.FirstOrDefault(b => b.BranchID == renameDto.BranchID);
                if (currentBranch is null)
                {
                    throw new ArgumentException("Branch with such an id does not exist");
                }

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

        private FilterBase GetGetDocumentElWrapperByIdFilter(ObjectId id) =>
            new EqualityFilter<ObjectId>(typeof(DocumentElementWrapper).GetBsonPropertyName("ID"), id);

        public async Task UpdateContentInBranchNodeAsync(DocumentElementContentUpdateDto updateDto) 
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var getFilter = GetGetDocumentElWrapperByIdFilter(updateDto.DocumentElementID);
                
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

        public async Task DeleteDocumentElementAsync(ObjectId elementID) 
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                await database.Delete(new EqualityFilter<ObjectId>("_id", elementID)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while deleting the element");
            }
        }

        public async Task<ObjectId> CreateNewDocumentElementAsync(DocumentElementCreateDto createDto) 
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var documentElementWrapper = GetEmptyDocumentWrapper(createDto);

                await documentStructureService.InsertDocumentElementInOrder(createDto.ParentDocumentID, 
                    createDto.ParentItemID, documentElementWrapper.ID, createDto.InsertionIndex).ConfigureAwait(false);
                await database.Insert(documentElementWrapper).ConfigureAwait(false);

                return documentElementWrapper.ID;
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while creationg new element");
            }
        }

        private DocumentElementWrapper GetEmptyDocumentWrapper(DocumentElementCreateDto createDto)
        {
            DateTime creationDate = dateService.Now;
            ObjectId newDocElementID = ObjectId.GenerateNewId();
            List<Branch> branches = new List<Branch>() 
            {
                Branch.GetNewBranch("New branch", dateService, createDto.Type, 
                    new List<BranchAccess>() { new BranchAccess(createDto.UserID, BranchAccessType.ReadWrite)}, 
                    createDto.UserID)
            };

            DocumentElementWrapper documentElement = new DocumentElementWrapper()
            {
                CreatorID = createDto.UserID,
                ID = newDocElementID,
                ParentDocumentID = createDto.ParentDocumentID,
                ParentItemID = createDto.ParentItemID,
                Type = createDto.Type,
                CurrentBranchID = branches[0].BranchID,
                CurrentBranchNodeID = branches[0].BranchNodes[0].BranchNodeID,
                Branches = branches
            };

            return documentElement;
        }

        public async Task ChangeSelectedBranch(ObjectId documentElementID, ObjectId branchID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                FilterBase getFilter = new EqualityFilter<ObjectId>("_id", documentElementID);
                var documentElement = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
                if (documentElement is null)
                {
                    throw new ArgumentException("No document element for given id");
                }

                documentElement.CurrentBranchID = branchID;

                await database.Update(documentElement).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while changing selected branch");
            }
        }

        public async Task ChangeSelectedBranchNode(ObjectId documentElementID, ObjectId branchNodeID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                FilterBase getFilter = new EqualityFilter<ObjectId>("_id", documentElementID);
                var documentElement = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
                if (documentElement is null)
                {
                    throw new ArgumentException("No document element for given id");
                }

                documentElement.CurrentBranchNodeID = branchNodeID;

                await database.Update(documentElement).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while changing selected branch node id");
            }
        }

        public async Task<DocumentElementDto> GetDocumentElementAsync(ObjectId elementID, ObjectId userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);

                FilterBase getFilter = new EqualityFilter<ObjectId>("_id", elementID);
                var element = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
                if (element is null)
                {
                    throw new ArgumentException("No document element was found for id");
                }

                var wrapper = new DocumentElementDto(element);
                wrapper.SetBranches(element.Branches, userID);

                return wrapper;
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while getting the document element");
            }
        }
    }
}