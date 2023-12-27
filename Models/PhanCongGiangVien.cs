using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{

    public class PhanCongGiangVienModel
    {
        public int? id_giang_vien { get; set; }
        public int? id_lop_hoc { get; set; }
    }


    // Lớp PhanCongGiangVienRepository chứa các hàm thao tác với cơ sở dữ liệu
    public class PhanCongGiangVienRepository
    {
        private readonly string connectionString;

        public PhanCongGiangVienRepository()
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

        // Trả về List<PhanCongGiangVienModel>
        public List<PhanCongGiangVienModel> GetAllPhanCongGiangVien()
        {
            List<PhanCongGiangVienModel> phanCongGiangVienList = new List<PhanCongGiangVienModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM phan_cong_giang_vien";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PhanCongGiangVienModel pc = new PhanCongGiangVienModel();
                            pc.id_giang_vien = reader["id_giang_vien"] != DBNull.Value ? Convert.ToInt32(reader["id_giang_vien"]) : (int?)null;
                            pc.id_lop_hoc = reader["id_lop_hoc"] != DBNull.Value ? Convert.ToInt32(reader["id_lop_hoc"]) : (int?)null;
                            phanCongGiangVienList.Add(pc);
                        }
                    }
                }
            }

            return phanCongGiangVienList;
        }

        // Trả về 1 PhanCongGiangVienModel
        public PhanCongGiangVienModel? GetPhanCongGiangVienById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM phan_cong_giang_vien WHERE id_giang_vien = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new PhanCongGiangVienModel // Trả về 1 PhanCongGiangVienModel
                            {
                                id_giang_vien = reader["id_giang_vien"] != DBNull.Value ? Convert.ToInt32(reader["id_giang_vien"]) : (int?)null,
                                id_lop_hoc = reader["id_lop_hoc"] != DBNull.Value ? Convert.ToInt32(reader["id_lop_hoc"]) : (int?)null
                            };
                        }
                        else
                            return null; // Không có trả về null
                    }
                }
            }
        }

        // Trả về Response
        public Response InsertPhanCongGiangVien(PhanCongGiangVienModel pc)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO phan_cong_giang_vien (id_giang_vien, id_lop_hoc) " +
                                    "VALUES (@id_giang_vien, @id_lop_hoc); ";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_giang_vien", pc.id_giang_vien);
                        command.Parameters.AddWithValue("@id_lop_hoc", pc.id_lop_hoc);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            state = true,
                            message = "Thêm phân công thành công",
                            insertedId = insertedId,
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response UpdatePhanCongGiangVien(PhanCongGiangVienModel pc)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE phan_cong_giang_vien " +
                                    "SET id_lop_hoc = @id_lop_hoc " +
                                    "WHERE id_giang_vien = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_lop_hoc", pc.id_lop_hoc);
                        command.Parameters.AddWithValue("@Id", pc.id_giang_vien);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật thông tin phân công thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response DeletePhanCongGiangVien(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM phan_cong_giang_vien WHERE id_giang_vien = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật thông tin phân công thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }

    }
}
