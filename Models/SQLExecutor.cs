using System.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace CourseWebsiteDotNet.Models
{
    public class SQLExecutor
    {
        public static DataTable ExecuteQuery(string sql)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(DatabaseConnection.CONNECTION_STRING))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }
                }
            }
            return dataTable;
        }

        public static Response ExecuteDML(string Sql) {
            return ExecuteDatabaseOperation(() =>
            {
                using (SqlConnection connection = new SqlConnection(DatabaseConnection.CONNECTION_STRING))
                {
                    connection.Open();

                    string query = Sql;

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Thực thi câu lệnh thành công",
                            InsertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }


        private static Response ExecuteDatabaseOperation(Func<Response> operation)
        {
            try
            {
                return operation();
            }
            catch (DbException dbEx)
            {
                return new Response
                {
                    State = false,
                    Message = $"Database Exception: {dbEx.Message}",
                    InsertedId = null
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    State = false,
                    Message = $"Exception: {ex.Message}",
                    InsertedId = null
                };
            }
        }
    }
}
