using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class DiemDanhModel
    {
        public int id_hoc_vien { get; set; }
        public int id_buoi_hoc { get; set; }
        public string? ghi_chu { get; set; }
        public int? co_mat {  get; set; }
    }
    public class DiemDanhRepository
    {
        private readonly string connectionString;
        public DiemDanhRepository()
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

        // Trả về List<DiemDanhModel>
        public List<DiemDanhModel> GetAllDiemDanh()
        {
            List<DiemDanhModel> diemDanhList = new List<DiemDanhModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM diem_danh";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DiemDanhModel diemDanh = new DiemDanhModel();
                            diemDanh.id_hoc_vien = Convert.ToInt32(reader["id_hoc_vien"]);
                            diemDanh.id_buoi_hoc = Convert.ToInt32(reader["id_buoi_hoc"]);
                            diemDanh.ghi_chu = (reader["ghi_chu"]) != DBNull.Value ? Convert.ToString(reader["ghi_chu"]) : (string?)null;
                            diemDanh.co_mat = (reader["co_mat"]) != DBNull.Value ? Convert.ToInt32(reader["co_mat"]) : (int?)null;
                            diemDanhList.Add(diemDanh);
                        }
                    }
                }
            }

            return diemDanhList;
        }

        // Trả về 1 DiemDanhModel
        public DiemDanhModel? GetDiemDanhById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM diem_danh WHERE id_hoc_vien = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new DiemDanhModel // Trả về 1 DiemDanhModel
                            {
                                id_hoc_vien = Convert.ToInt32(reader["id_hoc_vien"]),
                                id_buoi_hoc = Convert.ToInt32(reader["id_buoi_hoc"]),
                                ghi_chu = (reader["ghi_chu"]) != DBNull.Value ? Convert.ToString(reader["ghi_chu"]) : (string?)null,
                                co_mat = (reader["co_mat"]) != DBNull.Value ? Convert.ToInt32(reader["co_mat"]) : (int?)null
                            };
                        }
                        else
                            return null; // Không có trả về null
                    }
                }
            }
        }

        // Trả về Response
        public Response InsertDiemdanh(DiemDanhModel diemDanh)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO diemDanh (id_buoi_hoc, ghi_chu, co_mat) " +
                                   "VALUES (@id_buoi_hoc, @ghi_chu, @co_mat); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_buoi_hoc", diemDanh.id_buoi_hoc);
                        command.Parameters.AddWithValue("@ghi_chu", diemDanh.ghi_chu);
                        command.Parameters.AddWithValue("@co_mat", diemDanh.co_mat);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            State = true,
                            Message = "Thêm thông tin điểm danh thành công",
                            InsertedId = insertedId,
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response UpdateDiemDanh(DiemDanhModel diemDanh)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE diem_danh SET id_buoi_hoc = @id_buoi_hoc, " +
                                   "ghi_chu = @ghi_chu, co_mat = @co_mat " +
                    "WHERE id_hoc_vien = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_buoi_hoc", diemDanh.id_buoi_hoc);
                        command.Parameters.AddWithValue("@ghi_chu", diemDanh.ghi_chu);
                        command.Parameters.AddWithValue("@co_mat", diemDanh.co_mat);
                        command.Parameters.AddWithValue("@Id", diemDanh.id_hoc_vien);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Cập nhật thông tin điểm danh thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response DeleteDiemDanh(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM diem_danh WHERE id_hoc_vien = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Cập nhật thông tin điểm danh thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }
}
