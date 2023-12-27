using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    // Lớp HocVienThamGiaModel chứa các thuộc tính
    public class HocVienThamGiaModel
    {
        public int id_hoc_vien { get; set; }
        public int id_lop_hoc { get; set; }
    }


    // Lớp HocVienThamGiaRepository chứa các hàm thao tác với cơ sở dữ liệu
    public class HocVienThamGiaRepository
    {
        private readonly string connectionString;

        public HocVienThamGiaRepository()
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

        // Trả về List<HocVienThamGiaModel>
        public List<HocVienThamGiaModel> GetAllHocVienThamGia()
        {
            List<HocVienThamGiaModel> hocVienThamGiaList = new List<HocVienThamGiaModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM hoc_vien_tham_gia";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            HocVienThamGiaModel hocVienThamGia = new HocVienThamGiaModel();
                            hocVienThamGia.id_hoc_vien = Convert.ToInt32(reader["id_hoc_vien"]);
                            hocVienThamGia.id_lop_hoc = Convert.ToInt32(reader["id_lop_hoc"]);
                            hocVienThamGiaList.Add(hocVienThamGia);
                        }
                    }
                }
            }

            return hocVienThamGiaList;
        }

        // Trả về 1 HocVienThamGiaModel
        public HocVienThamGiaModel? GetHocVienThamGiaById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM hoc_vien_tham_gia WHERE id_hoc_vien = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new HocVienThamGiaModel // Trả về 1 HocVienThamGiaModel
                            {
                                id_hoc_vien = Convert.ToInt32(reader["id_hoc_vien"]),
                                id_lop_hoc = Convert.ToInt32(reader["id_lop_hoc"])
                            };
                        }
                        else
                            return null; // Không có trả về null
                    }
                }
            }
        }

        // Trả về Response
        public Response InsertHocVienThamGia(HocVienThamGiaModel hocVienThamGia)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO hoc_vien_tham_gia (id_lop_hoc) " +
                                   "VALUES (@id_lop_hoc); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_lop_hoc", hocVienThamGia.id_lop_hoc);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            state = true,
                            message = "Thêm thông tin học viên tham gia thành công",
                            insertedId = insertedId,
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response UpdateHocVienThamGia(HocVienThamGiaModel hocVienThamGia)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE hoc_vien_tham_gia SET id_lop_hoc = @id_lop_hoc WHERE id_hoc_vien = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_lop_hoc", hocVienThamGia.id_lop_hoc);
                        command.Parameters.AddWithValue("@Id", hocVienThamGia.id_hoc_vien);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật thông tin học viên tham gia thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response DeleteHocVienThamGia(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM hoc_vien_tham_gia WHERE id_hoc_vien = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật thông tin học viên tham gia thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }
}