using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Application.Documents.DocumentStructure.Exceptions;
using Application.Documents.DocumentStructure.Interfaces;
using Application.Documents.DocumentStructure.Models;

using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;

using Domain.DocumentStructureModels;
using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Date;

using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

using MongoDB.Bson;
using Domain.DocumentStructureModels.ItemStatus;

namespace Application.Documents.DocumentStructure
{
    public class DocumentStructureService : IDocumentStructureService
    { 
        private static readonly string databaseConfigFile = "./configs/mongo_database_config.json";
        
        #region Databases
        private readonly IDatabaseService<DocumentStructureModel> database;
        private readonly IConfigurationBuilder configurationBuilder;
        #endregion

        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IDateService dateService;
        #endregion

        private readonly LogConfiguration logConfiguration;

        public DocumentStructureService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager)
        {
            factoriesManager.GetService<IDateService>().TryGetTarget(out dateService);
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);

            databaseManager.MongoDBFactory.GetDatabase<DocumentStructureModel>()
                .TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            logConfiguration = new LogConfiguration(GetType());
        }


        public async Task<DocumentStructureModel> GetDocumentStructureAsync(ObjectId documentID)
        { 
            try
            {
                await database.Connect().ConfigureAwait(false);
                var documentStructure = (await database.Get(new EqualityFilter<ObjectId>("parentDocumentID", documentID))
                    .ConfigureAwait(false)).FirstOrDefault();

                return documentStructure;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while getting document structure.", ex);
            }
        }

        public async Task<DocumentStructureModel> GetStructureByID(ObjectId structureID) 
        { 
            try
            {
                await database.Connect().ConfigureAwait(false);
                var documentStructure = (await database.Get(new EqualityFilter<ObjectId>("_id", structureID))
                    .ConfigureAwait(false)).FirstOrDefault();

                return documentStructure;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while getting document structure.", ex);
            }
        }

        public async Task UpdateStructureAsync(DocumentStructureModel documentStructure, ItemUpdateDto itemUpdateDto,
                                               ObjectId userID)
        {
            try
            {
                Item itemToUpdate = FindItemInList(documentStructure.Items, itemUpdateDto.ItemID);

                if (itemToUpdate is null)
                {
                    throw new ItemNotFoundException("The item deos not present in the structure");
                }

                if (itemUpdateDto.NewInnerItem is {})
                {
                    InsertNewItemInItem(itemToUpdate, itemUpdateDto.NewInnerItem, userID);
                }

                if (itemUpdateDto.NewName is {} && itemUpdateDto.NewName.Length > 0)
                {
                    itemToUpdate.Name = itemUpdateDto.NewName;
                }

                itemToUpdate.UpdatedAt = dateService.Now;

                await database.Update(documentStructure).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while updating the structure", ex);
            }
        }

        private void InsertNewItemInItem(Item item, NewInnerItem newInnerItem, ObjectId userID)
        {
            Item newItem = new Item
            {
                CreatedAt = dateService.Now,
                ID = ObjectId.GenerateNewId(),
                Items = new List<Item>(),
                Name = newInnerItem.Name,
                UpdatedAt = dateService.Now,
                ItemStatus = new ItemStatus() 
                {
                    Accesses = new List<ItemUserRole>() {
                        new ItemUserRole()
                        {
                            Access = AccessType.CanAccess,
                            UserID = userID
                        },
                    },
                    ItemType = newInnerItem.ItemType
                }
            };

            item.Items.Insert(newInnerItem.Position, newItem);
        }

        private Item FindItemInList(List<Item> items, ObjectId objectId)
        {
            Item searchResult;

            int itemIndex = items.FindIndex(item => item.ID == objectId);

            if (itemIndex > -1)
            {
                return items[itemIndex];
            }
            else
            {
                FindItemRecursive(items, objectId, out searchResult);
            }

            return searchResult;
        }

        private static void FindItemRecursive(IEnumerable<Item> items, ObjectId objectId, out Item searchResult)
        {
            searchResult = null;

            foreach (Item item in items)
            {
                int itemIndex = item.Items.FindIndex(item => item.ID == objectId);
                if (itemIndex > -1)
                {
                    searchResult = item.Items[itemIndex];
                }
                else
                {
                    FindItemRecursive(item.Items, objectId, out searchResult);
                }
            }
        }

        public async Task<ObjectId> CreateNewDocumentStructure(ObjectId documentID, string documentName, ObjectId userID)
        {
            try
            {
                DocumentStructureModel documentStructure = DocumentStructureModel.
                    GetNewDocumentStructure(dateService.Now, documentID, userID, documentName);

                await database.Connect().ConfigureAwait(false);
                await database.Insert(documentStructure).ConfigureAwait(false);

                return documentStructure.ID;
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("The error occured while creating the document structure");
            }
        }
    }
}