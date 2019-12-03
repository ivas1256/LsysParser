using HtmlAgilityPack;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LsysParser.Robot.Helper
{
    class HtmlPropertyParser
    {
        HtmlDocument html;
        string contextUrl;
        Logger logger = LogManager.GetCurrentClassLogger();

        public delegate void LogEventHandler(string message);
        public event LogEventHandler OnMessage;

        /// <param name="html">страница из которой нужно извлечь данные</param>
        /// <param name="contextUrl">Url передаваемого html-ля</param>
        public HtmlPropertyParser(HtmlDocument html, string contextUrl)
        {
            this.html = html;
            this.contextUrl = contextUrl;
        }

        public string GetAsString(HtmlNode source, string xPath, string contextName = "", string mask = null)
        {
            try
            {
                string result = null;
                var node = source.SelectSingleNode(xPath);
                if (node != null)
                    if (mask == null)
                        result = RemoveSpecialSymbols(node.InnerText);
                    else
                        result = RemoveSpecialSymbols(node.InnerText, mask);

                return result;
            }
            catch (Exception ex)
            {
                OnMessage($"Не удалось спарсить свойство {contextName}, на странице {contextUrl}");
                logger.Error(ex, $"Не удалось спарсить свойство {contextName}, на странице {contextUrl}");
                return "";
            }
        }

        public string GetAsString(string xPath, string contextName = "", string mask = null)
        {
            try
            {
                string result = null;
                var node = html.DocumentNode.SelectSingleNode(xPath);
                if (node != null)
                    if (mask == null)
                        result = RemoveSpecialSymbols(node.InnerText);
                    else
                        result = RemoveSpecialSymbols(node.InnerText, mask);

                return result;
            }
            catch (Exception ex)
            {
                OnMessage($"Не удалось спарсить свойство {contextName}, на странице {contextUrl}");
                logger.Error(ex, $"Не удалось спарсить свойство {contextName}, на странице {contextUrl}");
                return "";
            }
        }

        public double GetAsDouble(string xPath, string contextName = "", string mask = null)
        {
            try
            {
                string str = GetAsString(xPath, mask)
                    .Replace(".", ",");

                str = Regex.Replace(str, "\\s", "");

                var str2 = "";
                var match = Regex.Match(str, @"(\d+\.\d+)|(\d+)");
                if (match.Success)
                    str2 = match.Value;
                else
                    return 0;

                return double.Parse(str2);
            }
            catch (Exception ex)
            {
                OnMessage($"Не удалось спарсить свойство {contextName}, на странице {contextUrl}");
                logger.Error(ex, $"Не удалось спарсить свойство {contextName}, на странице {contextUrl}");
                return -1;
            }
        }

        public string RemoveSpecialSymbols(string text)
        {
            text = text.Replace("&nbsp;", " ");
            text = text.Replace("&quot;", "\"");

            return text.Trim();
        }
        /// <summary>
        /// mask - это регулярка. Возвращается значение Groups[groupIndex]
        /// </summary>
        string RemoveSpecialSymbols(string text, string mask, int groupIndex = 1)
        {
            text = RemoveSpecialSymbols(text);
            var regex = Regex.Match(text, mask);
            return regex?.Groups[groupIndex]?.Value;
        }
    }
}
