using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class BaiTap
    {
        public class BaiTapModel
        {
            public int id_bai_tap { get; set; }
            public int trang_thai { get; set; }
            public string ten { get; set; }
            public string noi_dung { get; set; }
            public DateTime thoi_han { get; set; }
            public int id_giang_vien { get; set; }
            public int id_muc { get; set; }
        }


        // Lớp BaiTapRepository chứa các hàm thao tác với cơ sở dữ liệu
        public class BaiTapRepository
        {
            private readonly string connectionString;

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

            // Trả về List<BaiTapModel>
            public List<BaiTapModel> GetAllBaiTap()
            {
                List<BaiTapModel> BaiTapList = new List<BaiTapModel>();
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
                                baiTap.id_bai_tap = Convert.ToInt32(reader["id_bai_tap"]);
                                baiTap.trang_thai = Convert.ToInt32(reader["trang_thai"]);
                                baiTap.ten = Convert.ToString(reader["ten"]);
                                baiTap.noi_dung = Convert.ToString(reader["noi_dung"]);
                                baiTap.thoi_han = Convert.ToDateTime(reader["thoi_han"]);
                                baiTap.id_giang_vien = Convert.ToInt32(reader["id_giang_vien"]);
                                baiTap.id_muc = Convert.ToInt32(reader["id_muc"]);
                                BaiTapList.Add(baiTap);
                            }
                        }
                    }
                }

                return BaiTapList;
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
                                    id_bai_tap = Convert.ToInt32(reader["id_bai_tap"]),
                                    trang_thai = Convert.ToInt32(reader["trang_thai"]),
                                    ten = Convert.ToString(reader["ten"]),
                                    noi_dung = Convert.ToString(reader["noi_dung"]),
                                    thoi_han = Convert.ToDateTime(reader["thoi_han"]),
                                    id_giang_vien = Convert.ToInt32(reader["id_giang_vien"]),
                                    id_muc = Convert.ToInt32(reader["id_muc"])
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
                                State = true,
                                Message = "Thêm bài tập thành công",
                                InsertedId = insertedId,
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
                                State = true,
                                Message = "Cập nhật thông tin bài tập thành công",
                                InsertedId = null,
                                EffectedRows = effectedRows
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
                                State = true,
                                Message = "Cập nhật thông tin bài tập thành công",
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
