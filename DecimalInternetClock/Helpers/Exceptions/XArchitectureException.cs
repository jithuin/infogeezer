using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helpers.Exceptions
{
    [Serializable]
    public class XArchitectureException : Exception
    {
        public XArchitectureException()
        {
        }

        public XArchitectureException(string message)
            : base(message)
        {
        }

        public XArchitectureException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected XArchitectureException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}