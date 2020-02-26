using Domain.DocumentElements;
using MongoDB.Bson;

namespace Application.Documents.DocumentElement.Models
{
    public class DocumentElementContentUpdateDto
    {
        public ObjectId DocumentElementID { get; set; }
        public ObjectId UserID { get; set; }
        public ObjectId BranchID { get; set; }
        public ObjectId BranchNodeID { get; set; }
        public DocumentElementType ElementType { get; set; }
        public string NewContent { get; set; }
    }
}