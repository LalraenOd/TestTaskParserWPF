using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
            //ParseModels(url);
            PickingParser(url);
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
        /// <param name="webPageHtml">html source</param>
        private static void ParseModels(string url)
        {
            var webPageHtml = GetWebPage(url);
            Logger.LogMsg("Started parser...");
            HtmlParser parser = new HtmlParser();
            IHtmlDocument htmlDocument = parser.ParseDocument(webPageHtml);
            //Searching for model name
            IHtmlCollection<IElement> models = htmlDocument.QuerySelectorAll("div.List > div.Header > div.name");
            foreach (var model in models)
            {
                string modelName = model.TextContent;
                Logger.LogMsg($"FOUND MODEL: {modelName}");
                //Going back to model <div List Class>
                IElement modelParent = model.ParentElement.ParentElement;
                //Cheking number of model codes
                int childrenCount = modelParent.QuerySelector("div.List").ChildElementCount;
                Logger.LogMsg($"CHILDREN FOUND: {childrenCount}");
                //Parsing child elements (model codes)
                IElement[] childrenElements = modelParent.QuerySelectorAll("div.List").Children("div.List").ToArray();
                for (int i = 0; i < childrenCount; i++)
                {
                    string modelId = childrenElements[i].QuerySelector("div.List > div.List > div.id").TextContent;
                    string modelIdHref = childrenElements[i].QuerySelector("div.List > div.List > div.id > a").GetAttribute("href");
                    string modelDateRange = childrenElements[i].QuerySelector("div.List > div.List > div.dateRange").TextContent;
                    string modelPicking = childrenElements[i].QuerySelector("div.List > div.List > div.modelCode").TextContent;
                    Logger.LogMsg($"{modelName} {modelId} {modelDateRange} {modelPicking}");
                    PickingParser("https://www.ilcats.ru" + modelIdHref);
                }
            }
        }

        private static void PickingParser(string url)
        {                    
            //Parsing car pickings (tables)
            Logger.LogMsg($"Parsing car pickings by link: {url}");
            string pickingWebPage = GetWebPage(url);            
            HtmlParser parser = new HtmlParser();
            IHtmlDocument htmlDocument = parser.ParseDocument(pickingWebPage);
            IElement firstTable = htmlDocument.QuerySelector("tbody");            
            IHtmlCollection<IElement> pickingTable = firstTable.QuerySelectorAll("tbody > tr");
            IElement[] pickingTableHeaders = pickingTable[0].QuerySelectorAll("th").ToArray();
            MainWindow.AppWindow.RichTextBoxLog.AppendText("\n");
            foreach (var header in pickingTableHeaders)
            {
                MainWindow.AppWindow.RichTextBoxLog.AppendText(header.TextContent + "\n");
            }
            for (int tableRow = 1; tableRow < pickingTable.Length; tableRow++)
            {
                MainWindow.AppWindow.RichTextBoxLog.AppendText($"Picking {tableRow} \t");
                IElement[] cellElements = pickingTable[tableRow].QuerySelectorAll("td").ToArray();
                foreach (var cellElement in cellElements)
                {
                    MainWindow.AppWindow.RichTextBoxLog.AppendText(cellElement.TextContent + "\n");
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