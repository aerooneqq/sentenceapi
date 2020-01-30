using System;
using System.Runtime.Serialization;


namespace SharedLibrary.Events.Exceptions
{
    public class DomainEventException : Exception
    {
        public DomainEventException() {}

        public DomainEventException(string message) : base(message) {}

        public DomainEventException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected DomainEventException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}