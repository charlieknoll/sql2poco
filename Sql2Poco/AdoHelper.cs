using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Sql2Poco;


namespace Sql2Poco
{

    public static class AdoHelper
    {
        public static List<ResultFieldDetails> SetFieldDetailValues(List<ResultFieldDetails> fieldDetails, string connectionString, string sql) {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read()) {
                            for (int i = 0; i < fieldDetails.Count; i++)
                            {
                                fieldDetails[i].FieldValue = reader.GetValue(i);
                            }
    
                        }
                    }
                }
            }  // the connection will be closed & disposed here
            return fieldDetails;
        }
        public static List<List<ResultFieldDetails>> GetResults(string ConnectionString, string Query) {
            var result = new List<List<ResultFieldDetails>>();
            var SchemaTables = GetQuerySchema(ConnectionString, Query);
            foreach (var s in SchemaTables) {
                result.Add(GetFields(s));
            }
            return result;
        }
        public static string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }
        private static List<ResultFieldDetails> GetFields(DataTable SchemaTable)
        {
            //DataTable dt = new DataTable();
             

            List<ResultFieldDetails> result = new List<ResultFieldDetails>();
            if (SchemaTable == null)
                return result;

            for (int i = 0; i <= SchemaTable.Rows.Count - 1; i++)
            {
                var qf = new ResultFieldDetails();
                string properties = string.Empty;
                for (int j = 0; j <= SchemaTable.Columns.Count - 1; j++)
                {
                    properties += SchemaTable.Columns[j].ColumnName + (char)254 + SchemaTable.Rows[i].ItemArray[j].ToString();
                    if (j < SchemaTable.Columns.Count - 1)
                        properties += (char)255;

                    if (SchemaTable.Rows[i].ItemArray[j] != DBNull.Value)
                    {
                        switch (SchemaTable.Columns[j].ColumnName)
                        {
                            case "ColumnName":
                                // sby : ColumnName might be null, in which case it will be created from ordinal.
                                if (!string.IsNullOrEmpty(SchemaTable.Rows[i].Field<string>(j)))
                                    qf.ColumnName = FirstLetterToUpper(SchemaTable.Rows[i].Field<string>(j));
                                break;
                            case "ColumnOrdinal":
                                qf.ColumnOrdinal = (int)SchemaTable.Rows[i].Field<int>(j);
                                if (string.IsNullOrEmpty(qf.ColumnName))
                                    qf.ColumnName = "col" + qf.ColumnOrdinal.ToString();
                                break;
                            case "ColumnSize":
                                qf.ColumnSize = (int)SchemaTable.Rows[i].Field<int>(j);
                                break;
                            case "NumericPrecision":
                                qf.NumericPrecision = (int)SchemaTable.Rows[i].Field<short>(j);
                                break;
                            case "NumericScale":
                                qf.NumericScale = SchemaTable.Rows[i].Field<short>(j);
                                break;
                            case "IsUnique":
                                qf.IsUnique = SchemaTable.Rows[i].Field<bool>(j);
                                break;
                            case "BaseColumnName":
                                qf.BaseColumnName = SchemaTable.Rows[i].Field<string>(j);
                                break;
                            case "BaseTableName":
                                qf.BaseTableName = SchemaTable.Rows[i].Field<string>(j);
                                break;
                            case "DataType":
                                qf.DataType = SchemaTable.Rows[i].Field<System.Type>(j).FullName;
                                break;
                            case "AllowDBNull":
                                qf.AllowDBNull = SchemaTable.Rows[i].Field<bool>(j);
                                break;
                            case "ProviderType":
                                qf.ProviderType = SchemaTable.Rows[i].Field<int>(j);
                                break;
                            case "IsIdentity":
                                qf.IsIdentity = SchemaTable.Rows[i].Field<bool>(j);
                                break;
                            case "IsAutoIncrement":
                                qf.IsAutoIncrement = SchemaTable.Rows[i].Field<bool>(j);
                                break;
                            case "IsRowVersion":
                                qf.IsRowVersion = SchemaTable.Rows[i].Field<bool>(j);
                                break;
                            case "IsLong":
                                qf.IsLong = SchemaTable.Rows[i].Field<bool>(j);
                                break;
                            case "IsReadOnly":
                                qf.IsReadOnly = SchemaTable.Rows[i].Field<bool>(j);
                                break;
                            case "ProviderSpecificDataType":
                                qf.ProviderSpecificDataType = SchemaTable.Rows[i].Field<System.Type>(j).FullName;
                                break;
                            case "DataTypeName":
                                qf.DataTypeName = SchemaTable.Rows[i].Field<string>(j);
                                break;
                            case "UdtAssemblyQualifiedName":
                                qf.UdtAssemblyQualifiedName = SchemaTable.Rows[i].Field<string>(j);
                                break;
                            case "IsColumnSet":
                                qf.IsColumnSet = SchemaTable.Rows[i].Field<bool>(j);
                                break;
                            case "NonVersionedProviderType":
                                qf.NonVersionedProviderType = SchemaTable.Rows[i].Field<int>(j);
                                break;
                            default:
                                break;
                        }
                    }
                }
                qf.RawProperties = properties;
                result.Add(qf);
            }

            return result;
        }



        //Perform the query, extract the results
        private static List<DataTable> GetQuerySchema(string strconn, string strSQL)
        {
            //Returns a DataTable filled with the results of the query
            //Function returns the count of records in the datatable
            //----- dt (datatable) needs to be empty & no schema defined
            var result = new List<DataTable>();

            using (var connection = new SqlConnection(strconn))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = strSQL;
                    using (var srdrQuery = command.ExecuteReader())
                    {
                        while (srdrQuery.HasRows) {
                            
                            result.Add(srdrQuery.GetSchemaTable());
                            srdrQuery.NextResult();
                        }
                    }
                }
            }  // the connection will be closed & disposed here
            return result;
        }
    }
}