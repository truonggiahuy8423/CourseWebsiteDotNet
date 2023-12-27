using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    // Lớp HocVienModel chứa các thuộc tính
    public class HocVienModel
    {
        public int id_hoc_vien { get; set; }
        public string ho_ten { get; set; }
        public DateTime? ngay_sinh { get; set; }
        public int? gioi_tinh { get; set; }
        public string email { get; set; }
    }


    // Lớp HocVienRepository chứa các hàm thao tác với cơ sở dữ liệu
    public class HocVienRepository
    {
        private readonly string connectionString;

        public HocVienRepository()
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

        // Trả về List<HocVienModel>
        public List<HocVienModel> GetAllHocVien()
        {
            List<HocVienModel> hocVienList = new List<HocVienModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM hoc_vien";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            HocVienModel hocVien = new HocVienModel();
                            hocVien.id_hoc_vien = Convert.ToInt32(reader["id_hoc_vien"]);
                            hocVien.ho_ten = Convert.ToString(reader["ho_ten"]);
                            hocVien.ngay_sinh = (reader["ngay_sinh"]) != DBNull.Value ? Convert.ToDateTime(reader["ngay_sinh"]) : (DateTime?)null;
                            hocVien.gioi_tinh = (reader["gioi_tinh"]) != DBNull.Value ? Convert.ToInt32(reader["gioi_tinh"]) : (int?)null;
                            hocVien.email = Convert.ToString(reader["email"]);
                            hocVienList.Add(hocVien);
                        }
                    }
                }
            }

            return hocVienList;
        }

        // Trả về 1 HocVienModel
        public HocVienModel? GetHocVienById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM hoc_vien WHERE id_hoc_vien = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new HocVienModel // Trả về 1 HocVienModel
                            {
                                id_hoc_vien = Convert.ToInt32(reader["id_hoc_vien"]),
                                ho_ten = Convert.ToString(reader["ho_ten"]),
                                ngay_sinh = (reader["ngay_sinh"]) != DBNull.Value ? Convert.ToDateTime(reader["ngay_sinh"]) : (DateTime?)null,
                                gioi_tinh = (reader["gioi_tinh"]) != DBNull.Value ? Convert.ToInt32(reader["gioi_tinh"]) : (int?)null,
                                email = Convert.ToString(reader["email"]),
                            };
                        }
                        else
                            return null; // Không có trả về null
                    }
                }
            }
        }

        // Trả về Response
        public Response InsertHocVien(HocVienModel hocVien)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO hoc_vien (ho_ten, ngay_sinh, gioi_tinh, email) " +
                                   "VALUES (@ho_ten, @ngay_sinh, @gioi_tinh, @email); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ho_ten", hocVien.ho_ten);
                        command.Parameters.AddWithValue("@ngay_sinh", hocVien.ngay_sinh);
                        command.Parameters.AddWithValue("@gioi_tinh", hocVien.gioi_tinh);
                        command.Parameters.AddWithValue("@email", hocVien.email);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            state = true,
                            message = "Thêm học viên thành công",
                            insertedId = insertedId,
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response UpdateHocVien(HocVienModel hocVien)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE hoc_vien SET ho_ten = @ho_ten, ngay_sinh = @ngay_sinh, " +
                                   "gioi_tinh = @gioi_tinh, email = @email " +
                    "WHERE id_hoc_vien = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ho_ten", hocVien.ho_ten);
                        command.Parameters.AddWithValue("@ngay_sinh", hocVien.ngay_sinh);
                        command.Parameters.AddWithValue("@gioi_tinh", hocVien.gioi_tinh);
                        command.Parameters.AddWithValue("@email", hocVien.email);
                        command.Parameters.AddWithValue("@Id", hocVien.id_hoc_vien);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật thông tin học viên thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response DeleteHocVien(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM hoc_vien WHERE id_hoc_vien = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật thông tin học viên thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }
}
