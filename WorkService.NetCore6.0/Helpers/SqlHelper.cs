using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections;
using System.Reflection;

namespace WorkService.NetCore6._0.Helpers
{
    public class SqlHelper
    {
        private readonly string? _connectionString;

        public SqlHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Connection");
        }

        public Hashtable ModelToHashtableParamSQL<T>(T model)
        {
            Hashtable parameters = new Hashtable();

            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                string propertyName = "@" + property.Name;
                object propertyValue = property.GetValue(model);
                parameters[propertyName] = propertyValue;
            }

            return parameters;
        }


        /// <summary>
        ///Converts a Dataset to a hashtable - Sử dụng hàm này khi 1 stored trả về quá nhiều bảng và không muốn tạo model cụ thể cho từng bảng (nếu bảng chỉ có 1 dòng sẽ là về 1 object là data của dòng đó, nếu 1 dòng trở lên sẽ trả về array)
        /// </summary>
        public Hashtable DatasetSQLToHashtable(DataSet ds, List<string> keys)
        {
            var tableHashtable = new Hashtable();
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                DataTable table = ds.Tables[i];
                string tableName = keys[i];
                var tableData = new List<Hashtable>();

                foreach (DataRow row in table.Rows)
                {
                    var rowData = new Hashtable();
                    foreach (DataColumn column in table.Columns)
                    {
                        string columnName = column.ColumnName;
                        rowData[columnName] = row[column];
                    }

                    tableData.Add(rowData);
                }


                dynamic value = null;

                if (tableData.Count == 1)
                    value = tableData[0];
                if (tableData.Count > 1)
                    value = tableData;

                tableHashtable[tableName] = value;
            }

            return tableHashtable;
        }


        public DataTable QueryNotParamAsDatatable(string proc)
        {
            try
            {


                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(proc, conn)
                    {
                        CommandType = CommandType.Text
                    };
                    SqlDataAdapter dataAdapt = new SqlDataAdapter
                    {
                        SelectCommand = cmd
                    };
                    DataTable dataTable = new DataTable();
                    dataAdapt.Fill(dataTable);
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void ExecSqlNonQuery(string sql)
        {
            try
            {


                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                throw;

            }
        }
        public DataTable QueryAsDataTable(Hashtable param, string sp_proc)
        {
            try
            {
                DataTable dt = new DataTable();


                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sp_proc, conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    foreach (DictionaryEntry de in param)
                    {
                        cmd.Parameters.Add(new SqlParameter(de.Key.ToString(), de.Value));
                    }
                    SqlDataAdapter dataAdapt = new SqlDataAdapter
                    {
                        SelectCommand = cmd
                    };
                    dataAdapt.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataSet QueryAsDataSet(Hashtable param, string sp_proc)
        {

            try
            {
                DataSet ds = new DataSet();


                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sp_proc, conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    foreach (DictionaryEntry de in param)
                    {
                        cmd.Parameters.Add(new SqlParameter(de.Key.ToString(), de.Value));
                    }
                    SqlDataAdapter dataAdapt = new SqlDataAdapter
                    {
                        SelectCommand = cmd
                    };
                    dataAdapt.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }


    }
}
