using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Projects.Dto;
using Application.UserPhoto.Interfaces;
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
using Domain.Projects;
using Domain.Users;
using MongoDB.Bson;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;


namespace Application.Projects
{
    public class ProjectService : IProjectService 
    {
        private const string databaseConfigFile = "./configs/mongo_database_config.json";
        private readonly IDatabaseService<Project> database;

        private readonly IUserService<UserInfo> userService;
        private readonly IUserPhotoService userPhotoService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly LogConfiguration logConfiguration;

        private readonly IDateService dateService;


        public ProjectService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager) 
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IDateService>().TryGetTarget(out dateService);
            factoriesManager.GetService<IUserService<UserInfo>>().TryGetTarget(out userService);
            factoriesManager.GetService<IUserPhotoService>().TryGetTarget(out userPhotoService);

            logConfiguration = new LogConfiguration(GetType());

            databaseManager.MongoDBFactory.GetDatabase<Project>().TryGetTarget(out database);
            IConfigurationBuilder configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();
        }


        public async Task<ProjectDto> CreateProjectAsync(ObjectId authorID, string name, string description)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                
                Domain.UserPhoto.UserPhoto userPhoto = await userPhotoService.GetPhotoAsync(authorID).ConfigureAwait(false);
                if (userPhoto is null)
                {
                    throw new ArgumentException("User photo was not found for given ID");
                }

                byte[] userRawPhoto = await userPhotoService.GetRawPhotoAsync(userPhoto.CurrentPhotoID)
                    .ConfigureAwait(false);
                
                UserInfo author = await userService.GetAsync(authorID).ConfigureAwait(false);
                if (author is null)
                {
                    throw new ArgumentException("Author was not found for given ID");
                }

                Project newProject = Project.GetEmptyProject(authorID, name, description, dateService);                
                author.UserRelatedProjects.Add(newProject.ID);
                await userService.UpdateAsync(author).ConfigureAwait(false);

                await database.Insert(newProject).ConfigureAwait(false);

                return new ProjectDto(newProject, new Dictionary<ObjectId, UserInfo>() { [authorID] = author },
                                      new Dictionary<ObjectId, byte[]>() { [authorID] = userRawPhoto });
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while creating new projects");
            }
        }

        public async Task<ProjectDto> GetProjectInfoAsync(ObjectId projectID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var getFilter = new EqualityFilter<ObjectId>("_id", projectID);
                Project project = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
                if (project is null)
                {
                    throw new ArgumentException("No project was found for given ID");
                }

                IDictionary<ObjectId, UserInfo> participants = project.Users.Select(user => 
                {
                    var getFilter = new EqualityFilter<ObjectId>("_id", user.UserID);
                    UserInfo userInfo = userService.GetAsync(user.UserID).GetAwaiter().GetResult();
                    if (userInfo is null)
                    {
                        throw new ArgumentException("User was not found for given ID");
                    }

                    return userInfo;
                }).ToDictionary(user => user.ID, user => user);

                IDictionary<ObjectId, byte[]> userPhotos = project.Users.Select(user =>
                {
                    Domain.UserPhoto.UserPhoto userPhoto = userPhotoService.GetPhotoAsync(user.UserID)
                        .GetAwaiter().GetResult();
                    byte[] rawUserPhoto = userPhotoService.GetRawPhotoAsync(userPhoto.CurrentPhotoID)
                        .GetAwaiter().GetResult();

                    return (userPhoto.UserID, rawUserPhoto);
                }).ToDictionary(tuple => tuple.UserID, tuple => tuple.rawUserPhoto);

                return new ProjectDto(project, participants, userPhotos);              
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while getting project info");
            }
        }

        public async Task<IEnumerable<ProjectShortDto>> GetUserShortProjectsAsync(ObjectId userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                UserInfo user = await userService.GetAsync(userID).ConfigureAwait(false);
                List<ObjectId> projectsIDs = user.UserRelatedProjects;
                if (projectsIDs is null || projectsIDs.Count == 0)
                {
                    return new List<ProjectShortDto>();
                }
                
                return projectsIDs.Select(projectID => 
                {
                    var filter = new EqualityFilter<ObjectId>("_id", projectID);
                    Project project =  database.Get(filter).GetAwaiter().GetResult().FirstOrDefault();
                    if (project is null)
                    {
                        throw new ArgumentException("Project was not found for given id");
                    }

                    return project;
                }).Select(project => new ProjectShortDto(project));
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while getting user projects");
            }
        }

        public async Task DeleteProject(ObjectId projectID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var getFilter = new EqualityFilter<ObjectId>("_id", projectID);
                Project project = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
                if (project is null)
                {
                    throw new ArgumentException("Project was not found for given ID");
                }

                ObjectId authorID = project.Users.Where(u => u.Role == ProjectRole.Creator).First().UserID;
                UserInfo author = await userService.GetAsync(authorID).ConfigureAwait(false);
                if (author is null)
                {
                    throw new ArgumentException("No author was found for template");
                }

                author.UserRelatedProjects.Remove(project.ID);
                await userService.UpdateAsync(author).ConfigureAwait(false);

                await database.Delete(getFilter).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while deleting project");
            }
        }
    }
}