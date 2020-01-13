using System.IO;
using System.Net;
using System.Threading.Tasks;

using MongoDB.Bson;

using SentenceAPI.Events.Exceptions;
using SentenceAPI.Events.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsStorage.Models;
using SentenceAPI.StartupHelperClasses;

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


        public FileCreationEvent(DocumentFile file, ObjectId userID)
        {
            this.userID = userID;
            this.file = file;

            documentsApiUrl = $"{Startup.OtherApis[OtherApis.DocumentsAPI]}/fileToDocument?fileID={file.ID}&" +
                $"userID={userID}&fileName={file.FileName}&documentType=0";
        }


        public async Task Handle()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(documentsApiUrl);
            request.Method = "PUT";
            
            HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync().ConfigureAwait(false));
            
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new DomainEventException("Error occured on the document server");
            }
        }
    }
}
