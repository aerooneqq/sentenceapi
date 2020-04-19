using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Templates;
using MongoDB.Bson;

namespace Application.Templates
{
    public interface ITemplateService
    {
        Task<Template> CreateNewTemplate(TemplateCreationDto dto);
        Task<IEnumerable<Template>> GetPublishedTemplates();
        Task<IEnumerable<Template>> SearchForPublishedTemplates(string query);
        Task<Template> GetTemplateByID(ObjectId templateID);
        Task<Template> UpdateTemplate(TemplateUpdateDto dto);
        Task DeleteTemplate(ObjectId templateID);
        Task<IEnumerable<Template>> GetUserTemplates(ObjectId userID);
        Task<Template> IncreaseDocumentCountForTemplate(ObjectId templateID);
    }
}