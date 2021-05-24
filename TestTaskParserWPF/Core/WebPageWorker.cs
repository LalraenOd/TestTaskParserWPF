using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Data.SqlClient;
using System;
using System.Net;
using System.Text;
using System.Linq;

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
            var models = document.QuerySelectorAll("div.List")
                ?.Children("div.Header")
                ?.Children("div.name");
            foreach (var model in models)
            {
                var modelName = model.TextContent;
                var modelsData = document.QuerySelectorAll("div.List")
                    ?.Children("div.Header")
                    ?.Children("div.name")
                    ?.Where(div => div.TextContent == modelName)
                    ?.Parent("div.Header")
                    ?.Parent("div.List")
                    ?.Children("div.List")
                    ;
                Logger.LogMsg(modelName);
                foreach (var modelData in modelsData)
                {
                    Logger.LogMsg(modelData.TextContent);
                }
                Logger.LogMsg("PART FINISHED");
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
