using MongoDB.Bson;

namespace Application.Documents.DocumentElement.Models
{
    public class DocumentElementRenameDto
    {
        public ObjectId DocumentElementID { get; set; }
        public ObjectId UserID { get; set; }
        public ObjectId BranchID { get; set; }
        public string NewName { get; set; }
    }
}