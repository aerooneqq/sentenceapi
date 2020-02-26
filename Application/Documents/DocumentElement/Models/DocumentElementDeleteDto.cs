using MongoDB.Bson;

namespace Application.Documents.DocumentElement.Models
{
    public class DocumentElementDeleteDto
    {
        public ObjectId DocumentElementID { get; set; }
    }
}