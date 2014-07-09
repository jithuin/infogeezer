using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clocks.Exceptions
{
    [Serializable]
    public class TimeStringFormatMismatchException : Exception
    {
        public TimeStringFormatMismatchException()
        {
        }

        public TimeStringFormatMismatchException(string message)
            : base(message)
        {
        }

        public TimeStringFormatMismatchException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected TimeStringFormatMismatchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}