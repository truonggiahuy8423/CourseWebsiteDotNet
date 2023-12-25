using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class ChiTietBaiNopModel
    {
        public int id_bai_nop { get; set; }
        public int id_tep_tin_tai_len { get; set; }
    }
    public class ChiTietBaiNopRepository
    {
        private readonly string connectionString;
        public ChiTietBaiNopRepository()
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

        // Trả về List<ChiTietBaiNopModel>
        public List<ChiTietBaiNopModel> GetAllChiTietBaiNop()
        {
            List<ChiTietBaiNopModel> chiTietBaiNopList = new List<ChiTietBaiNopModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM chiTietBaiNop";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ChiTietBaiNopModel chiTietBaiNop = new ChiTietBaiNopModel();
                            chiTietBaiNop.id_bai_nop = Convert.ToInt32(reader["id_bai_nop"]);
                            chiTietBaiNop.id_tep_tin_tai_len = Convert.ToInt32(reader["id_tep_tin_tai_len"]);
                            chiTietBaiNopList.Add(chiTietBaiNop);
                        }
                    }
                }
            }

            return chiTietBaiNopList;
        }

        // Trả về 1 ChiTietBaiNopModel
        public ChiTietBaiNopModel? GetChiTietBaiNopById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM chi_tiet_bai_nop WHERE id_bai_nop = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ChiTietBaiNopModel // Trả về 1 ChiTietBaiNopModel
                            {
                                id_bai_nop = Convert.ToInt32(reader["id_bai_nop"]),
                                id_tep_tin_tai_len = Convert.ToInt32(reader["id_tep_tin_tai_len"])
                            };
                        }
                        else
                            return null; // Không có trả về null
                    }
                }
            }
        }

        // Trả về Response
        public Response InsertChiTietBaiNop(ChiTietBaiNopModel chiTietBaiNop)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO chi_tiet_bai_nop (id_tep_tin_tai_len) " +
                                   "VALUES (@id_tep_tin_tai_len); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_tep_tin_tai_len", chiTietBaiNop.id_tep_tin_tai_len);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            State = true,
                            Message = "Thêm bài nộp thành công",
                            InsertedId = insertedId,
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response UpdateChiTietBaiNop(ChiTietBaiNopModel chiTietBaiNop)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE chi_tiet_bai_nop SET id_tep_tin_tai_len = @id_tep_tin_tai_len WHERE id_bai_nop = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_tep_tin_tai_len", chiTietBaiNop.id_tep_tin_tai_len);
                        command.Parameters.AddWithValue("@Id", chiTietBaiNop.id_bai_nop);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Cập nhật thông tin bài nộp thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response DeleteChiTietBaiNop(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM chi_tiet_bai_nop WHERE id_bai_nop = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Cập nhật thông tin bài nộp thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }
}
