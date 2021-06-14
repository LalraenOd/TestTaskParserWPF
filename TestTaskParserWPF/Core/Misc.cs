using Microsoft.Data.SqlClient;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace TestTaskParserWPF.Core
{
    /// <summary>
    /// 
    /// </summary>
    internal class Misc
    {
        internal static int totalRequests = 0;

        /// <summary>
        /// Gets webpage from source url using UTF-8 encoding
        /// </summary>
        /// <param name="url">URL link to parse</param>
        /// <returns>Html source code in string</returns>
        internal static string GetWebPage(string url)
        {
            totalRequests++;
            if (CheckWebPageAvailability(url))
            {
                if (ProxyWorker.requestsWithProxy >= 150)
                {
                    ProxyWorker.selectedProxy++;
                    ProxyWorker.requestsWithProxy = 0;
                }
                Logger.LogMsg($"Getting page: {url}");
                if (ProxyWorker.workingProxies[ProxyWorker.selectedProxy] != null)
                {

                }
                using (WebClient webClient = new WebClient())
                {
                    string webPage = "";
                    WebProxy webProxy;
                    try
                    {
                        webProxy = new WebProxy(ProxyWorker.workingProxies[ProxyWorker.selectedProxy]);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Thread.Sleep(5000);
                        return GetWebPage(url);
                    }
                    webClient.Proxy = webProxy;
                    webClient.Encoding = Encoding.UTF8;
                    ProxyWorker.requestsWithProxy++;
                    try
                    {
                        return webPage = webClient.DownloadString(url);
                    }
                    catch (WebException)
                    {
                        ProxyWorker.selectedProxy++;
                        return GetWebPage(url);
                    }
                }
            }
            else
            {
                ProxyWorker.selectedProxy++;
                return GetWebPage(url);
            }
        }

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
                    return true;
                }
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

        /// <summary>
        /// Saves image to images folder
        /// </summary>
        /// <param name="imageLink">Link to the image to be saved</param>
        /// <param name="imageName">Name of the file</param>
        internal static void SaveImage(string imageLink, string imageName)
        {
            var imagePath = "images\\" + imageName + ".jpg";
            if (!Directory.Exists("images"))
            {
                Directory.CreateDirectory("images");
            }
            if (!File.Exists(imagePath))
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile("https:" + imageLink, imagePath);
                }
            }
        }
    }
}