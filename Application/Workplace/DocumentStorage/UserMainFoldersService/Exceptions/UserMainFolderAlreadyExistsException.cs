using System;
using System.Runtime.Serialization;

namespace SentenceAPI.Features.Workplace.DocumentsStorage.Exceptions
{
    public class UserMainFolderAlreadyExistsException : Exception
    {
        public UserMainFolderAlreadyExistsException() { }

        public UserMainFolderAlreadyExistsException(string message) : base(message) { }

        public UserMainFolderAlreadyExistsException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected UserMainFolderAlreadyExistsException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}