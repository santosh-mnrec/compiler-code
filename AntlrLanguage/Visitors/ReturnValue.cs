using System;
using System.Runtime.Serialization;

namespace TinyLanguage
{
    [Serializable]
    internal class ReturnValue : Exception
    {
        public Value Value { get; set; }
        public ReturnValue()
        {
        }

        public ReturnValue(string message) : base(message)
        {
        }

        public ReturnValue(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ReturnValue(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}