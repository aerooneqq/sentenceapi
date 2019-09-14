using SentenceAPI.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Events
{
    class FileCreationEvent : IDomainEvent
    {
        #region Event properties
        private readonly string folderName;
        private readonly long userID;
        private readonly long folderID;
        #endregion

        #region Services
        #endregion

        public FileCreationEvent(string folderName, long userID, long folderID)
        {
            this.folderName = folderName;
            this.userID = userID;
            this.folderID = folderID;
        }

        public Task Handle()
        {
            throw new NotImplementedException();
        }
    }
}
