using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LsysParser.CustomException
{
    class PropertyParserException : ParserException
    {
        public PropertyParserException()
        {
        }

        public PropertyParserException(string message) : base(message)
        {
        }

        public PropertyParserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PropertyParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
