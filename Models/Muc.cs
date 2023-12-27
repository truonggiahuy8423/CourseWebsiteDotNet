using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    // Lớp MucModel chứa các thuộc tính
    public class MucModel
    {
        public int id_muc { get; set; }
        public string? ten_muc { get; set; }
        public int id_lop_hoc { get; set; }
        public int? id_muc_cha { get; set; }
    }


    // Lớp MucRepository chứa các hàm thao tác với cơ sở dữ liệu
    public class MucRepository
    {
        private readonly string connectionString;

        public MucRepository()
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

        // Trả về List<MucModel>
        public List<MucModel> GetAllMuc()
        {
            List<MucModel> mucList = new List<MucModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM muc";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MucModel muc = new MucModel();
                            muc.id_muc = Convert.ToInt32(reader["id_muc"]);
                            muc.ten_muc = (reader["ten_muc"]) != DBNull.Value ? Convert.ToString(reader["ten_muc"]) : (string?)null;
                            muc.id_lop_hoc = Convert.ToInt32(reader["id_lop_hoc"]);
                            muc.id_muc_cha = (reader["id_muc_cha"]) != DBNull.Value ? Convert.ToInt32(reader["id_muc_cha"]) : (int?)null;
                            mucList.Add(muc);
                        }
                    }
                }
            }

            return mucList;
        }

        // Trả về 1 MucModel
        public MucModel? GetMucById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM muc WHERE id_muc = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new MucModel // Trả về 1 MucModel
                            {
                                id_muc = Convert.ToInt32(reader["id_muc"]),
                                ten_muc = (reader["ten_muc"]) != DBNull.Value ? Convert.ToString(reader["ten_muc"]) : (string?)null,
                                id_lop_hoc = Convert.ToInt32(reader["id_lop_hoc"]),
                                id_muc_cha = (reader["id_muc_cha"]) != DBNull.Value ? Convert.ToInt32(reader["id_muc_cha"]) : (int?)null
                            };
                        }
                        else
                            return null; // Không có trả về null
                    }
                }
            }
        }

        // Trả về Response
        public Response InsertMuc(MucModel muc)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO muc (ten_muc, id_lop_hoc, id_muc_cha) " +
                                   "VALUES (@ten_muc, @id_lop_hoc, @id_muc_cha); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ten_muc", muc.ten_muc);
                        command.Parameters.AddWithValue("@id_lop_hoc", muc.id_muc);
                        command.Parameters.AddWithValue("@id_muc_cha", muc.id_muc_cha);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            state = true,
                            message = "Thêm mục thành công",
                            insertedId = insertedId,
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response UpdateMuc(MucModel muc)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE muc SET ten_muc = @ten_muc, " +
                                   "id_lop_hoc = @id_lop_hoc, id_muc_cha = @id_muc_cha " +
                    "WHERE id_muc = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ten_muc", muc.ten_muc);
                        command.Parameters.AddWithValue("@id_lop_hoc", muc.id_lop_hoc);
                        command.Parameters.AddWithValue("@id_muc_cha", muc.id_muc_cha);
                        command.Parameters.AddWithValue("@Id", muc.id_muc);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật thông tin mục thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response DeleteMuc(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM muc WHERE id_muc = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật thông tin mục thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }
}
