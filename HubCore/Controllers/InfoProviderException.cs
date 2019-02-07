using System;
using System.Runtime.Serialization;

namespace HubCore.Controllers
{
    [Serializable]
    internal class InfoProviderException : Exception
    {
        public InfoProviderException()
        {
        }

        public InfoProviderException(string message) : base(message)
        {
        }

        public InfoProviderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InfoProviderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}