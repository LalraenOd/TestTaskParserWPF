using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Data.SqlClient;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestTaskParserWPF.Core
{
    internal class Misc
    {
        /// <summary>
        /// Checks if programm can access web page
        /// </summary>
        /// <param name="url">URL link to check</param>
        /// <returns>Boolean showing site availability</returns>
        internal static bool CheckWebPageAvailability(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 5000;
                request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
                request.Method = "HEAD";
                using (var response = request.GetResponse())
                {
                    Logger.LogMsg("Site is available.");
                }
                return true;
            }
            catch (ThreadAbortException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogMsg("Site is not avilable. Exception logged.\n" + ex.ToString());
                return false;
            }
        }

        internal static bool CheckSiteBlock(string url)
        {
            //TODO Parse header to understand is site available
            string webPage = "";
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                webPage = webClient.DownloadString(url);
            }
            HtmlParser parser = new HtmlParser();
            IHtmlDocument htmlDocument = parser.ParseDocument(webPage);
            var header = "";
            try
            {
                header = htmlDocument.QuerySelector("div.Body > h1").TextContent;
            }
            catch (NullReferenceException)
            {
                return true;
            }
            if (header == "")
            {
                Logger.LogMsg("HELP. SITE BLOCKED US.");
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Checks if DB is available
        /// </summary>
        /// <param name="dbConnectionString">Connection string</param>
        /// <returns>Boolean showing db availability</returns>
        internal static bool CheckDbConnection(string dbConnectionString)
        {
            using (SqlConnection sqlConnection = new SqlConnection(dbConnectionString))
            {
                try
                {
                    sqlConnection.Open();
                    Logger.LogMsg($"DB connection opened.\n" +
                        $"Connection properties:\n" +
                        $"Connection string: {sqlConnection.ConnectionString}\n" +
                        $"DB: {sqlConnection.Database}\n" +
                        $"Server: { sqlConnection.DataSource}\n" +
                        $"State: {sqlConnection.State}");
                    sqlConnection.Close();
                    return true;
                }
                catch (SqlException ex)
                {
                    Logger.LogMsg("Exception handled: " + ex.ToString());
                    return false;
                }
            }
        }

        internal static void ThreadCounter()
        {
            while (true)
            {
                try
                {
                    MainWindow.AppWindow.Dispatcher.Invoke((Action)(() =>
                    {
                        MainWindow.AppWindow.ThreadCounter.Text = "Current threads: " + Process.GetCurrentProcess().Threads.Count.ToString();
                    }));
                }
                catch (ThreadAbortException)
                {
                    continue;
                }
                catch (TaskCanceledException)
                {
                    continue;
                }
                catch (Exception ex)
                {
                    throw new NotImplementedException();
                }
                Thread.Sleep(1000);
            }
        }

        internal static void SaveImage(string imageLink, string imageName)
        {
            var imagePath = "images\\" + imageName + ".jpg";
            if (!Directory.Exists("images"))
            {
                Directory.CreateDirectory("images");
            }
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(imageLink, imagePath);
            }
        }
    }
}