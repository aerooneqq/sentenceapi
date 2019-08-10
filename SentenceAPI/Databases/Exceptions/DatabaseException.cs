using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SentenceAPI.Databases.Exceptions
{
    /// <summary>
    /// This exception is thrown when the exception occurs while working with the database
    /// </summary>
    public class DatabaseException : Exception
    {
        public DatabaseException() { }

        public DatabaseException(string message) : base(message) { }

        public DatabaseException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected DatabaseException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
