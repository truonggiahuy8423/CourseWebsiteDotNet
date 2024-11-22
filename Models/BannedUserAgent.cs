using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
        public class BannedUserAgentModel
        {
            public int? id { get; set; }
            public string? user_agent { get; set; }
            public string? ip { get; set; }
            public DateTime? banned_to { get; set; }
        }

        public class BannedUserAgentRepository
        {
            private readonly string connectionString;

            public BannedUserAgentRepository()
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
        public Response InsertBannedUserAgent(BannedUserAgentModel bannedUserAgent)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO banned_user_agent (user_agent, ip, banned_to) " +
                                   "VALUES (@user_agent, @ip, @banned_to); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@user_agent", bannedUserAgent.user_agent);
                        command.Parameters.AddWithValue("@ip", bannedUserAgent.ip);
                        command.Parameters.AddWithValue("@banned_to", bannedUserAgent.banned_to);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            state = true,
                            message = "Thêm thông tin bị cấm thành công",
                            insertedId = insertedId,
                        };
                    }
                }
            });
        }

        public Response UpdateBannedUserAgent(BannedUserAgentModel bannedUserAgent)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE banned_user_agent " +
                                   "SET user_agent = @user_agent, " +
                                       "ip = @ip, " +
                                       "banned_to = @banned_to " +
                                   "WHERE id = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@user_agent", bannedUserAgent.user_agent);
                        command.Parameters.AddWithValue("@ip", bannedUserAgent.ip);
                        command.Parameters.AddWithValue("@banned_to", bannedUserAgent.banned_to);
                        command.Parameters.AddWithValue("@Id", bannedUserAgent.id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật thông tin bị cấm thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }

        public BannedUserAgentModel? GetBannedUserAgentByUserAgentAndIp(string userAgent, string ip)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM banned_user_agent WHERE user_agent = @UserAgent AND ip = @Ip";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserAgent", userAgent);
                    command.Parameters.AddWithValue("@Ip", ip);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new BannedUserAgentModel
                            {
                                id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : (int?)null,
                                user_agent = reader["user_agent"] != DBNull.Value ? Convert.ToString(reader["user_agent"]) : (string?)null,
                                ip = reader["ip"] != DBNull.Value ? Convert.ToString(reader["ip"]) : (string?)null,
                                banned_to = reader["banned_to"] != DBNull.Value ? Convert.ToDateTime(reader["banned_to"]) : (DateTime?)null
                            };
                        }
                        else
                        {
                            return null; // Không tìm thấy
                        }
                    }
                }
            }
        }








    }

}
