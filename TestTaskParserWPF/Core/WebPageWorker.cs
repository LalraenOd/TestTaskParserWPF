using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Parsing model names
        /// </summary>
        /// <param name="url">Page link to parse model names</param>
        private static void ParseModels(string url)
        {
            var webPageHtml = Misc.GetWebPage(url);
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
                    //writing modeldata to class
                    string modelCode = childrenElements[counter].QuerySelector("div.List > div.List > div.id").TextContent;
                    string modelIdHref = childrenElements[counter].QuerySelector("div.List > div.List > div.id > a").GetAttribute("href");
                    string modelDateRange = childrenElements[counter].QuerySelector("div.List > div.List > div.dateRange").TextContent;
                    string modelPickingCode = childrenElements[counter].QuerySelector("div.List > div.List > div.modelCode").TextContent;
                    ModelData modelData = new ModelData(modelCode, modelName, modelDateRange, modelPickingCode);
                    //send modeldata to db writer
                    DbWriter.WriteModelData(modelData);
                    string[] threadParams = new string[] { "https://www.ilcats.ru" + modelIdHref, modelData.ModelCode };
                    //start parsing of model equipment
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
            string pickingWebPage = Misc.GetWebPage(url);
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
                throw;
            }
            //parsing cell headers
            IElement[] pickingTableHeaders = pickingTable[0].QuerySelectorAll("th").ToArray();
            for (int tableRow = 1; tableRow < pickingTable.Length; tableRow++)
            {
                //parsing table data cells
                IElement[] cellElements = pickingTable[tableRow].QuerySelectorAll("td").ToArray();
                DbWriter.WritePickingData(pickingTableHeaders, cellElements, modelCode);
                var pickingGroupLink = "https://www.ilcats.ru" + cellElements[0].QuerySelector("div.modelCode > a").GetAttribute("href");
                //starting picking groups parser
                ParseSpareGroups(pickingGroupLink, cellElements[0].TextContent);
            }
        }

        /// <summary>
        /// Parisng picking groups
        /// </summary>
        /// <param name="pickingGroupLink">Link to group</param>
        /// <param name="pickingEquipment">picking equipment name</param>
        private static void ParseSpareGroups(string pickingGroupLink, string pickingEquipment)
        {
            List<string> groupNames = new List<string>();
            List<string> groupLinks = new List<string>();
            string pickingGroupPage = Misc.GetWebPage(pickingGroupLink);
            HtmlParser parser = new HtmlParser();
            IHtmlDocument htmlDocument = parser.ParseDocument(pickingGroupPage);
            IHtmlCollection<IElement> elements = htmlDocument.QuerySelectorAll("div.List > div.List > div.name");
            foreach (var element in elements)
            {
                groupNames.Add(element.TextContent);
                groupLinks.Add(element.QuerySelector("a").GetAttribute("href"));
            }
            DbWriter.WriteSparePartGroups(groupNames, pickingEquipment);

            ParseSpareSubGroups(groupNames, groupLinks);
        }

        /// <summary>
        /// Pasing picking subgroups
        /// </summary>
        /// <param name="groupNames">Group names</param>
        /// <param name="groupLinks">Link to these groups</param>
        private static void ParseSpareSubGroups(List<string> groupNames, List<string> groupLinks)
        {
            List<string> subGroupNames = new List<string>();
            List<string> spareLinks = new List<string>();
            //parsing each group
            for (int groupCounter = 0; groupCounter < groupNames.Count; groupCounter++)
            {
                string pickingGroupPage = Misc.GetWebPage("https://www.ilcats.ru/" + groupLinks[groupCounter]);
                HtmlParser parser = new HtmlParser();
                IHtmlDocument htmlDocument = parser.ParseDocument(pickingGroupPage);
                IHtmlCollection<IElement> elements = htmlDocument.QuerySelectorAll("div.Tiles > div.List > div.List > div.name");
                foreach (var element in elements)
                {
                    subGroupNames.Add(element.TextContent);
                    spareLinks.Add("https://www.ilcats.ru/" + element.QuerySelector("a").GetAttribute("href"));
                }
                DbWriter.WriterPickingSubGroups(groupNames[groupCounter], subGroupNames);
                //starting each spare
                ParseSpareData(subGroupNames, spareLinks);
            }
        }

        /// <summary>
        /// Parsing exact picking
        /// </summary>
        /// <param name="subGroupNames"></param>
        /// <param name="subGroupLinks"></param>
        private static void ParseSpareData(List<string> subGroupNames, List<string> pickingLinks)
        {
            for (int pickingCounter = 0; pickingCounter < subGroupNames.Count; pickingCounter++)
            {
                var pickingLink = pickingLinks[pickingCounter];
                //Generating image name
                Regex alldata = new Regex(@"model=(?<model>.+)&modification=(?<modification>.+)&complectation=(?<complectation>.+)&group=(?<group>.+)&subgroup=(?<subgroup>.+)");
                var imageName = $"{alldata.Match(pickingLink).Groups["model"].Value}_" +
                    $"{alldata.Match(pickingLink).Groups["modification"].Value}_" +
                    $"{alldata.Match(pickingLink).Groups["complectation"].Value}_" +
                    $"{alldata.Match(pickingLink).Groups["group"].Value}_" +
                    $"{alldata.Match(pickingLink).Groups["subgroup"].Value}";
                //Creating new picking data
                PickingData pickingData = new PickingData(subGroupNames[pickingCounter], pickingLink, imageName);
                //Parsing
                var webPageHtml = Misc.GetWebPage(pickingLink);
                HtmlParser parser = new HtmlParser();
                IHtmlDocument htmlDocument = parser.ParseDocument(webPageHtml);
                //getting image
                try
                {
                    var imageLink = htmlDocument.QuerySelector("div.ifImage > div.Images > div.ImageArea > div.Image > img").GetAttribute("src");
                    Misc.SaveImage(imageLink, imageName);
                }
                catch (NullReferenceException)
                {
                    imageName = "";
                    Logger.LogMsg("No image or exception.");
                }
                //getting pickings details
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
                DbWriter.WriteSpares(pickingData);
            }
        }
    }
}