using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using TestTaskParserWPF.Core;

namespace TestTaskParserWPF
{
    internal class WebPageWork
    {
        /// <summary>
        /// Main link processing
        /// </summary>
        /// <param name="DBConnectionString">DataBase connection string</param>
        internal static void WebPageWorker()
        {
            string url = "";
            MainWindow.AppWindow.Dispatcher.Invoke((Action)(() =>
            {
                url = MainWindow.AppWindow.TextBoxLink.Text;
            }));
            ParseModels(url);
        }

        /// <summary>
        /// Gets webpage from source url using UTF-8 encoding
        /// </summary>
        /// <param name="url">URL link to parse</param>
        /// <returns>Html source code in string</returns>
        private static string GetWebPage(string url)
        {
            if (Misc.CheckWebPageAvailability(url))
            {
                Logger.LogMsg($"Getting page: {url}");
                using (WebClient webClient = new WebClient())
                {
                    string webPage = "";
                    webClient.Encoding = Encoding.UTF8;
                    return webPage = webClient.DownloadString(url);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Parsing model names
        /// </summary>
        /// <param name="url">Page link to parse model names</param>
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
                //Going back to model <div List Class>
                IElement modelParent = model.ParentElement.ParentElement;
                //Cheking number of model codes
                int childrenCount = modelParent.QuerySelector("div.List").ChildElementCount;
                //Parsing child elements (model codes)
                IElement[] childrenElements = modelParent.QuerySelectorAll("div.List").Children("div.List").ToArray();
                for (int counter = 0; counter < childrenCount; counter++)
                {
                    //Smth to do with models data
                    string modelCode = childrenElements[counter].QuerySelector("div.List > div.List > div.id").TextContent;
                    string modelIdHref = childrenElements[counter].QuerySelector("div.List > div.List > div.id > a").GetAttribute("href");
                    string modelDateRange = childrenElements[counter].QuerySelector("div.List > div.List > div.dateRange").TextContent;
                    string modelPickingCode = childrenElements[counter].QuerySelector("div.List > div.List > div.modelCode").TextContent;
                    ModelData modelData = new ModelData(modelCode, modelName, modelDateRange, modelPickingCode);
                    DbWriter.DbWriterModelData(modelData);
                    ParseEquipment("https://www.ilcats.ru" + modelIdHref, modelData.ModelCode);
                }
            }
        }

        /// <summary>
        /// Parsing each model pickings table
        /// </summary>
        /// <param name="url">Link to pickings webpage</param>
        private static void ParseEquipment(string url, string modelCode)
        {
            //Parsing car pickings (tables)
            string pickingWebPage = GetWebPage(url);
            HtmlParser parser = new HtmlParser();
            IHtmlDocument htmlDocument = parser.ParseDocument(pickingWebPage);
            IElement firstTable = htmlDocument.QuerySelector("tbody");
            IHtmlCollection<IElement> pickingTable;
            try
            {
                pickingTable = firstTable.QuerySelectorAll("tbody > tr");
            }
            catch (NullReferenceException)
            {
                if (Misc.CheckWebPageAvailability(url))
                {
                    pickingTable = firstTable.QuerySelectorAll("tbody > tr");
                }
                else
                {
                    throw;
                }
            }
            IElement[] pickingTableHeaders = pickingTable[0].QuerySelectorAll("th").ToArray();
            //foreach (var header in pickingTableHeaders)
            //{
            //    Smth to do with headers
            //    MainWindow.AppWindow.RichTextBoxLog.AppendText(header.TextContent + "\n");
            //}
            for (int tableRow = 1; tableRow < pickingTable.Length; tableRow++)
            {
                IElement[] cellElements = pickingTable[tableRow].QuerySelectorAll("td").ToArray();
                DbWriter.WritePickingData(pickingTableHeaders, cellElements, modelCode);
                var pickingGroupLink = "https://www.ilcats.ru" + cellElements[0].QuerySelector("div.modelCode > a").GetAttribute("href");
                ParsePickingGroups(pickingGroupLink, cellElements[0].TextContent);
                //foreach (var cellElement in cellElements)
                //{
                //    //Smth to do with each table cell
                //    MainWindow.AppWindow.RichTextBoxLog.AppendText(cellElement.TextContent + "\n");
                //}
            }
        }

        /// <summary>
        /// Parisng picking groups
        /// </summary>
        /// <param name="pickingGroupLink">Link to group</param>
        /// <param name="pickingEquipment">picking equipment name</param>
        private static void ParsePickingGroups(string pickingGroupLink, string pickingEquipment)
        {
            List<string> groupNames = new List<string>();
            List<string> groupLinks = new List<string>();
            string pickingGroupPage = GetWebPage(pickingGroupLink);
            HtmlParser parser = new HtmlParser();
            IHtmlDocument htmlDocument = parser.ParseDocument(pickingGroupPage);
            IHtmlCollection<IElement> elements = htmlDocument.QuerySelectorAll("div.List > div.List > div.name");
            foreach (var element in elements)
            {
                groupNames.Add(element.TextContent);
                groupLinks.Add(element.QuerySelector("a").GetAttribute("href"));
            }
            DbWriter.WritePickingGroups(groupNames, pickingEquipment);
            ParsePickingSubGroups(groupNames, groupLinks);
        }

        /// <summary>
        /// Pasing picking subgroups
        /// </summary>
        /// <param name="groupNames">Group names</param>
        /// <param name="groupLinks">Link to these groups</param>
        private static void ParsePickingSubGroups(List<string> groupNames, List<string> groupLinks)
        {
            List<string> subGroupNames = new List<string>();
            List<string> pickingLinks = new List<string>();
            for (int groupCounter = 0; groupCounter < groupNames.Count; groupCounter++)
            {
                string pickingGroupPage = GetWebPage("https://www.ilcats.ru/" + groupLinks[groupCounter]);
                HtmlParser parser = new HtmlParser();
                IHtmlDocument htmlDocument = parser.ParseDocument(pickingGroupPage);
                IHtmlCollection<IElement> elements = htmlDocument.QuerySelectorAll("div.Tiles > div.List > div.List > div.name");
                foreach (var element in elements)
                {
                    subGroupNames.Add(element.TextContent);
                    pickingLinks.Add("https://www.ilcats.ru/" + element.QuerySelector("a").GetAttribute("href"));
                }
                DbWriter.WriterPickingSubGroups(groupNames[groupCounter], subGroupNames);
                ParsePicking(subGroupNames, pickingLinks);
            }
        }

        /// <summary>
        /// Parsing exact picking
        /// </summary>
        /// <param name="subGroupNames"></param>
        /// <param name="subGroupLinks"></param>
        private static void ParsePicking(List<string> subGroupNames, List<string> pickingLinks)
        {
            for (int pickingCounter = 0; pickingCounter < subGroupNames.Count; pickingCounter++)
            {
                /*
                 * https://www.ilcats.ru/toyota/?function=getParts&market=EU&model=671440&modification=LN51L-KRA&complectation=001&group=1&subgroup=0901
                 */
                var pickingLink = pickingLinks[pickingCounter];
                //Generating image name
                Regex regexModel = new Regex(@"model=(?<result>.+)&modification=");
                Regex regexModification = new Regex(@"modification=(?<result>.+)&complectation=");
                var modelName = regexModel.Match(pickingLink).Groups["result"].Value;
                var modification = regexModification.Match(pickingLink).Groups["result"].Value;
                var imageName = $"{modelName}_{modification}";
                //Crating new picking data
                PickingData pickingData = new PickingData(pickingLink, imageName);
                //Parsing
                var webPageHtml = GetWebPage(pickingLink);
                HtmlParser parser = new HtmlParser();
                IHtmlDocument htmlDocument = parser.ParseDocument(webPageHtml);
                IElement table = htmlDocument.QuerySelector("div.Info > table > tbody");
                IHtmlCollection<IElement> pickings = table.QuerySelectorAll("tr");
                for (int i = 1; i < pickings.Length; i++)
                {
                    var currentPicking = pickings[i];
                    //check if tr element is table header or has data
                    if (currentPicking.QuerySelector("th") != null)
                    {
                        pickingData.Tree = currentPicking.TextContent;
                        pickingData.TreeCode = currentPicking.GetAttribute("data-id");
                    }
                    else
                    {
                        pickingData.Number = currentPicking.QuerySelector("td > div.number").TextContent;
                        pickingData.Quantity = Int32.Parse(currentPicking.QuerySelector("td > div.count").TextContent.Replace('X', '0'));
                        pickingData.DateRange = currentPicking.QuerySelector("td > div.dateRange").TextContent;
                        pickingData.Info = currentPicking.QuerySelector("td > div.usage").TextContent;
                    }
                }
                DbWriter.WritePickings(pickingData);
            }
        }
    }
}