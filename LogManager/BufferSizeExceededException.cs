using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LogManager
{
    [Serializable]
    class BufferSizeExceededException : Exception
    {
        public BufferSizeExceededException() : base()
        {
        }

        public BufferSizeExceededException(string message) : base(message)
        {
        }

        public BufferSizeExceededException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BufferSizeExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
