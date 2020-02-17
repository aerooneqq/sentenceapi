using System.Net;
using System.Threading.Tasks;

using Domain.Workplace.DocumentsStorage;

using MongoDB.Bson;

using SentenceAPI.StartupHelperClasses;

using SharedLibrary.Events.Exceptions;
using SharedLibrary.Events.Interfaces;


namespace SentenceAPI.Features.Workplace.DocumentsStorage.Events
{
    /// <summary>
    /// When the file is created the corresponding document must be also created.
    /// </summary>
    class FileCreationEvent : IDomainEvent
    {
        #region Event properties
        private readonly ObjectId userID;
        private readonly DocumentFile file;
        #endregion

        private readonly string documentsApiUrl;


        public FileCreationEvent(DocumentFile file, ObjectId userID, int documentType)
        {
            this.userID = userID;
            this.file = file;

            documentsApiUrl = $"{Startup.OtherApis[OtherApis.DocumentsAPI]}/documentsapi/fileToDocument?fileID={file.ID}&" +
                              $"userID={userID}&fileName={file.FileName}&documentType={documentType}";
        }


        public async Task Handle()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(documentsApiUrl);
                request.Method = "PUT";

                HttpWebResponse response = (HttpWebResponse) (await request.GetResponseAsync().ConfigureAwait(false));
            }
            catch (WebException) 
            {
                throw new DomainEventException("Error occured on the document's API server");
            }
        }
    }
}