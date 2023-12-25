using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Data.SqlClient;

namespace CourseWebsiteDotNet.Models
{
    // Lớp LopHocModel chứa các thuộc tính
    public class LopHocModel
    {
        public int IdLopHoc { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int IdMonHoc { get; set; }
    }


    // Lớp LopHocRepository chứa các hàm thao tác với cơ sở dữ liệu
    public class LopHocRepository
    {
        private readonly string connectionString;

        public LopHocRepository()
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
            catch (DbException dbEx)
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

        // Trả về List<LopHocModel>
        public List<LopHocModel> GetAllLopHoc()
        {
            List<LopHocModel> lopHocList = new List<LopHocModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM lop_hoc";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LopHocModel lopHoc = new LopHocModel();
                            lopHoc.IdLopHoc = Convert.ToInt32(reader["id_lop_hoc"]);
                            lopHoc.NgayBatDau = Convert.ToDateTime(reader["ngay_bat_dau"]);
                            lopHoc.NgayKetThuc = Convert.ToDateTime(reader["ngay_ket_thuc"]);
                            lopHoc.IdMonHoc = Convert.ToInt32(reader["id_mon_hoc"]);
                            lopHocList.Add(lopHoc);
                        }
                    }
                }
            }

            return lopHocList;
        }

        // Trả về 1 LopHocModel
        public LopHocModel? GetLopHocById(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM lop_hoc WHERE id_lop_hoc = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new LopHocModel // Trả về 1 LopHocModel
                            {
                                IdLopHoc = Convert.ToInt32(reader["id_lop_hoc"]),
                                NgayBatDau = Convert.ToDateTime(reader["ngay_bat_dau"]),
                                NgayKetThuc = Convert.ToDateTime(reader["ngay_ket_thuc"]),
                                IdMonHoc = Convert.ToInt32(reader["id_mon_hoc"])
                            };
                        }
                        else
                            return null; // Không có trả về null
                    }
                }
            }

            return null;
        }

        // Trả về Response
        public Response InsertLopHoc(LopHocModel lopHoc)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO lop_hoc (ngay_bat_dau, ngay_ket_thuc, id_mon_hoc) " +
                                   "VALUES (@NgayBatDau, @NgayKetThuc, @IdMonHoc); SELECT LAST_INSERT_ID();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NgayBatDau", lopHoc.NgayBatDau);
                        command.Parameters.AddWithValue("@NgayKetThuc", lopHoc.NgayKetThuc);
                        command.Parameters.AddWithValue("@IdMonHoc", lopHoc.IdMonHoc);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            State = true,
                            Message = "Thêm lớp học thành công",
                            InsertedId = insertedId,
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response UpdateLopHoc(LopHocModel lopHoc)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE lop_hoc SET ngay_bat_dau = @NgayBatDau, " +
                                   "ngay_ket_thuc = @NgayKetThuc, id_mon_hoc = @IdMonHoc " +
                    "WHERE id_lop_hoc = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NgayBatDau", lopHoc.NgayBatDau);
                        command.Parameters.AddWithValue("@NgayKetThuc", lopHoc.NgayKetThuc);
                        command.Parameters.AddWithValue("@IdMonHoc", lopHoc.IdMonHoc);
                        command.Parameters.AddWithValue("@Id", lopHoc.IdLopHoc);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Cập nhật thông tin lớp học thành công",
                            InsertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response DeleteLopHoc(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM lop_hoc WHERE id_lop_hoc = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Cập nhật thông tin lớp học thành công",
                            InsertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }

}
