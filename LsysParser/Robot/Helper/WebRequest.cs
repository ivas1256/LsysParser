using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace LsysParser.Robot.Helper
{
    class HttpHelper
    {
        /// <summary>
        /// Таймаут ожиданий ответа в секундах
        /// </summary>
        public int Timeout { get; set; } = 60;

        public string Accept { get; set; }
        public string UserAgent { get; set; }
        public NameValueCollection CustomHeaders { get; set; }

        public int RequestDelay { get; set; }
        DateTime lastRequested;
        public Exception LastError { get; private set; }

        public delegate void LogMessageEventHandler(Exception ex, string message);
        public event LogMessageEventHandler Log;

        public HttpHelper()
        {
            UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:54.0) Gecko/20100101 Firefox/54.0";
            CustomHeaders = new NameValueCollection();
            CustomHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
        }

        #region get request
        HttpWebResponse Get(Uri url)
        {
            WaitDelay();

            if (string.IsNullOrWhiteSpace(url?.ToString()))
                throw new ArgumentException("Ссылка на страницу пуста");

            var request = (HttpWebRequest)System.Net.WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = Timeout * 1000;

            if (!string.IsNullOrEmpty(Accept))
                request.Accept = Accept;
            if (!string.IsNullOrEmpty(UserAgent))
                request.UserAgent = UserAgent;
            if (CustomHeaders.Count != 0)
                foreach (var header in CustomHeaders.AllKeys)
                    request.Headers.Add(header, CustomHeaders[header]);

            lastRequested = DateTime.Now;
            return (HttpWebResponse)request.GetResponse();
        }

        string GetContent(Uri url)
        {
            using (HttpWebResponse response = Get(url))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                        return reader.ReadToEnd();
                else
                    throw new WebException($"Сервер ответил с кодом: {response.StatusCode}({response.StatusDescription})", null,
                        WebExceptionStatus.ReceiveFailure, response);
            }
        }

        public string GetContent(string url)
        {
            try
            {
                var resp = GetContent(new Uri(url));
                if (string.IsNullOrEmpty(resp))
                {
                    LastError = new WebException("Пустой ответ");
                    Log(LastError, $"Страница: {url}");
                    return null;
                }

                return resp;
            }
            catch (Exception ex)
            {
                LastError = ex;
                Log(ex, $"Не удалось загрузить страницу: {url}");
                return null;
            }
        }

        public HtmlDocument GetHtml(string url)
        {
            try
            {
                var html = new HtmlDocument();
                string resp = GetContent(new Uri(url));
                if (string.IsNullOrEmpty(resp))
                {
                    LastError = new Exception("Пустой ответ");
                    Log(LastError, $"Страница: {url}");
                    return null;
                }
                html.LoadHtml(resp);

                return html;
            }
            catch (Exception ex)
            {
                LastError = ex;
                Log(ex, $"Не удалось загрузить страницу: {url}");
                return null;
            }
        }
        #endregion

        #region post request
        public HttpWebResponse Post(Uri url, string postData, string contentType)
        {
            try
            {
                var now = DateTime.Now;
                var check = now - lastRequested;
                if (check.Seconds < RequestDelay)
                    Thread.Sleep(1000 - check.Milliseconds);

                if (string.IsNullOrWhiteSpace(url?.ToString()))
                    throw new ArgumentException("Url string empty");

                var request = (HttpWebRequest)System.Net.WebRequest.Create(url);
                request.Method = "POST";

                if (!string.IsNullOrEmpty(Accept))
                    request.Accept = Accept;
                if (!string.IsNullOrEmpty(UserAgent))
                    request.UserAgent = UserAgent;
                if (CustomHeaders.Count != 0)
                    foreach (var header in CustomHeaders.AllKeys)
                        request.Headers.Add(header, CustomHeaders[header]);

                var data = Encoding.Default.GetBytes(postData.Replace("?", ""));

                request.ContentType = contentType;
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                lastRequested = DateTime.Now;
                var response = (HttpWebResponse)request.GetResponse();

                return response;
            }
            catch (Exception ex)
            {
                LastError = ex;
                return null;
            }
        }
        public string PostContent(Uri url, string postData, string contentType)
        {
            HttpWebResponse response = Post(url, postData, contentType);
            try
            {
                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    var stream = response.GetResponseStream();
                    using (var reader = new StreamReader(stream))
                    {
                        var strResp = reader.ReadToEnd();

                        return strResp;
                    }
                }
            }
            catch (Exception ex)
            {
                LastError = ex;
            }
            return null;
        }
        public HtmlDocument PostHtml(Uri url, string postData, string contentType)
        {
            string content = PostContent(url, postData, contentType);
            if (string.IsNullOrEmpty(content))
                return null;

            try
            {
                var html = new HtmlDocument();
                html.LoadHtml(content);
            }
            catch (Exception ex)
            {
                LastError = ex;
            }
            return null;
        }
        #endregion

        private void WaitDelay()
        {
            var now = DateTime.Now;
            var check = now - lastRequested;
            if (check.Seconds < RequestDelay)
                Thread.Sleep((RequestDelay - check.Seconds) * 1000);
        }

        public string BuildQueryString(NameValueCollection _params)
        {
            var array = (from key in _params.AllKeys
                         from value in _params.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                         .ToArray();
            return "?" + string.Join("&", array);
        }
    }
}
