using System;
using System.Runtime.Serialization;

namespace HubCore.Infrastructure
{
    [Serializable]
    public class HubOperationException : Exception
    {
        public HubOperationException()
        {
        }

        public HubOperationException(string message) : base(message)
        {
        }

        public HubOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HubOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}