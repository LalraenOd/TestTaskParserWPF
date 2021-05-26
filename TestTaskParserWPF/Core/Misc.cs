using Microsoft.Data.SqlClient;
using System;
using System.Net;

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
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Timeout = 3000;
                request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
                request.Method = "HEAD";
                using (var response = request.GetResponse())
                {
                    Logger.LogMsg("Site is available.");
                }
                return true;
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
    }
}