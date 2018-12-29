using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LogManager
{
    class TraceStateException : Exception
    {
        public TraceStateException() : base()
        {
        }

        public TraceStateException(string message) : base(message)
        {
        }

        public TraceStateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TraceStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
