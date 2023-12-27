using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class GiangVienModel
    {
        public int? id_giang_vien { get; set; }
        public string? ho_ten { get; set; }
        public DateTime? ngay_sinh { get; set; }
        public int? gioi_tinh { get; set; }
        public string? email { get; set; }
    }


    // Lớp GiangVienRepository chứa các hàm thao tác với cơ sở dữ liệu
    public class GiangVienRepository
    {
        private readonly string connectionString;

        public GiangVienRepository()
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

        // Trả về List<GiangVienModel>
        public List<GiangVienModel> GetAllGiangVien()
        {
            List<GiangVienModel> giangVienList = new List<GiangVienModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM giang_vien";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            GiangVienModel giangVien = new GiangVienModel();
                            giangVien.id_giang_vien = reader["id_giang_vien"] != DBNull.Value ? Convert.ToInt32(reader["id_giang_vien"]) : (int?)null;
                            giangVien.ho_ten = reader["ho_ten"] != DBNull.Value ? Convert.ToString(reader["ho_ten"]) : (string?)null;
                            giangVien.ngay_sinh = reader["ngay_sinh"] != DBNull.Value ? Convert.ToDateTime(reader["ngay_sinh"]) : (DateTime?)null;
                            giangVien.gioi_tinh = reader["gioi_tinh"] != DBNull.Value ? Convert.ToInt32(reader["gioi_tinh"]) : (int?)null;
                            giangVien.email = reader["email"] != DBNull.Value ? Convert.ToString(reader["email"]) : (string?)null;
                            giangVienList.Add(giangVien);
                        }
                    }
                }
            }

            return giangVienList;
        }

        // Trả về 1 LopHocModel
        public GiangVienModel? GetLopHocById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM giang_vien WHERE id_giang_vien = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new GiangVienModel // Trả về 1 LopHocModel
                            {
                                id_giang_vien = reader["id_giang_vien"] != DBNull.Value ? Convert.ToInt32(reader["id_giang_vien"]) : (int?)null,
                                ho_ten = reader["ho_ten"] != DBNull.Value ? Convert.ToString(reader["ho_ten"]) : (string?)null,
                                ngay_sinh = reader["ngay_sinh"] != DBNull.Value ? Convert.ToDateTime(reader["ngay_sinh"]) : (DateTime?)null,
                                gioi_tinh = reader["gioi_tinh"] != DBNull.Value ? Convert.ToInt32(reader["gioi_tinh"]) : (int?)null,
                                email = reader["email"] != DBNull.Value ? Convert.ToString(reader["email"]) : (string?)null
                            };
                        }
                        else
                            return null; // Không có trả về null
                    }
                }
            }
        }

        // Trả về Response
        public Response InsertGiangVien(GiangVienModel giangVien)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO giang_vien (ho_ten, ngay_sinh, gioi_tinh, email) " +
                                    "VALUES (@ho_ten, @ngay_sinh, @gioi_tinh, @email); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ho_ten", giangVien.ho_ten);
                        command.Parameters.AddWithValue("@ngay_sinh", giangVien.ngay_sinh);
                        command.Parameters.AddWithValue("@gioi_tinh", giangVien.gioi_tinh);
                        command.Parameters.AddWithValue("@email", giangVien.email);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            State = true,
                            Message = "Thêm giảng viên thành công",
                            InsertedId = insertedId,
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response UpdateGiangVien(GiangVienModel giangVien)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE giang_vien " +
                                    "SET ho_ten = @ho_ten, " +
                                        "ngay_sinh = @ngay_sinh, " +
                                        "ho_ten = @ho_ten, " +
                                        "email = @email " +
                                    "WHERE id_giang_vien = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ho_ten", giangVien.ho_ten);
                        command.Parameters.AddWithValue("@ngay_sinh", giangVien.ngay_sinh);
                        command.Parameters.AddWithValue("@gioi_tinh", giangVien.gioi_tinh);
                        command.Parameters.AddWithValue("@email", giangVien.email);
                        command.Parameters.AddWithValue("@Id", giangVien.id_giang_vien);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Cập nhật thông tin giảng viên thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response DeleteGiangVien(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM giang_vien WHERE id_giang_vien = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                            return new Response
                            {
                                State = true,
                                Message = "Cập nhật thông tin giảng viên thành công",
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

