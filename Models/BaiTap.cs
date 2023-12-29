using MySql.Data.MySqlClient;
using System.Data;

namespace CourseWebsiteDotNet.Models
{
    public class BaiTapModel
    {
        public int? id_bai_tap { get; set; }
        public int? trang_thai { get; set; }
        public string? ten { get; set; }
        public string? noi_dung { get; set; }
        public DateTime? thoi_han { get; set; }
        public int? id_giang_vien { get; set; }
        public int? id_muc { get; set; }
    }


    // Lớp BaiTapRepository chứa các hàm thao tác với cơ sở dữ liệu
    public class BaiTapRepository
    {
        private readonly string connectionString;

        public DataTable GetBaiTapByCourseId(int id)
        {
            DataTable dataTable = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = $@"SELECT bai_tap.*, giang_vien.ho_ten FROM bai_tap inner join giang_vien on bai_tap.id_giang_vien = giang_vien.id_giang_vien 
                    inner join muc on muc.id_muc = bai_tap.id_muc where muc.id_lop_hoc = @Id order by bai_tap.ngay_dang ASC;
                    ";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);


                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }
                }
            }

            return dataTable;
        }
        public BaiTapRepository()
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

        // Trả về List<BaiTapModel>
        public List<BaiTapModel> GetAllBaiTap()
        {
            List<BaiTapModel> baiTapList = new List<BaiTapModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM bai_tap";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BaiTapModel baiTap = new BaiTapModel();
                            baiTap.id_bai_tap = reader["id_bai_tap"] != DBNull.Value ? Convert.ToInt32(reader["id_bai_tap"]) : (int?)null;
                            baiTap.trang_thai = reader["trang_thai"] != DBNull.Value ? Convert.ToInt32(reader["trang_thai"]) : (int?)null;
                            baiTap.ten = reader["ten"] != DBNull.Value ? Convert.ToString(reader["ten"]) : (string?)null;
                            baiTap.noi_dung = reader["noi_dung"] != DBNull.Value ? Convert.ToString(reader["noi_dung"]) : (string?)null;
                            baiTap.thoi_han = reader["thoi_han"] != DBNull.Value ? Convert.ToDateTime(reader["thoi_han"]) : (DateTime?)null;
                            baiTap.id_giang_vien = reader["id_giang_vien"] != DBNull.Value ? Convert.ToInt32(reader["id_giang_vien"]) : (int?)null;
                            baiTap.id_muc = reader["id_muc"] != DBNull.Value ? Convert.ToInt32(reader["id_muc"]) : (int?)null;
                            baiTapList.Add(baiTap);
                        }
                    }
                }
            }

            return baiTapList;
        }

        // Trả về 1 BaiTapModel
        public BaiTapModel? GetBaiTapById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM bai_tap WHERE id_bai_tap = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new BaiTapModel // Trả về 1 BaiTapModel
                            {
                                id_bai_tap = reader["id_bai_tap"] != DBNull.Value ? Convert.ToInt32(reader["id_bai_tap"]) : (int?)null,
                                trang_thai = reader["trang_thai"] != DBNull.Value ? Convert.ToInt32(reader["trang_thai"]) : (int?)null,
                                ten = reader["ten"] != DBNull.Value ? Convert.ToString(reader["ten"]) : (string?)null,
                                noi_dung = reader["noi_dung"] != DBNull.Value ? Convert.ToString(reader["noi_dung"]) : (string?)null,
                                thoi_han =reader["thoi_han"] != DBNull.Value ? Convert.ToDateTime(reader["thoi_han"]) : (DateTime?)null,
                                id_giang_vien = reader["id_giang_vien"] != DBNull.Value ? Convert.ToInt32(reader["id_giang_vien"]) : (int?)null,
                                id_muc = reader["id_muc"] != DBNull.Value ? Convert.ToInt32(reader["id_muc"]) : (int?)null
                            };
                        }
                        else
                            return null; // Không có trả về null
                    }
                }
            }
        }

        // Trả về Response
        public Response InsertBaiTap(BaiTapModel baiTap)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO bai_tap (trang_thai, ten, noi_dung, thoi_han, id_giang_vien, id_muc) " +
                                    "VALUES (@trang_thai, @ten, @noi_dung, @thoi_han, @id_giang_vien, @id_muc); " +
                                    "SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@trang_thai", baiTap.trang_thai);
                        command.Parameters.AddWithValue("@ten", baiTap.ten);
                        command.Parameters.AddWithValue("@noi_dung", baiTap.noi_dung);
                        command.Parameters.AddWithValue("@thoi_han", baiTap.thoi_han);
                        command.Parameters.AddWithValue("@id_giang_vien", baiTap.id_giang_vien);
                        command.Parameters.AddWithValue("@id_muc", baiTap.id_muc);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            state = true,
                            message = "Thêm bài tập thành công",
                            insertedId = insertedId,
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response UpdateBaiTap(BaiTapModel baiTap)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE bai_tap " +
                                    "SET trang_thai = @trang_thai, " +
                                        "ten = @ten, " +
                                        "trang_thai = @trang_thai, " +
                                        "thoi_han = @thoi_han " +
                                        "id_giang_vien = @id_giang_vien " +
                                        "id_muc = @id_muc " +
                                    "WHERE id_bai_tap = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@trang_thai", baiTap.trang_thai);
                        command.Parameters.AddWithValue("@ten", baiTap.ten);
                        command.Parameters.AddWithValue("@noi_dung", baiTap.noi_dung);
                        command.Parameters.AddWithValue("@thoi_han", baiTap.thoi_han);
                        command.Parameters.AddWithValue("@id_giang_vien", baiTap.id_giang_vien);
                        command.Parameters.AddWithValue("@id_muc", baiTap.id_muc);
                        command.Parameters.AddWithValue("@Id", baiTap.id_bai_tap);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật thông tin bài tập thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response DeleteBaiTap(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM bai_tap WHERE id_bai_tap = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật thông tin bài tập thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }
}
