using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class BuoiHocModel : IEquatable<BuoiHocModel>
    {
        
        public int? id_buoi_hoc { get; set; }
        public int? trang_thai { get; set; }
        public DateTime? ngay { get; set; }
        public int? id_lop_hoc { get; set; }
        public int? id_ca { get; set; }
        public int? id_phong { get; set; }

        public bool Equals(BuoiHocModel? other)
        {
            return this.id_buoi_hoc == other.id_buoi_hoc;
        }
    }
    public class BuoiHocRepository 
    {
        private readonly string connectionString;

        
        public BuoiHocRepository()
        {
            connectionString = DatabaseConnection.CONNECTION_STRING;
        }

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
        public List<BuoiHocModel> GetAllBuoiHocByCourseId(int courseid)
        {
            List<BuoiHocModel> buoiHocList = new List<BuoiHocModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM buoi_hoc where buoi_hoc.id_buoi_hoc = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", courseid);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BuoiHocModel buoiHoc = new BuoiHocModel
                            {
                                id_buoi_hoc = reader["id_buoi_hoc"] != DBNull.Value ? Convert.ToInt32(reader["id_buoi_hoc"]) : (int?)null,
                                trang_thai = reader["trang_thai"] != DBNull.Value ? Convert.ToInt32(reader["trang_thai"]) : (int?)null,
                                ngay = reader["ngay"] != DBNull.Value ? Convert.ToDateTime(reader["ngay"]) : (DateTime?)null,
                                id_lop_hoc = reader["id_lop_hoc"] != DBNull.Value ? Convert.ToInt32(reader["id_lop_hoc"]) : (int?)null,
                                id_ca = reader["id_ca"] != DBNull.Value ? Convert.ToInt32(reader["id_ca"]) : (int?)null,
                                id_phong = reader["id_phong"] != DBNull.Value ? Convert.ToInt32(reader["id_phong"]) : (int?)null
                            };
                            buoiHocList.Add(buoiHoc);
                        }
                    }
                }
            }

            return buoiHocList;
        }
        public List<BuoiHocModel> GetAllBuoiHoc()
        {
            List<BuoiHocModel> buoiHocList = new List<BuoiHocModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM buoi_hoc";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BuoiHocModel buoiHoc = new BuoiHocModel
                            {
                                id_buoi_hoc = reader["id_buoi_hoc"] != DBNull.Value ? Convert.ToInt32(reader["id_buoi_hoc"]) : (int?)null,
                                trang_thai = reader["trang_thai"] != DBNull.Value ? Convert.ToInt32(reader["trang_thai"]) : (int?)null,
                                ngay = reader["ngay"] != DBNull.Value ? Convert.ToDateTime(reader["ngay"]) : (DateTime?)null,
                                id_lop_hoc = reader["id_lop_hoc"] != DBNull.Value ? Convert.ToInt32(reader["id_lop_hoc"]) : (int?)null,
                                id_ca = reader["id_ca"] != DBNull.Value ? Convert.ToInt32(reader["id_ca"]) : (int?)null,
                                id_phong = reader["id_phong"] != DBNull.Value ? Convert.ToInt32(reader["id_phong"]) : (int?)null
                            };
                            buoiHocList.Add(buoiHoc);
                        }
                    }
                }
            }

            return buoiHocList;
        }

        public BuoiHocModel? GetBuoiHocById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM buoi_hoc WHERE id_buoi_hoc = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new BuoiHocModel
                            {
                                id_buoi_hoc = reader["id_buoi_hoc"] != DBNull.Value ? Convert.ToInt32(reader["id_buoi_hoc"]) : (int?)null,
                                trang_thai = reader["trang_thai"] != DBNull.Value ? Convert.ToInt32(reader["trang_thai"]) : (int?)null,
                                ngay = reader["ngay"] != DBNull.Value ? Convert.ToDateTime(reader["ngay"]) : (DateTime?)null,
                                id_lop_hoc = reader["id_lop_hoc"] != DBNull.Value ? Convert.ToInt32(reader["id_lop_hoc"]) : (int?)null,
                                id_ca = reader["id_ca"] != DBNull.Value ? Convert.ToInt32(reader["id_ca"]) : (int?)null,
                                id_phong = reader["id_phong"] != DBNull.Value ? Convert.ToInt32(reader["id_phong"]) : (int?)null
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public Response InsertBuoiHoc(BuoiHocModel buoiHoc)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO buoi_hoc (trang_thai, ngay, id_lop_hoc, id_ca, id_phong) " +
                                   "VALUES (@trang_thai, @ngay, @id_lop_hoc, @id_ca, @id_phong); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@trang_thai", buoiHoc.trang_thai);
                        command.Parameters.AddWithValue("@ngay", buoiHoc.ngay);
                        command.Parameters.AddWithValue("@id_lop_hoc", buoiHoc.id_lop_hoc);
                        command.Parameters.AddWithValue("@id_ca", buoiHoc.id_ca);
                        command.Parameters.AddWithValue("@id_phong", buoiHoc.id_phong);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            state = true,
                            message = "Thêm buổi học thành công",
                            insertedId = insertedId,
                        };
                    }
                }
            });
        }

        public Response UpdateBuoiHoc(BuoiHocModel buoiHoc)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE buoi_hoc SET trang_thai = @trang_thai, " +
                                   "ngay = @ngay, id_lop_hoc = @id_lop_hoc, id_ca = @id_ca, id_phong = @id_phong " +
                                   "WHERE id_buoi_hoc = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@trang_thai", buoiHoc.trang_thai);
                        command.Parameters.AddWithValue("@ngay", buoiHoc.ngay);
                        command.Parameters.AddWithValue("@id_lop_hoc", buoiHoc.id_lop_hoc);
                        command.Parameters.AddWithValue("@id_ca", buoiHoc.id_ca);
                        command.Parameters.AddWithValue("@id_phong", buoiHoc.id_phong);
                        command.Parameters.AddWithValue("@Id", buoiHoc.id_buoi_hoc);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật thông tin buổi học thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }

        public Response DeleteBuoiHoc(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM buoi_hoc WHERE id_buoi_hoc = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Xóa buổi học thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }

}
