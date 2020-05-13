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
    public class BranchService : IBranchService
    {
        private const string databaseConfigFile = "./configs/mongo_database_config.json";

        private readonly IDatabaseService<DocumentElementWrapper> database;

        private readonly IDocumentStructureService documentStructureService;

        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IDateService dateService;

        private readonly LogConfiguration logConfiguration;


        public BranchService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager) 
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

        public async Task<DocumentElementDto> CreateNewBranchAsync(ObjectId documentElementID, string branchName,
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

                element.Branches.Add(Branch.GetNewBranch(branchName, dateService, element.Type,
                    new List<BranchAccess>() { new BranchAccess(userID, BranchAccessType.ReadWrite) }, userID));

                await database.Update(element).ConfigureAwait(false);

                var dto = new DocumentElementDto(element);
                dto.SetBranches(element.Branches, userID);

                return dto;
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occurred while creating new branch");
            }
        }

        public async Task<DocumentElementDto> DeleteBranchAsync(ObjectId elementID, ObjectId branchID, ObjectId userID)
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

                var branch = element.Branches.Find(branch => branch.BranchID == branchID);
                if (branch is null)
                {
                    throw new ArgumentException("There is no branch for given id");
                }

                element.Branches.Remove(branch);
                if (element.Branches.Count == 0)
                {
                    element.Branches.Add(Branch.GetNewBranch("New branch", dateService, element.Type,
                        new List<BranchAccess>() { new BranchAccess(userID, BranchAccessType.ReadWrite)}, userID));
                }
                
                element.CurrentBranchID = element.Branches.First().BranchID;
                if (element.Branches.First().BranchNodes.Count == 0)
                {
                    element.Branches.First().BranchNodes.Add(BranchNode.GetEmptyNode(element.Type, dateService,
                        userID, "New node", "Comment"));
                }

                element.CurrentBranchNodeID = element.Branches.First().BranchNodes.First().BranchNodeID;

                await database.Update(element).ConfigureAwait(false);

                var dto = new DocumentElementDto(element);
                dto.SetBranches(element.Branches, userID);

                return dto;
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occurred while deleting branch");
            }
        }
    }
}