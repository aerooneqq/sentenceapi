using System;
using System.Runtime.Serialization;

namespace Application.Requests.Exceptions
{
    public class RequestException : Exception
    {
        public RequestException() { }

        protected RequestException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }

        public RequestException(string message) : base(message) { }

        public RequestException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}