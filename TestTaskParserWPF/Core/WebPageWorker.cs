using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Net;
using System.Text;

namespace TestTaskParserWPF
{
    internal class WebPageWork
    {
        /// <summary>
        /// Main working link processing
        /// </summary>
        /// <param name="DBConnectionString">DataBase connection string</param>
        /// <param name="url">URL to parse</param>
        internal static void WebPageWorker(string DBConnectionString, string url)
        {
            var webPage = GetWebPage(url);
            ParseModels(webPage);
        }

        /// <summary>
        /// Gets webpage from source url using UTF-8 encoding
        /// </summary>
        /// <param name="url">URL link to parse</param>
        /// <returns>Html source code in string</returns>
        private static string GetWebPage(string url)
        {
            Logger.LogMsg($"Getting page: {url}");
            using (WebClient webClient = new WebClient())
            {
                string webPage = "";
                webClient.Encoding = Encoding.UTF8;
                return webPage = webClient.DownloadString(url);
            }
        }

        /// <summary>
        /// Parsing
        /// </summary>
        /// <param name="html">html source</param>
        private static void ParseModels(string html)
        {
            Logger.LogMsg("Started parser...");
            HtmlParser parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(html);
            IHtmlCollection<IElement> models = document.QuerySelectorAll("div.List > div.Header > div.name");
            foreach (var model in models)
            {
                var modelName = model.TextContent;
                Logger.LogMsg($"FOUND MODEL: {modelName}");
                IElement modelParent = model.ParentElement.ParentElement;
                int childrenCount = modelParent.QuerySelector("div.List").ChildElementCount;
                Logger.LogMsg($"CHILDREN FOUND: {childrenCount}");
                IElement[] childrenElements = modelParent.QuerySelectorAll("div.List").Children("div.List").ToArray();
                for (int i = 0; i < childrenCount; i++)
                {
                    string modelId = childrenElements[i].QuerySelector("div.List > div.List > div.id").TextContent;
                    string modelIdHref = childrenElements[i].QuerySelector("div.List > div.List > div.id > a").GetAttribute("href");
                    string modelDateRange = childrenElements[i].QuerySelector("div.List > div.List > div.dateRange").TextContent;
                    string modelCode = childrenElements[i].QuerySelector("div.List > div.List > div.modelCode").TextContent;
                    Logger.LogMsg($"{modelName}\n{modelIdHref}\n{modelId}\n{modelDateRange}\n{modelCode}");
                }
            }
        }

        private static void DBWriter(string DBConnectionString)
        {
            using (SqlConnection sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
            }
            throw new NotImplementedException();
        }
    }
}