using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class TinNhanRieng
    {
        public class TinNhanRiengModel
        {
            public int id_tin_nhan { get; set; }
            public string noi_dung { get; set; }
            public DateTime thoi_gian { get; set; }
            public int user_gui { get; set; }
            public int user_nhan { get; set; }
        }


        // Lớp TinNhanRiengRepository chứa các hàm thao tác với cơ sở dữ liệu
        public class TinNhanRiengRepository
        {
            private readonly string connectionString;

            public TinNhanRiengRepository()
            {
                connectionString = DatabaseConnection.CONNECTION_STRING;
            }

            // Hàm try catch lỗi từ database
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

            // Trả về List<TinNhanRiengModel>
            public List<TinNhanRiengModel> GetAllTinNhanRieng()
            {
                List<TinNhanRiengModel> tinNhanRiengList = new List<TinNhanRiengModel>();
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM tin_nhan_rieng";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TinNhanRiengModel tn = new TinNhanRiengModel();
                                tn.id_tin_nhan = Convert.ToInt32(reader["id_tin_nhan"]) != DBNull.Value ? Convert.ToInt32(reader["id_tin_nhan"]) : (int?)null;
                                tn.noi_dung = Convert.ToString(reader["noi_dung"]) != DBNull.Value ? Convert.ToString(reader["noi_dung"]) : (string?)null;
                                tn.thoi_gian = Convert.ToDateTime(reader["thoi_gian"]) != DBNull.Value ? Convert.ToDateTime(reader["thoi_gian"]) : (DateTime?)null;
                                tn.user_gui = Convert.ToInt32(reader["user_gui"]) != DBNull.Value ? Convert.ToInt32(reader["user_gui"]) : (int?)null;
                                tn.user_nhan = Convert.ToInt32(reader["user_nhan"]) != DBNull.Value ? Convert.ToInt32(reader["user_nhan"]) : (int?)null;
                                tinNhanRiengList.Add(tn);
                            }
                        }
                    }
                }

                return tinNhanRiengList;
            }

            // Trả về 1 TinNhanRiengModel
            public TinNhanRiengModel? GetTinNhanRiengById(int id)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM tin_nhan_rieng WHERE id_tin_nhan = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new TinNhanRiengModel // Trả về 1 TinNhanRiengModel
                                {
                                    id_tin_nhan = Convert.ToInt32(reader["id_tin_nhan"]) != DBNull.Value ? Convert.ToInt32(reader["id_tin_nhan"]) : (int?)null,
                                    noi_dung = Convert.ToString(reader["noi_dung"]) != DBNull.Value ? Convert.ToString(reader["noi_dung"]) : (string?)null,
                                    thoi_gian = Convert.ToDateTime(reader["thoi_gian"]) != DBNull.Value ? Convert.ToDateTime(reader["thoi_gian"]) : (DateTime?)null,
                                    user_gui = Convert.ToInt32(reader["user_gui"]) != DBNull.Value ? Convert.ToInt32(reader["user_gui"]) : (int?)null,
                                    user_nhan = Convert.ToInt32(reader["user_nhan"]) != DBNull.Value ? Convert.ToInt32(reader["user_nhan"]) : (int?)null
                                };
                            }
                            else
                                return null; // Không có trả về null
                        }
                    }
                }
            }

            // Trả về Response
            public Response InsertTinNhanRieng(TinNhanRiengModel tn)
            {
                return ExecuteDatabaseOperation(() =>
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "INSERT INTO tin_nhan_rieng (noi_dung, thoi_gian, user_gui, user_nhan) " +
                                       "VALUES (@noi_dung, @thoi_gian, @user_gui, @user_nhan); " +
                                       "SELECT LAST_INSERT_ID();";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@noi_dung", tn.noi_dung);
                            command.Parameters.AddWithValue("@thoi_gian", tn.thoi_gian);
                            command.Parameters.AddWithValue("@user_gui", tn.user_gui);
                            command.Parameters.AddWithValue("@user_nhan", tn.user_nhan);

                            int insertedId = Convert.ToInt32(command.ExecuteScalar());

                            return new Response
                            {
                                State = true,
                                Message = "Thêm bài tập thành công",
                                InsertedId = insertedId,
                            };
                        }
                    }
                });
            }

            // Trả về Response
            public Response UpdateTinNhanRieng(TinNhanRiengModel tn)
            {
                return ExecuteDatabaseOperation(() =>
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "UPDATE tin_nhan_rieng " +
                                       "SET " +
                                            "noi_dung = @noi_dung, " +
                                            "thoi_gian = @thoi_gian " +
                                            "user_gui = @user_gui " +
                                            "user_nhan = @user_nhan " +
                                       "WHERE id_tin_nhan = @Id";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@noi_dung", tn.noi_dung);
                            command.Parameters.AddWithValue("@thoi_gian", tn.thoi_gian);
                            command.Parameters.AddWithValue("@user_gui", tn.user_gui);
                            command.Parameters.AddWithValue("@user_nhan", tn.user_nhan);
                            command.Parameters.AddWithValue("@Id", tn.id_tin_nhan);

                            int effectedRows = command.ExecuteNonQuery();

                            return new Response
                            {
                                State = true,
                                Message = "Cập nhật thông tin bài tập thành công",
                                InsertedId = null,
                                EffectedRows = effectedRows
                            };
                        }
                    }
                });
            }

            // Trả về Response
            public Response DeleteTinNhanRieng(int id)
            {
                return ExecuteDatabaseOperation(() =>
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "DELETE FROM tin_nhan_rieng WHERE id_tin_nhan = @Id";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", id);

                            int effectedRows = command.ExecuteNonQuery();

                            return new Response
                            {
                                State = true,
                                Message = "Cập nhật thông tin tin nhắn thành công",
                                InsertedId = null,
                                EffectedRows = effectedRows
                            };
                        }
                    }
                });
            }
        }
    }
}
