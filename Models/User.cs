using System.Data.Common;
using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class UserModel
    {
        public int id_user { get; set; }
        public string? anh_dai_dien { get; set; }
        public string tai_khoan { get; set; }
        public string mat_khau { get; set; }
        public DateTime? thoi_gian_dang_nhap_gan_nhat { get; set; }
        public int? id_ad { get; set; }
        public int? id_giang_vien { get; set; }
        public int? id_hoc_vien { get; set; }
    }
    public class UserRepository
    {
        private readonly string connectionString;

        public UserRepository()
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

        // Trả về List<UserModel>
        public List<UserModel> GetAllUsers()
        {
            List<UserModel> userList = new List<UserModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM users";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader  reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserModel user = new UserModel();
                            user.id_user = Convert.ToInt32(reader["id_user"]);
                            user.anh_dai_dien = Convert.ToBase64String((byte[])reader["anh_dai_dien"]);
                            user.tai_khoan = reader["tai_khoan"].ToString();
                            user.mat_khau = reader["mat_khau"].ToString();
                            user.thoi_gian_dang_nhap_gan_nhat = Convert.ToDateTime(reader["thoi_gian_dang_nhap_gan_nhat"]);
                            user.id_ad = Convert.ToInt32(reader["id_ad"]);
                            user.id_giang_vien = Convert.ToInt32(reader["id_giang_vien"]);
                            user.id_hoc_vien = Convert.ToInt32(reader["id_hoc_vien"]);
                            userList.Add(user);
                        }
                    }
                }
            }

            return userList;
        }

        public UserModel? GetUserById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM users WHERE id_user = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader  reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserModel
                            {
                                id_user = Convert.ToInt32(reader["id_user"]),
                                anh_dai_dien = reader["anh_dai_dien"] as string,
                                tai_khoan = reader["tai_khoan"].ToString(),
                                mat_khau = reader["mat_khau"].ToString(),
                                thoi_gian_dang_nhap_gan_nhat = Convert.ToDateTime(reader["thoi_gian_dang_nhap_gan_nhat"]),
                                id_ad = Convert.ToInt32(reader["id_ad"]),
                                id_giang_vien = Convert.ToInt32(reader["id_giang_vien"]),
                                id_hoc_vien = Convert.ToInt32(reader["id_hoc_vien"])
                            };
                        }
                        else
                            return null;
                    }
                }
            }
        }

        // Trả về Response
        public Response InsertUser(UserModel user)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO users (anh_dai_dien, tai_khoan, mat_khau, thoi_gian_dang_nhap_gan_nhat, id_ad, id_giang_vien, id_hoc_vien) " +
                                   "VALUES (@AnhDaiDien, @TaiKhoan, @MatKhau, @ThoiGianDangNhapGanNhat, @IdAd, @IdGiangVien, @IdHocVien); SELECT SCOPE_IDENTITY();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AnhDaiDien", user.anh_dai_dien);
                        command.Parameters.AddWithValue("@TaiKhoan", user.tai_khoan);
                        command.Parameters.AddWithValue("@MatKhau", user.mat_khau);
                        command.Parameters.AddWithValue("@ThoiGianDangNhapGanNhat", user.thoi_gian_dang_nhap_gan_nhat);
                        command.Parameters.AddWithValue("@IdAd", user.id_ad);
                        command.Parameters.AddWithValue("@IdGiangVien", user.id_giang_vien);
                        command.Parameters.AddWithValue("@IdHocVien", user.id_hoc_vien);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            State = true,
                            Message = "Thêm người dùng thành công",
                            InsertedId = insertedId,
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response UpdateUser(UserModel user)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE users SET anh_dai_dien = @AnhDaiDien, " +
                                   "tai_khoan = @TaiKhoan, mat_khau = @MatKhau, thoi_gian_dang_nhap_gan_nhat = @ThoiGianDangNhapGanNhat, " +
                                   "id_ad = @IdAd, id_giang_vien = @IdGiangVien, id_hoc_vien = @IdHocVien " +
                                   "WHERE id_user = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AnhDaiDien", user.anh_dai_dien);
                        command.Parameters.AddWithValue("@TaiKhoan", user.tai_khoan);
                        command.Parameters.AddWithValue("@MatKhau", user.mat_khau);
                        command.Parameters.AddWithValue("@ThoiGianDangNhapGanNhat", user.thoi_gian_dang_nhap_gan_nhat);
                        command.Parameters.AddWithValue("@IdAd", user.id_ad);
                        command.Parameters.AddWithValue("@IdGiangVien", user.id_giang_vien);
                        command.Parameters.AddWithValue("@IdHocVien", user.id_hoc_vien);
                        command.Parameters.AddWithValue("@Id", user.id_user);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Cập nhật thông tin người dùng thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response DeleteUser(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM users WHERE id_user = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Xóa người dùng thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }

}
