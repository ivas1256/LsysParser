using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LsysParser.Data.Model
{
    enum ErrorSource { CatalogPage = 1, ProductPage, ProductLinksParser, ProductParser }

    class ErrorRuntimeObj
    {
        public string Url { get; set; }
        public Nullable<int> Code { get; set; }
        public string Message { get; set; }
        public int ErrorSourceId { get; set; }

        public ErrorRuntimeObj(string url, WebException ex, ErrorSource errorSource)
        {
            Url = url;
            ErrorSourceId = (int)errorSource;

            if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                Code = 404;
            else
            {
                Code = (int)((HttpWebResponse)ex.Response).StatusCode;
                Message = ex.Message;
            }
        }

        public ErrorRuntimeObj(string url, Exception ex, ErrorSource errorSource, string message)
        {
            Url = url;
            ErrorSourceId = (int)errorSource;
            Message = $"{message}; {ex.Message}";
        }
    }
}
