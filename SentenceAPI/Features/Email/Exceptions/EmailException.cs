using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Email.Exceptions
{
    public class EmailException : Exception
    {
        public EmailException() { }

        public EmailException(string message) : base(message) { }

        public EmailException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected EmailException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
