using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class SQLExecutor
    {
        public static DataTable ExecuteQuery(string sql)
        {
            DataTable dataTable = new DataTable();

            using (MySqlConnection connection = new MySqlConnection(DatabaseConnection.CONNECTION_STRING))
            {
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
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
                using (MySqlConnection connection = new MySqlConnection(DatabaseConnection.CONNECTION_STRING))
                {
                    connection.Open();

                    string query = Sql;

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Thực thi câu lệnh thành công",
                            insertedId = null,
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
            catch (MySqlException dbEx)
            {
                return new Response
                {
                    state = false,
                    message = $"Database Exception: {dbEx.Message}",
                    insertedId = null
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    state = false,
                    message = $"Exception: {ex.Message}",
                    insertedId = null
                };
            }
        }
    }
}
