using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class UserAgentLoginStatusModel
    {
        public int? id { get; set; }
        public string? user_agent { get; set; }
        public string? ip { get; set; }
        public int? login_status { get; set; }
    }

    public class UserAgentLoginStatusRepository
    {
        private readonly string connectionString;

        public UserAgentLoginStatusRepository()
        {
            connectionString = DatabaseConnection.CONNECTION_STRING;
        }

        private Response ExecuteDatabaseOperation(Func<Response> operation)
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

        // Hàm Insert vào cơ sở dữ liệu
        public Response InsertUserAgentLoginStatus(UserAgentLoginStatusModel userAgentLoginStatus)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO user_agent_login_status (user_agent, ip, login_status) " +
                                   "VALUES (@user_agent, @ip, @login_status); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@user_agent", userAgentLoginStatus.user_agent);
                        command.Parameters.AddWithValue("@ip", userAgentLoginStatus.ip);
                        command.Parameters.AddWithValue("@login_status", userAgentLoginStatus.login_status);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            state = true,
                            message = "Thêm thông tin user-agent login status thành công",
                            insertedId = insertedId,
                        };
                    }
                }
            });
        }

        // Hàm tìm kiếm theo user_agent và ip
        public List<UserAgentLoginStatusModel> GetUserAgentLoginStatus(string userAgent, string ip, int top)
        {
            // Khởi tạo danh sách để lưu kết quả
            var results = new List<UserAgentLoginStatusModel>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Xây dựng truy vấn với LIMIT (thay {top} bằng giá trị top)
                string query = $"SELECT * FROM user_agent_login_status WHERE user_agent = @userAgent AND ip = @ip ORDER BY id DESC LIMIT {top}";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Thêm tham số truy vấn
                    command.Parameters.AddWithValue("@userAgent", userAgent);
                    command.Parameters.AddWithValue("@ip", ip);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Duyệt qua các bản ghi
                        while (reader.Read())
                        {
                            // Thêm từng bản ghi vào danh sách
                            results.Add(new UserAgentLoginStatusModel
                            {
                                id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : (int?)null,
                                user_agent = reader["user_agent"] != DBNull.Value ? Convert.ToString(reader["user_agent"]) : (string?)null,
                                ip = reader["ip"] != DBNull.Value ? Convert.ToString(reader["ip"]) : (string?)null,
                                login_status = reader["login_status"] != DBNull.Value ? Convert.ToInt32(reader["login_status"]) : (int?)null,
                            });
                        }
                    }
                }
            }

            // Trả về danh sách kết quả
            return results;
        }

    }

}
