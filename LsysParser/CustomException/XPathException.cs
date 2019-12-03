using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LsysParser.CustomException
{
    class XPathException : ParserException
    {
        public XPathException()
        {
        }

        public XPathException(string message) : base(message)
        {
        }

        public XPathException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected XPathException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
