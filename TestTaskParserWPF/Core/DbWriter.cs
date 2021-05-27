using AngleSharp.Dom;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace TestTaskParserWPF.Core
{
    internal class DbWriter
    {
        public static string DBConnectionString { get; set; } = "";

        /// <summary>
        /// Writing ModelData to DB
        /// </summary>
        /// <param name="modelData"></param>
        internal static void WriteModelData(ModelData modelData)
        {
            using (SqlConnection sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                try
                {
                    SqlCommand command = new SqlCommand("ModelDataAdd", sqlConnection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@modelCode", modelData.ModelCode));
                    command.Parameters.Add(new SqlParameter("@modelName", modelData.ModelName));
                    command.Parameters.Add(new SqlParameter("@modelDateRange", modelData.ModelDateRange));
                    command.Parameters.Add(new SqlParameter("@modelPickingCode", modelData.ModelPickingCode));
                    command.ExecuteNonQuery();
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.ToString().Contains("UNIQUE") == true)
                    {
                    }
                    else
                    {
                        throw;
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
                        sqlExprInsert += $"[{headers[counter].TextContent.Replace('\'', ' ')}]";

                    if (counter < cellElements.Length - 1)
                        sqlExprValues += $"'{cellElements[counter].TextContent}',";
                    else if (counter == (cellElements.Length - 1))
                        sqlExprValues += $"'{cellElements[counter].TextContent}'";
                }
            }
            string sqlExpression = $"INSERT INTO [MODELEQUIPMENT] ({sqlExprInsert}) VALUES ({sqlExprValues})";
            //writing info to db using expression
            using (SqlConnection sqlConnection = new SqlConnection(DBConnectionString))
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
                using (SqlConnection sqlConnection = new SqlConnection(DBConnectionString))
                {
                    sqlConnection.Open();
                    try
                    {
                        SqlCommand command = new SqlCommand("SparePartGroupAdd", sqlConnection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@equipmentCode", pickingEquipment));
                        command.Parameters.Add(new SqlParameter("@sparePartGroupName", groupName));
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
                using (SqlConnection sqlConnection = new SqlConnection(DBConnectionString))
                {
                    sqlConnection.Open();
                    try
                    {
                        SqlCommand command = new SqlCommand("SparePartSubGroupAdd", sqlConnection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@sparePartSubGroup", subGroupName));
                        command.Parameters.Add(new SqlParameter("@sparePartGroup", groupName));
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
            using (SqlConnection sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                try
                {
                    SqlCommand command = new SqlCommand("SparePartDataAdd", sqlConnection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@sparePartCode", pickingData.Number));
                    command.Parameters.Add(new SqlParameter("@sparePartCount", pickingData.Quantity));
                    command.Parameters.Add(new SqlParameter("@sparePartInfo", pickingData.Info));
                    command.Parameters.Add(new SqlParameter("@sparePartTreeCode", pickingData.TreeCode));
                    command.Parameters.Add(new SqlParameter("@sparePartTree", pickingData.Tree));
                    command.Parameters.Add(new SqlParameter("@sparePartDate", pickingData.DateRange));
                    command.Parameters.Add(new SqlParameter("@sparePartSubGroupName", pickingData.SubGropName));
                    command.Parameters.Add(new SqlParameter("@sparePartSubGroupLink", pickingData.SubGroupLink));
                    command.Parameters.Add(new SqlParameter("@sparePartImageName", pickingData.ImageName));
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