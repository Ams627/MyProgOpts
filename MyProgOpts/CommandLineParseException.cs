using System;
using System.Runtime.Serialization;

namespace MyProgOpts
{
    [Serializable]
    internal class CommandLineParseException : Exception
    {
        public CommandLineParseException()
        {
        }

        public CommandLineParseException(string message) : base(message)
        {
        }

        public CommandLineParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CommandLineParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}