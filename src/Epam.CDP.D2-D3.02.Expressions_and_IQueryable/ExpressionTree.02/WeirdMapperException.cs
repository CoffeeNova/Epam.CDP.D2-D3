using System;

namespace ExpressionTree._02
{
    [Serializable]
    public class WeirdMapperException : Exception
    {
        public WeirdMapperException() { }
        public WeirdMapperException(string message) : base(message) { }
        public WeirdMapperException(string message, Exception inner) : base(message, inner) { }
        protected WeirdMapperException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
