using System.Threading.Tasks;

using Application.Documents.DocumentStructure.Interfaces;

using MongoDB.Bson;

using SharedLibrary.Events.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace DocumentsAPI.Features.FileToDocument.Events
{
    public class FileToDocumentAssociated : IDomainEvent
    {
        private readonly IDocumentStructureService documentStructureService; 

        private readonly ObjectId documentID;
        private readonly ObjectId userID;
        private readonly string documentName;


        public FileToDocumentAssociated(ObjectId documentID, ObjectId userID, 
                                        string documentName, IFactoriesManager factoriesManager)
        {
            this.documentID = documentID;
            this.userID = userID;
            this.documentName = documentName;

            factoriesManager.GetService<IDocumentStructureService>().TryGetTarget(out documentStructureService);
        }


        public async Task Handle()
        {
            await documentStructureService.CreateNewDocumentStructure(documentID, documentName, userID).
                ConfigureAwait(false);
        }
    }
}