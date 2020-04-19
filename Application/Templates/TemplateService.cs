using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Filters;
using Domain.Date;
using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Templates;
using MongoDB.Bson;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;

namespace Application.Templates
{
    public class TemplatesService : ITemplateService
    {
        private const string databaseConfigFile = "./configs/mongo_database_config.json";
        private readonly IDatabaseService<Template> database;


        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly LogConfiguration logConfiguration;

        private readonly IDateService dateService;


        public TemplatesService(IDatabaseManager databaseManager, IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IDateService>().TryGetTarget(out dateService);

            IConfigurationBuilder configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();
        }

        public async Task<Template> CreateNewTemplate(TemplateCreationDto dto)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                Template template = Template.GetEmptyTemplate(dto.AuthorID, dto.Name, dto.Organization, 
                    dto.Description, dateService);
                
                await database.Insert(template).ConfigureAwait(false);

                return template;
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while creating new template");
            }
        }

        public async Task DeleteTemplate(ObjectId templateID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                await database.Delete(new EqualityFilter<ObjectId>("_id", templateID));
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while creating new template");
            }
        }

        public async Task<IEnumerable<Template>> GetPublishedTemplates()
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                return await database.Get(new EqualityFilter<bool>("isPublished", true)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocured while getting templates");
            }
        }

        public async Task<Template> GetTemplateByID(ObjectId templateID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                Template template = (await database.Get(new EqualityFilter<ObjectId>("_id", templateID))
                    .ConfigureAwait(false)).FirstOrDefault();

                if (template is null)
                {
                    throw new ArgumentException("No template for such an ID");
                } 

                return template;
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while getting template");
            }
        }

        public async Task<IEnumerable<Template>> GetUserTemplates(ObjectId userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var getFilter = new EqualityFilter<ObjectId>("userID", userID);
                
                return await database.Get(getFilter).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while searching for templates");
            }
        }

        public async Task<Template> IncreaseDocumentCountForTemplate(ObjectId templateID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var idFilter = new EqualityFilter<ObjectId>("_id", templateID);
                Template template = (await database.Get(idFilter).ConfigureAwait(false)).FirstOrDefault();

                if (template is null)
                {
                    throw new ArgumentException("No template with such an ID");
                }

                ++template.DocumentCount;

                await database.Update(template, new [] {"documentCount"});

                return template;
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while searching for templates");
            }
        }

        public async Task<IEnumerable<Template>> SearchForPublishedTemplates(string query)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                RegexFilter nameFilter = new RegexFilter("name", query);
                return await database.Get(nameFilter).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while searching for templates");
            }
        }

        public async Task<Template> UpdateTemplate(TemplateUpdateDto dto)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var idFilter = new EqualityFilter<ObjectId>("_id", dto.TemplateID);
                Template template = (await database.Get(idFilter).ConfigureAwait(false)).FirstOrDefault();

                if (template is null)
                {
                    throw new ArgumentException("No template with such an ID");
                }
            
                var (newPublished, newName, newDesc, newOrg, newItems) = dto;
                template.Update(newPublished, newName, newDesc, newOrg, newItems);

                await database.Update(template).ConfigureAwait(false);
                
                return template;
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while searching for templates");
            }
        }
    }
}