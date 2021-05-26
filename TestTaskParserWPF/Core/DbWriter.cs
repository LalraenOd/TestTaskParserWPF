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
                    string sqlExpression = $"INSERT INTO ModelData (MODELCODE, MODELNAME, MODELDATERANGE, MODELPICKINGCODE) " +
                        $"VALUES ('{modelData.ModelCode}','{modelData.ModelName}','{modelData.ModelDateRange}','{modelData.ModelPickingCode}')";
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
                    if (counter == 0)
                        sqlExprInsert += "[DATE],";
                    else if (counter == 1)
                        sqlExprInsert += "[EQUIPMENT],";
                    else if (counter < headers.Length - 1 && counter > 1)
                        sqlExprInsert += $"[{headers[counter].TextContent.Replace('\'', ' ')}],";
                    else if (counter == (headers.Length - 1))
                        sqlExprInsert += $"[{headers[counter].TextContent.Replace('\'', ' ')}]";

                    if (counter < cellElements.Length - 1)
                        sqlExprValues += $"'{cellElements[counter].TextContent}',";
                    else if (counter == (cellElements.Length - 1))
                        sqlExprValues += $"'{cellElements[counter].TextContent}'";
                }
            }
            string sqlExpression = $"INSERT INTO ModelPicking ({sqlExprInsert}) VALUES ({sqlExprValues})";
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
        ///
        /// </summary>
        /// <param name="groupNames"></param>
        /// <param name="pickingEquipment"></param>
        internal static void WritePickingGroups(List<string> groupNames, string pickingEquipment)
        {
            foreach (var groupName in groupNames)
            {
                using (SqlConnection sqlConnection = new SqlConnection(dBConnectionString))
                {
                    sqlConnection.Open();
                    try
                    {
                        string sqlExpression = $"INSERT INTO [PickingGroups] ([PICKINGID], [PICKINGGROUPNAME]) " +
                            $"VALUES ('{pickingEquipment}', '{groupName}')";
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
                        string sqlExpression = $"INSERT INTO PickingSubGroups ([PICKINGSUBGROUPNAME], [PICKINGGROUPNAME]) " +
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
                    string sqlExpression = $"INSERT INTO PickingData ([PICKINGCODE], [PICKINGCOUNT], [PICKINGINFO], [PICKINGTREECODE], [PICKINGTREE], [PICKINGDATE], [PICKINGSUBGROUPLINK], [PICKINGIMAGENAME]) " +
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