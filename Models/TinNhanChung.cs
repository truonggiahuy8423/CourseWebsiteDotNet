using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class TinNhanChung
    {
        public class TinNhanChungModel
        {
            public int id_tin_nhan { get; set; }
            public string noi_dung { get; set; }
            public DateTime thoi_gian { get; set; }
            public int user_gui { get; set; }
            public int kenh_nhan { get; set; }
        }


        // Lớp TinNhanChungRepository chứa các hàm thao tác với cơ sở dữ liệu
        public class TinNhanChungRepository
        {
            private readonly string connectionString;

            public TinNhanChungRepository()
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

            // Trả về List<TinNhanChungModel>
            public List<TinNhanChungModel> GetAllTinNhanChung()
            {
                List<TinNhanChungModel> TinNhanChungList = new List<TinNhanChungModel>();
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM tin_nhan_chung";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TinNhanChungModel tn = new TinNhanChungModel();
                                tn.id_tin_nhan = Convert.ToInt32(reader["id_tin_nhan"]);
                                tn.noi_dung = Convert.ToString(reader["noi_dung"]);
                                tn.thoi_gian = Convert.ToDateTime(reader["thoi_gian"]);
                                tn.user_gui = Convert.ToInt32(reader["user_gui"]);
                                tn.kenh_nhan = Convert.ToInt32(reader["kenh_nhan"]);
                                TinNhanChungList.Add(tn);
                            }
                        }
                    }
                }

                return TinNhanChungList;
            }

            // Trả về 1 TinNhanChungModel
            public TinNhanChungModel? GetTinNhanChungById(int id)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM tin_nhan_chung WHERE id_tin_nhan = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new TinNhanChungModel // Trả về 1 TinNhanChungModel
                                {
                                    id_tin_nhan = Convert.ToInt32(reader["id_tin_nhan"]),
                                    noi_dung = Convert.ToString(reader["noi_dung"]),
                                    thoi_gian = Convert.ToDateTime(reader["thoi_gian"]),
                                    user_gui = Convert.ToInt32(reader["user_gui"]),
                                    kenh_nhan = Convert.ToInt32(reader["kenh_nhan"])
                                };
                            }
                            else
                                return null; // Không có trả về null
                        }
                    }
                }
            }

            // Trả về Response
            public Response InsertTinNhanChung(TinNhanChungModel tn)
            {
                return ExecuteDatabaseOperation(() =>
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "INSERT INTO tin_nhan_chung (noi_dung, thoi_gian, user_gui, kenh_nhan) " +
                                       "VALUES (@noi_dung, @thoi_gian, @user_gui, @kenh_nhan); " +
                                       "SELECT LAST_INSERT_ID();";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@noi_dung", tn.noi_dung);
                            command.Parameters.AddWithValue("@thoi_gian", tn.thoi_gian);
                            command.Parameters.AddWithValue("@user_gui", tn.user_gui);
                            command.Parameters.AddWithValue("@kenh_nhan", tn.kenh_nhan);

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
            public Response UpdateTinNhanChung(TinNhanChungModel tn)
            {
                return ExecuteDatabaseOperation(() =>
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "UPDATE tin_nhan_chung " +
                                       "SET " +
                                            "noi_dung = @noi_dung, " +
                                            "thoi_gian = @thoi_gian " +
                                            "user_gui = @user_gui " +
                                            "kenh_nhan = @kenh_nhan " +
                                       "WHERE id_tin_nhan = @Id";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@noi_dung", tn.noi_dung);
                            command.Parameters.AddWithValue("@thoi_gian", tn.thoi_gian);
                            command.Parameters.AddWithValue("@user_gui", tn.user_gui);
                            command.Parameters.AddWithValue("@kenh_nhan", tn.kenh_nhan);
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
            public Response DeleteTinNhanChung(int id)
            {
                return ExecuteDatabaseOperation(() =>
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "DELETE FROM tin_nhan_chung WHERE id_tin_nhan = @Id";

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
