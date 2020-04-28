using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Templates;
using MongoDB.Bson;

namespace Application.Templates.Interfaces
{
    public interface ITemplateService
    {
        Task<TemplateDto> CreateNewTemplate(TemplateCreationDto dto);
        Task<IEnumerable<TemplateDto>> GetPublishedTemplates();
        Task<IEnumerable<TemplateDto>> SearchForPublishedTemplates(string query);
        Task<TemplateDto> GetTemplateByID(ObjectId templateID);
        Task<TemplateDto> UpdateTemplate(TemplateUpdateDto dto);
        Task DeleteTemplate(ObjectId templateID);
        Task<IEnumerable<TemplateDto>> GetUserTemplates(ObjectId userID);
        Task<TemplateDto> IncreaseDocumentCountForTemplate(ObjectId templateID);
        Task<IEnumerable<TemplateDto>> SearchForUserTemplates(ObjectId userID, string query);
    }
}