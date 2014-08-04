using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Helpers.Exceptions
{
    [DataContract]
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

    }
}