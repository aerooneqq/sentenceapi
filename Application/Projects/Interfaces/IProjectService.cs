using System.Collections.Generic;
using System.Threading.Tasks;

using Application.Projects.Dto;

using Domain.KernelInterfaces;

using MongoDB.Bson;


namespace Application.Projects
{
    public interface IProjectService : IService
    {
        Task<IEnumerable<ProjectShortDto>> GetUserShortProjectsAsync(ObjectId userID);
        Task<ProjectShortDto> CreateProjectAsync(ObjectId authorID, string name, string description);
        Task<ProjectShortDto> GetProjectInfoAsync(ObjectId projectID);
        Task DeleteProject(ObjectId projectID);
        Task<ProjectShortDto> UpdateProject(ProjectUpdateDto update);
        Task<ProjectShortDto> CreateNewDocumentInProject(ObjectId projectID, ObjectId userID, string documentName,
                                                    ObjectId templateID);
        Task InviteUserInProject(ObjectId projectID, ObjectId userID);
        Task<IEnumerable<ProjectUserDto>> GetProjectParticipants(ObjectId projectID);
        Task<IEnumerable<ProjectDocumentDto>> GetProjectDocuments(ObjectId projectID);
    }
}