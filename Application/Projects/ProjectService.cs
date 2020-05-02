using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Documents.Documents.Interfaces;
using Application.Documents.DocumentStructure.Interfaces;
using Application.Documents.FileToDocument.Interfaces;
using Application.Projects.Dto;
using Application.Templates;
using Application.Templates.Interfaces;
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
using Domain.Models.Document;
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
        private readonly IDocumentService documentService;
        private readonly IDocumentStructureService documentStructureService;
        private readonly ITemplateService templateService;
        private readonly IFileToDocumentService fileToService; 
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly LogConfiguration logConfiguration;

        private readonly IDateService dateService;


        public ProjectService(IFactoriesManager factoriesManager, IDatabaseManager databaseManager) 
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IDateService>().TryGetTarget(out dateService);
            factoriesManager.GetService<IUserService<UserInfo>>().TryGetTarget(out userService);
            factoriesManager.GetService<IUserPhotoService>().TryGetTarget(out userPhotoService);
            factoriesManager.GetService<IDocumentService>().TryGetTarget(out documentService);
            factoriesManager.GetService<IFileToDocumentService>().TryGetTarget(out fileToService);
            factoriesManager.GetService<IDocumentStructureService>().TryGetTarget(out documentStructureService);
            factoriesManager.GetService<ITemplateService>().TryGetTarget(out templateService);

            logConfiguration = new LogConfiguration(GetType());

            databaseManager.MongoDBFactory.GetDatabase<Project>().TryGetTarget(out database);
            IConfigurationBuilder configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();
        }


        public async Task<ProjectShortDto> CreateProjectAsync(ObjectId authorID, string name, string description)
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

                return new ProjectShortDto(newProject);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while creating new projects");
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

        public async Task<ProjectShortDto> UpdateProject(ProjectUpdateDto update)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var getFilter = new EqualityFilter<ObjectId>("_id", update.ProjectID);
                Project project = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
                if (project is null)
                {
                    throw new ArgumentException("No project was found for given ID");
                }

                UpdateProject(project, update);

                await database.Update(project).ConfigureAwait(false);

                return new ProjectShortDto(project);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while deleting project");
            }
        }

        private static void UpdateProject(Project project, ProjectUpdateDto update)
        {
            if (update.Description is {})
            {
                project.Description = update.Description;
            }

            if (update.Name is {})
            {
                project.Name = update.Name;
            }
        }

        public async Task<ProjectShortDto> CreateNewDocumentInProject(ObjectId projectID, ObjectId userID, string documentName,
                                                                 ObjectId templateID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var getFilter = new EqualityFilter<ObjectId>("_id", projectID);
                Project project = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();
                
                ObjectId documentID = await documentService.CreateEmptyDocument(userID, documentName, DocumentType.Project)
                    .ConfigureAwait(false);

                project.Documents.Add(documentID);

                ObjectId structureID = await documentStructureService.CreateNewDocumentStructure(documentID, documentName, userID)
                    .ConfigureAwait(false);
                var documentStructure = await documentStructureService.GetStructureByID(structureID).ConfigureAwait(false);

                if (documentStructure is null)
                {
                    throw new Exception("Document structure was not created. FATAL ERROR");
                }

                var template = await templateService.GetTemplateByID(templateID);
                if (template is null)
                {
                    throw new ArgumentException("Template with such an ID does not exists");
                }

                documentStructure.ApplyTemplate(template.Items);

                await documentStructureService.UpdateDocumentStructureAsync(documentStructure).ConfigureAwait(false);
                await templateService.IncreaseDocumentCountForTemplate(template.ID).ConfigureAwait(false);
                await database.Update(project).ConfigureAwait(false);

                return new ProjectShortDto(project);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while deleting project");
            }
        }

        public async Task InviteUserInProject(ObjectId projectID, ObjectId userID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var getFilter = new EqualityFilter<ObjectId>("_id", projectID);
                Project project = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();

                if (project.InvitedUsers.Contains(userID))
                {
                    throw new ArgumentException("The user is already invited");
                }

                UserInfo user = await userService.GetAsync(userID).ConfigureAwait(false);
                user.UserInvitedProjects.Add(project.ID);

                project.InvitedUsers.Add(user.ID);

                await userService.UpdateAsync(user).ConfigureAwait(false);
                await database.Update(project).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while deleting project");
            }
        }
        public async Task<IEnumerable<ProjectUserDto>> GetProjectParticipants(ObjectId projectID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var getFilter = new EqualityFilter<ObjectId>("_id", projectID);
                Project project = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();

                return project.Users.Select(user => 
                {
                    UserInfo userInfo = userService.GetAsync(user.UserID).GetAwaiter().GetResult();
                    var userPhoto = userPhotoService.GetPhotoAsync(userInfo.ID).GetAwaiter().GetResult();
                    byte[] rawPhoto = userPhotoService.GetRawPhotoAsync(userPhoto.CurrentPhotoID).GetAwaiter().GetResult();

                    return new ProjectUserDto
                    {
                        AuthorName = userInfo.Name,
                        Role = user.Role,
                        UserID = userInfo.ID,
                        UserPhoto = rawPhoto
                    };
                });
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while deleting project");
            }
        }

        public async Task<IEnumerable<ProjectDocumentDto>> GetProjectDocuments(ObjectId projectID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var getFilter = new EqualityFilter<ObjectId>("_id", projectID);
                Project project = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();

                return project.Documents.Select(documentID => 
                {
                    Document document = documentService.GetDocumentByID(documentID).GetAwaiter().GetResult();
                    return new ProjectDocumentDto()
                    {
                        DocumentID = documentID,
                        DocumentName = document.Name,
                        DocumentStatus = document.DocumentStatus,
                    };
                });
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while deleting project");
            }
        }

        public async Task<ProjectShortDto> GetProjectInfoAsync(ObjectId projectID)
        {
            try
            {
                await database.Connect().ConfigureAwait(false);
                var getFilter = new EqualityFilter<ObjectId>("_id", projectID);
                Project project = (await database.Get(getFilter).ConfigureAwait(false)).FirstOrDefault();

                return new ProjectShortDto(project);
            }
            catch (Exception ex) when (ex.GetType() != typeof(ArgumentException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error ocurred while deleting project");
            }
        }
    }
}