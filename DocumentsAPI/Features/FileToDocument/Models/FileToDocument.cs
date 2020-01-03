using DataAccessLayer.KernelModels;

namespace DocumentsAPI.Features.FileToDocument.Models
{
    public class FileToDocument : UniqueEntity
    {
        public long FileID { get; set; }
        public long DocumentID { get; set; }


        public FileToDocument()
        {
            FileID = -1;
            DocumentID = -1;
        }
        public FileToDocument(long fileID, long documentID)
        {
            FileID = fileID;
            DocumentID = documentID;
        }
    }
}