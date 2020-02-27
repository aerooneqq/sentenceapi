using Domain.DocumentElements;
using MongoDB.Bson;

namespace Application.Documents.DocumentElement.Models
{
    public class DocumentElementCreateDto
    {
        public ObjectId ParentDocumentID { get; set; }
        public ObjectId UserID { get; set; }
        public ObjectId ParentItemID { get; set; }
        public DocumentElementType Type { get; set; }
        public string Name { get; set; }
    }
}