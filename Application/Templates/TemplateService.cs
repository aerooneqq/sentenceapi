using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Templates.Interfaces;
using Application.Users.Interfaces;
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
using Domain.Users;
using MongoDB.Bson;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;

namespace Application.Templates
{
    public class TemplatesService : ITemplateService
    {
        private const string databaseConfigFile = "./configs/mongo_database_config.json";
        private readonly IDatabaseService<Template> database;

        private readonly IUserService<UserInfo> userService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly LogConfiguration logConfiguration;

        private readonly IDateService dateService;


        public TemplatesService(IDatabaseManager databaseManager, IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IDateService>().TryGetTarget(out dateService);
            factoriesManager.GetService<IUserService<UserInfo>>().TryGetTarget(out userService);

            databaseManager.MongoDBFactory.GetDatabase<Template>().TryGetTarget(out database);
            IConfigurationBuilder configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            logConfiguration = new LogConfiguration(GetType());
        }

        public async Task<TemplateDto> CreateNewTemplate(TemplateCreationDto dto)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                Template template = Template.GetEmptyTemplate(dto.AuthorID, dto.Name, dto.Organization, 
                    dto.Description, dateService);
                
                await database.Insert(template).ConfigureAwait(false);

                UserInfo user = await userService.GetAsync(dto.AuthorID);

                return new TemplateDto(template, user);
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

        public async Task<IEnumerable<TemplateDto>> GetPublishedTemplates()
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var templates = await database.Get(new EqualityFilter<bool>("published", true)).ConfigureAwait(false);
                IDictionary<ObjectId, UserInfo> users = templates.Select(template => template.AuthorID).ToHashSet()
                    .Select(id => 
                    {
                        if (userService.GetAsync(id).GetAwaiter().GetResult() is {} user)
                        {
                            return user;
                        }
                        throw new ArgumentException("No user fo given ID");
                    })
                    .ToDictionary(u => u.ID, u => u);

                return templates.Select(template => new TemplateDto(template, users[template.AuthorID])); 
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocured while getting templates");
            }
        }

        public async Task<TemplateDto> GetTemplateByID(ObjectId templateID)
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

                var user = await userService.GetAsync(template.AuthorID);

                if (user is null)
                {
                    throw new ArgumentException("No user for given ID");
                }

                return new TemplateDto(template, user);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while getting template");
            }
        }

        public async Task<IEnumerable<TemplateDto>> GetUserTemplates(ObjectId userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var getFilter = new EqualityFilter<ObjectId>("authorID", userID);
                
                var templates = await database.Get(getFilter).ConfigureAwait(false);
                var templatesAuthor = await userService.GetAsync(userID);
                if (templatesAuthor is null)
                {
                    throw new ArgumentException("No author for this template");
                }

                return templates.Select(template => new TemplateDto(template, templatesAuthor));
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while searching for templates");
            }
        }

        public async Task<TemplateDto> IncreaseDocumentCountForTemplate(ObjectId templateID)
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

                await database.Update(template).ConfigureAwait(false);

                var templateAuthor = await userService.GetAsync(template.AuthorID);
                if (templateAuthor is null)
                {
                    throw new ArgumentException("No user fot given ID");
                }

                return new TemplateDto(template, templateAuthor);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while searching for templates");
            }
        }

        public async Task<IEnumerable<TemplateDto>> SearchForPublishedTemplates(string query)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                RegexFilter nameFilter = new RegexFilter("name", query);
                var templates = await database.Get(nameFilter).ConfigureAwait(false);

                IDictionary<ObjectId, UserInfo> users = templates.Select(template => template.AuthorID).ToHashSet()
                    .Select(id => 
                    {
                        if (userService.GetAsync(id).GetAwaiter().GetResult() is {} user)
                        {
                            return user;
                        }
                        throw new ArgumentException("No user fo given ID");
                    })
                    .ToDictionary(u => u.ID, u => u);
        
                return templates.Select(template => new TemplateDto(template, users[template.AuthorID]));
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while searching for templates");
            }
        }

        public async Task<IEnumerable<TemplateDto>> SearchForUserTemplates(ObjectId userID, string query)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var userFilter = new EqualityFilter<ObjectId>("authorID", userID);
                var nameFilter = new RegexFilter("name", query);
                var getFilter = userFilter & nameFilter;

                var templates = await database.Get(getFilter).ConfigureAwait(false);
                var author = await userService.GetAsync(userID);
                if (author is null)
                {
                    throw new ArgumentException("No user fot given ID");
                }

                return templates.Select(template => new TemplateDto(template, author));
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while searching for templates");
            }
        }

        public async Task<TemplateDto> UpdateTemplate(TemplateUpdateDto dto)
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
            
                var (newPublished, newName, newDesc, newOrg, newItems, newLogo) = dto;
                template.Update(newPublished, newName, newDesc, newOrg, newItems, newLogo);

                await database.Update(template).ConfigureAwait(false);
                
                var templateAuthor = await userService.GetAsync(template.AuthorID).ConfigureAwait(false);

                return new TemplateDto(template, templateAuthor);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while searching for templates");
            }
        }
    }
}