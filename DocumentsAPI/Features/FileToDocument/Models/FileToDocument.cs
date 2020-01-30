using Domain.KernelModels;

using MongoDB.Bson;


namespace DocumentsAPI.Features.FileToDocument.Models
{
    public class FileToDocument : UniqueEntity
    {
        public ObjectId FileID { get; set; }
        public ObjectId DocumentID { get; set; }


        public FileToDocument() {}
        public FileToDocument(ObjectId fileID, ObjectId documentID)
        {
            FileID = fileID;
            DocumentID = documentID;
        }
    }
}