using System;

namespace FileFormatterService.Exceptions
{
    [Serializable]
    public class BuildFileException : Exception
    {
        public BuildFileException() { }
        public BuildFileException(string message) : base(message) { }
        public BuildFileException(string message, Exception inner) : base(message, inner) { }
        protected BuildFileException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
