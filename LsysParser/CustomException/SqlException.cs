﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LsysParser.CustomException
{
    class SqlException : ParserException
    {
        public SqlException()
        {
        }

        public SqlException(string message) : base(message)
        {
        }

        public SqlException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SqlException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
