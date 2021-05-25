using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Net;
using System.Text;
using TestTaskParserWPF.Core;

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
            ParseModels(url);
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
                Logger.LogMsg($"Found {childrenCount} models on page.");
                IElement[] childrenElements = modelParent.QuerySelectorAll("div.List").Children("div.List").ToArray();
                for (int counter = 0; counter < childrenCount; counter++)
                {
                    Logger.LogMsg($"Parsing {counter} model");
                    //Smth to do with models data
                    string modelId = childrenElements[counter].QuerySelector("div.List > div.List > div.id").TextContent;
                    string modelIdHref = childrenElements[counter].QuerySelector("div.List > div.List > div.id > a").GetAttribute("href");
                    string modelDateRange = childrenElements[counter].QuerySelector("div.List > div.List > div.dateRange").TextContent;
                    string modelPickingCode = childrenElements[counter].QuerySelector("div.List > div.List > div.modelCode").TextContent;
                    ModelData modelData = new ModelData(modelId, modelName, modelDateRange, modelPickingCode);
                    Logger.LogMsg($"Writing {counter} model to db...");
                    DBWriterModelData(modelData);
                    PickingParser("https://www.ilcats.ru" + modelIdHref);
                }
            }
        }

        /// <summary>
        /// Parsing each model pickings table
        /// </summary>
        /// <param name="url">Link to pickings webpage</param>
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
                //Smth to do with headers
                //MainWindow.AppWindow.RichTextBoxLog.AppendText(header.TextContent + "\n");
            }
            for (int tableRow = 1; tableRow < pickingTable.Length; tableRow++)
            {
                MainWindow.AppWindow.RichTextBoxLog.AppendText($"Picking {tableRow} \t");
                IElement[] cellElements = pickingTable[tableRow].QuerySelectorAll("td").ToArray();
                DbWriterPickingData(pickingTableHeaders, cellElements);
                //foreach (var cellElement in cellElements)
                //{
                //    //Smth to do with each table cell
                //    MainWindow.AppWindow.RichTextBoxLog.AppendText(cellElement.TextContent + "\n");
                //}
            }
        }

        /// <summary>
        /// Writing ModelData to DB
        /// </summary>
        /// <param name="modelData"></param>
        private static void DBWriterModelData(ModelData modelData)
        {
            string DBConnectionString = MainWindow.AppWindow.TextBoxSQLConnectionString.Text;
            using (SqlConnection sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                try
                {
                    string sqlExpresion = $"INSERT INTO ModelData (modelCode, modelName, modelDateRange, modelPickingCode) " +
                        $"VALUES ('{modelData.ModelCode}','{modelData.ModelName}','{modelData.ModelDateRange}','{modelData.ModelPickingCode}')";
                    Logger.LogMsg($"\nsqlexp :{sqlExpresion}\n");
                    SqlCommand command = new SqlCommand(sqlExpresion, sqlConnection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Logger.LogMsg(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void DbWriterPickingData(IElement[] headers, IElement[] cellElements)
        {
            string sqlExprInsert = "";
            string sqlExprValues = "";
            if (headers.Length == cellElements.Length)
            {
                for (int counter = 0; counter < headers.Length; counter++)
                {
                    switch (counter)
                    {
                        case 0:
                            sqlExprInsert += "'DATE',";
                            continue;
                        case 1:
                            sqlExprInsert += "'EQUIPMENT',";
                            continue;
                        default:
                            sqlExprInsert += $"'{headers[counter].TextContent}',";
                            break;
                    }
                    sqlExprValues += $"'{cellElements[counter].TextContent}',";
                }
            }
            string sqlExpression = $"INSERT INTO ModelPicking ({sqlExprInsert}), VALUES ({sqlExprValues})";
            string DBConnectionString = MainWindow.AppWindow.TextBoxSQLConnectionString.Text;
            using (SqlConnection sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                try
                {
                    Logger.LogMsg($"\nsqlexp :{sqlExpression}\n");
                    SqlCommand command = new SqlCommand(sqlExpression, sqlConnection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Logger.LogMsg(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
    }
}