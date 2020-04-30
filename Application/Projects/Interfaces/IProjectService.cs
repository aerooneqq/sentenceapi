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
        Task<ProjectDto> CreateProjectAsync(ObjectId authorID, string name, string description);
        Task<ProjectDto> GetProjectInfoAsync(ObjectId projectID);   
        Task DeleteProject(ObjectId projectID);
    }
}