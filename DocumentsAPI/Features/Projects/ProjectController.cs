using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Application.Projects;
using Application.Projects.Dto;
using Application.Requests.Interfaces;
using Application.Tokens.Interfaces;

using DataAccessLayer.Exceptions;

using Domain.Logs;
using Domain.Logs.Configuration;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using Newtonsoft.Json;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;


namespace DocumentsAPI.Features.Projects
{
    [ApiController, Route("documentsapi/[controller]"), Authorize]
    public class ProjectsController : ControllerBase
    {
        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IProjectService projectService;
        private readonly IRequestService requestService;
        private readonly ITokenService tokenService;
        #endregion

        private readonly LogConfiguration logConfiguration;

        public ProjectsController(IFactoriesManager factoriesManager)
        { 
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);

            logConfiguration = new LogConfiguration(GetType());

            factoriesManager.GetService<IProjectService>().TryGetTarget(out projectService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserProjects()
        {
            try
            {
                ObjectId userObjectID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));
                var projects = await projectService.GetUserShortProjectsAsync(userObjectID).ConfigureAwait(false);
            
                return new OkJson<IEnumerable<ProjectShortDto>>(projects);
            }
            catch (FormatException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>("One of query params was not in correct format");
            }
            catch (ArgumentException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>(ex.Message);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex) 
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectInfo([FromQuery]string projectID)
        {
            try
            {
                ObjectId projectObjectID = ObjectId.Parse(projectID);
                var project = await projectService.GetProjectInfoAsync(projectObjectID).ConfigureAwait(false);

                return new OkJson<ProjectShortDto>(project);
            }
            catch (FormatException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>("One of query params was not in correct format");
            }
            catch (ArgumentException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>(ex.Message);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex) 
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }

        [HttpGet("projectUsers")]
        public async Task<IActionResult> GetProjectUsers([FromQuery]string projectID)
        {
            try
            {
                ObjectId projectObjectID = ObjectId.Parse(projectID);
                var projectUsers = await projectService.GetProjectParticipants(projectObjectID).ConfigureAwait(false);

                return new OkJson<IEnumerable<ProjectUserDto>>(projectUsers);
            }
            catch (FormatException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>("One of query params was not in correct format");
            }
            catch (ArgumentException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>(ex.Message);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex) 
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }

        [HttpGet("projectDocuments")]
        public async Task<IActionResult> GetProjectDocuments([FromQuery]string projectID)
        {
            try
            {
                ObjectId projectObjectID = ObjectId.Parse(projectID);
                var projectDocuments = await projectService.GetProjectDocuments(projectObjectID).ConfigureAwait(false);

                return new OkJson<IEnumerable<ProjectDocumentDto>>(projectDocuments);
            }
            catch (FormatException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>("One of query params was not in correct format");
            }
            catch (ArgumentException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>(ex.Message);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex) 
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewProject([FromQuery]string name, [FromQuery]string description)
        {
            try
            {
                ObjectId authorObjectID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));
                var project = await projectService.CreateProjectAsync(authorObjectID, name, description)
                    .ConfigureAwait(false);

                return new OkJson<ProjectShortDto>(project);
            }
            catch (FormatException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>("One of query params was not in correct format");
            }
            catch (ArgumentException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>(ex.Message);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex) 
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProject([FromQuery]string projectID)
        {
            try
            {
                ObjectId projectObjectID = ObjectId.Parse(projectID);
                await projectService.DeleteProject(projectObjectID).ConfigureAwait(false);

                return new Ok();
            }
            catch (FormatException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>("One of query params was not in correct format");
            }
            catch (ArgumentException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>(ex.Message);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex) 
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProject() 
        {
            try
            {
                string requestBody = await requestService.GetRequestBody(Request).ConfigureAwait(false);
                ProjectUpdateDto update = JsonConvert.DeserializeObject<ProjectUpdateDto>(requestBody);
                ProjectShortDto updatedProject = await projectService.UpdateProject(update).ConfigureAwait(false);

                return new OkJson<ProjectShortDto>(updatedProject);
            }
            catch (FormatException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>("One of query params was not in correct format");
            }
            catch (ArgumentException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>(ex.Message);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex) 
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }

        [HttpPut("invite")]
        public async Task<IActionResult> InviteUserInProject([FromQuery]string userID, [FromQuery]string projectID)
        {
            try
            {
                ObjectId userObjectID = ObjectId.Parse(userID);
                ObjectId projectObjectID = ObjectId.Parse(projectID);

                await projectService.InviteUserInProject(projectObjectID, userObjectID).ConfigureAwait(false);

                return new Ok();
            }
            catch (FormatException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>("One of query params was not in correct format");
            }
            catch (ArgumentException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>(ex.Message);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex) 
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }

        [HttpPut("document")]
        public async Task<IActionResult> CreateNewDocumentInProject([FromQuery]string projectID, [FromQuery]string documentName,
                                                                    [FromQuery]string templateID)
        {
            try
            {
                ObjectId templateObjectID = ObjectId.Parse(templateID);
                ObjectId projectObjectID = ObjectId.Parse(projectID);
                ObjectId userObjectID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));
                
                await projectService.CreateNewDocumentInProject(projectObjectID, userObjectID, documentName, templateObjectID)
                    .ConfigureAwait(false);

                return new Ok();
            }
            catch (FormatException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>("One of query params was not in correct format");
            }
            catch (ArgumentException ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new BadSentRequest<string>(ex.Message);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex) 
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }
    }
}