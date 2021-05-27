using AngleSharp.Dom;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace TestTaskParserWPF.Core
{
    internal class DbWriter
    {
        public static string dBConnectionString { get; set; } = "";

        /// <summary>
        /// Writing ModelData to DB
        /// </summary>
        /// <param name="modelData"></param>
        internal static void WriteModelData(ModelData modelData)
        {
            using (SqlConnection sqlConnection = new SqlConnection(dBConnectionString))
            {
                sqlConnection.Open();
                try
                {
                    string sqlExpression = $"INSERT INTO [MODELDATA] ([MODELCODE], [MODELNAME], [MODELDATERANGE], [MODELPICKINGCODE]) " +
                        $"VALUES ('{modelData.ModelCode}','{modelData.ModelName}','{modelData.ModelDateRange}','{modelData.ModelPickingCode}')";
                    SqlCommand command = new SqlCommand(sqlExpression, sqlConnection);
                    command.ExecuteNonQuery();
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.ToString().Contains("UNIQUE") == true)
                    {
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogMsg(ex.ToString());
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// Writing picking info to db
        /// </summary>
        /// <param name="headers">table headers collection</param>
        /// <param name="cellElements">cell elements collection</param>
        /// <param name="dBConnectionString">db connection string</param>
        internal static void WritePickingData(IElement[] headers, IElement[] cellElements, string modelCode)
        {
            string sqlExprInsert = "[MODELCODE],";
            string sqlExprValues = $"'{modelCode}',";
            if (headers.Length == cellElements.Length)
            {
                //building sql expression for writing
                for (int counter = 0; counter < headers.Length; counter++)
                {
                    //chaniging first and second column name to english manually
                    //Понимаю, что лучше использовать хранимые, но тут я не нашел решения адекватнее.
                    if (counter == 0)
                        sqlExprInsert += "[EQUIPMENTCODE],";
                    else if (counter == 1)
                        sqlExprInsert += "[DATE],";
                    else if (counter < headers.Length - 1 && counter > 1)
                        sqlExprInsert += $"[{headers[counter].TextContent.Replace('\'', ' ')}],";
                    else if (counter == (headers.Length - 1))
                        sqlExprInsert += $"[{headers[counter].TextContent.Replace('\'', ' ')}]";D

                    if (counter < cellElements.Length - 1)
                        sqlExprValues += $"'{cellElements[counter].TextContent}',";
                    else if (counter == (cellElements.Length - 1))
                        sqlExprValues += $"'{cellElements[counter].TextContent}'";
                }
            }
            string sqlExpression = $"INSERT INTO [MODELEQUIPMENT] ({sqlExprInsert}) VALUES ({sqlExprValues})";
            //writing info to db using expression
            using (SqlConnection sqlConnection = new SqlConnection(dBConnectionString))
            {
                sqlConnection.Open();
                try
                {
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

        /// <summary>
        ///Writing picking groups
        /// </summary>
        /// <param name="groupNames"></param>
        /// <param name="pickingEquipment"></param>
        internal static void WriteSparePartGroups(List<string> groupNames, string pickingEquipment)
        {
            foreach (var groupName in groupNames)
            {
                using (SqlConnection sqlConnection = new SqlConnection(dBConnectionString))
                {
                    sqlConnection.Open();
                    try
                    {
                        string sqlExpressionCheck = $"  SELECT * FROM [SPAREPARTGROUP] WHERE [SPAREPARTGROUPNAME] = '{groupName}'";
                        SqlCommand sqlCommandCheck = new SqlCommand(sqlExpressionCheck, sqlConnection);
                        var groupExists = sqlCommandCheck.ExecuteScalar();
                        if (groupExists == null)
                        {
                            string sqlExpression = $"INSERT INTO [SPAREPARTGROUP] ([EQUIPMENTCODE], [SPAREPARTGROUPNAME]) " +
                                                $"VALUES ('{pickingEquipment}', '{groupName}')";
                            SqlCommand command = new SqlCommand(sqlExpression, sqlConnection);
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogMsg(ex.ToString());
                        throw;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Writing sub group names
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="subGroupNames"></param>
        internal static void WriterPickingSubGroups(string groupName, List<string> subGroupNames)
        {
            foreach (var subGroupName in subGroupNames)
            {
                using (SqlConnection sqlConnection = new SqlConnection(dBConnectionString))
                {
                    sqlConnection.Open();
                    try
                    {
                        string sqlExpression = $"INSERT INTO [SPAREPARTSUBGROUP] ([SPAREPARTSUBGROUPNAME], [SPAREPARTGROUPNAME]) " +
                            $"VALUES ('{subGroupName}', '{groupName}')";
                        SqlCommand command = new SqlCommand(sqlExpression, sqlConnection);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogMsg(ex.ToString());
                        throw;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pickingData"></param>
        internal static void WritePickings(PickingData pickingData)
        {
            using (SqlConnection sqlConnection = new SqlConnection(dBConnectionString))
            {
                sqlConnection.Open();
                try
                {
                    string sqlExpression = $"INSERT INTO SPAREPARTDATA ([SPAREPARTCODE], [SPAREPARTCOUNT], [SPAREPARTINFO], [SPAREPARTTREECODE], [SPAREPARTTREE], [SPAREPARTDATE], [SPAREPARTSUBGROUPLINK], [SPAREPARTIMAGENAME]) " +
                        $"VALUES ('{pickingData.Number}', '{pickingData.Quantity}', '{pickingData.Info}', '{pickingData.TreeCode}', '{pickingData.Tree}', '{pickingData.DateRange}', '{pickingData.SubGroupLink}', '{pickingData.ImageName}')";
                    SqlCommand command = new SqlCommand(sqlExpression, sqlConnection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Logger.LogMsg(ex.ToString());
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
    }
}