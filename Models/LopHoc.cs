using System.Data.Common;
using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    // Lớp LopHocModel chứa các thuộc tính
    public class LopHocModel
    {
        public int id_lop_hoc { get; set; }
        public DateTime ngay_bat_dau { get; set; }
        public DateTime ngay_ket_thuc { get; set; }
        public int id_mon_hoc { get; set; }
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

        // Trả về List<LopHocModel>
        public List<LopHocModel> GetAllLopHoc()
        {
            List<LopHocModel> lopHocList = new List<LopHocModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM lop_hoc";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader  reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LopHocModel lopHoc = new LopHocModel();
                            lopHoc.id_lop_hoc = Convert.ToInt32(reader["id_lop_hoc"]) != DBNull.Value ? Convert.ToInt32(reader["id_lop_hoc"]) : (int?)null;
                            lopHoc.ngay_bat_dau = Convert.ToDateTime(reader["ngay_bat_dau"]) != DBNull.Value ? Convert.ToDateTime(reader["ngay_bat_dau"]) : (DateTime?)null;
                            lopHoc.ngay_ket_thuc = Convert.ToDateTime(reader["ngay_ket_thuc"]) != DBNull.Value ? Convert.ToDateTime(reader["ngay_ket_thuc"]) : (DateTime?)null;
                            lopHoc.id_mon_hoc = Convert.ToInt32(reader["id_mon_hoc"]) != DBNull.Value ? Convert.ToInt32(reader["id_mon_hoc"]) : (int?)null;
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
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM lop_hoc WHERE id_lop_hoc = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader  reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new LopHocModel // Trả về 1 LopHocModel
                            {
                                id_lop_hoc = Convert.ToInt32(reader["id_lop_hoc"]) != DBNull.Value ? Convert.ToInt32(reader["id_lop_hoc"]) : (int?)null,
                                ngay_bat_dau = Convert.ToDateTime(reader["ngay_bat_dau"]) != DBNull.Value ? Convert.ToDateTime(reader["ngay_bat_dau"]) : (DateTime?)null,
                                ngay_ket_thuc = Convert.ToDateTime(reader["ngay_ket_thuc"]) != DBNull.Value ? Convert.ToDateTime(reader["ngay_ket_thuc"]) : (DateTime?)null,
                                id_mon_hoc = Convert.ToInt32(reader["id_mon_hoc"]) != DBNull.Value ? Convert.ToInt32(reader["id_mon_hoc"]) : (int?)null
                            };
                        }
                        else
                            return null; // Không có trả về null
                    }
                }
            }
        }

        // Trả về Response
        public Response InsertLopHoc(LopHocModel lopHoc)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO lop_hoc (ngay_bat_dau, ngay_ket_thuc, id_mon_hoc) " +
                                   "VALUES (@ngay_bat_dau, @ngay_ket_thuc, @id_mon_hoc); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ngay_bat_dau", lopHoc.ngay_bat_dau);
                        command.Parameters.AddWithValue("@ngay_ket_thuc", lopHoc.ngay_ket_thuc);
                        command.Parameters.AddWithValue("@id_mon_hoc", lopHoc.id_mon_hoc);

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
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE lop_hoc SET ngay_bat_dau = @ngay_bat_dau, " +
                                   "ngay_ket_thuc = @ngay_ket_thuc, id_mon_hoc = @id_mon_hoc " +
                    "WHERE id_lop_hoc = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ngay_bat_dau", lopHoc.ngay_bat_dau);
                        command.Parameters.AddWithValue("@ngay_ket_thuc", lopHoc.ngay_ket_thuc);
                        command.Parameters.AddWithValue("@id_mon_hoc", lopHoc.id_mon_hoc);
                        command.Parameters.AddWithValue("@Id", lopHoc.id_lop_hoc);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Cập nhật thông tin lớp học thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
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
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM lop_hoc WHERE id_lop_hoc = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Cập nhật thông tin lớp học thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }

}
