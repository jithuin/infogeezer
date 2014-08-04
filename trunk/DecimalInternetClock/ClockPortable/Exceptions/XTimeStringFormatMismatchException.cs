using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Clocks.Exceptions
{
    [DataContract]
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

        
    }
}