using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class TepTinTaiLenModel
    {
        public int? id_tep_tin_tai_len { get; set; }
        public string decoded { get; set; }
        public DateTime? ngay_tai_len { get; set; }
        public int? id_user { get; set; }
        public string ten_tep { get; set; }
        public string extension { get; set; }
    }
    public class TepTinTaiLenRepository
    {
        private readonly string connectionString;

        public TepTinTaiLenRepository()
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

        public List<TepTinTaiLenModel> GetAllTepTinTaiLen()
        {
            List<TepTinTaiLenModel> tepTinTaiLenList = new List<TepTinTaiLenModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM tep_tin_tai_len";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TepTinTaiLenModel tepTinTaiLen = new TepTinTaiLenModel
                            {
                                id_tep_tin_tai_len = reader["id_tep_tin_tai_len"] != DBNull.Value ? Convert.ToInt32(reader["id_tep_tin_tai_len"]) : (int?)null,
                                decoded = reader["decoded"] != DBNull.Value ? Convert.ToString(reader["decoded"]) : null,
                                ngay_tai_len = reader["ngay_tai_len"] != DBNull.Value ? Convert.ToDateTime(reader["ngay_tai_len"]) : (DateTime?)null,
                                id_user = reader["id_user"] != DBNull.Value ? Convert.ToInt32(reader["id_user"]) : (int?)null,
                                ten_tep = reader["ten_tep"] != DBNull.Value ? Convert.ToString(reader["ten_tep"]) : null,
                                extension = reader["extension"] != DBNull.Value ? Convert.ToString(reader["extension"]) : null
                            };
                            tepTinTaiLenList.Add(tepTinTaiLen);
                        }
                    }
                }
            }

            return tepTinTaiLenList;
        }
        public List<TepTinTaiLenModel> GetTepTinTaiLenByUserId(int userid)
        {
            List<TepTinTaiLenModel> tepTinTaiLenList = new List<TepTinTaiLenModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM tep_tin_tai_len where id_user = @Id order by ngay_tai_len desc";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", userid);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TepTinTaiLenModel tepTinTaiLen = new TepTinTaiLenModel
                            {
                                id_tep_tin_tai_len = reader["id_tep_tin_tai_len"] != DBNull.Value ? Convert.ToInt32(reader["id_tep_tin_tai_len"]) : (int?)null,
                                decoded = reader["decoded"] != DBNull.Value ? Convert.ToString(reader["decoded"]) : null,
                                ngay_tai_len = reader["ngay_tai_len"] != DBNull.Value ? Convert.ToDateTime(reader["ngay_tai_len"]) : (DateTime?)null,
                                id_user = reader["id_user"] != DBNull.Value ? Convert.ToInt32(reader["id_user"]) : (int?)null,
                                ten_tep = reader["ten_tep"] != DBNull.Value ? Convert.ToString(reader["ten_tep"]) : null,
                                extension = reader["extension"] != DBNull.Value ? Convert.ToString(reader["extension"]) : null
                            };
                            tepTinTaiLenList.Add(tepTinTaiLen);
                        }
                    }
                }
            }

            return tepTinTaiLenList;
        }

        public TepTinTaiLenModel? GetTepTinTaiLenById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM tep_tin_tai_len WHERE id_tep_tin_tai_len = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new TepTinTaiLenModel
                            {
                                id_tep_tin_tai_len = reader["id_tep_tin_tai_len"] != DBNull.Value ? Convert.ToInt32(reader["id_tep_tin_tai_len"]) : (int?)null,
                                decoded = reader["decoded"] != DBNull.Value ? Convert.ToString(reader["decoded"]) : null,
                                ngay_tai_len = reader["ngay_tai_len"] != DBNull.Value ? Convert.ToDateTime(reader["ngay_tai_len"]) : (DateTime?)null,
                                id_user = reader["id_user"] != DBNull.Value ? Convert.ToInt32(reader["id_user"]) : (int?)null,
                                ten_tep = reader["ten_tep"] != DBNull.Value ? Convert.ToString(reader["ten_tep"]) : null,
                                extension = reader["extension"] != DBNull.Value ? Convert.ToString(reader["extension"]) : null
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

        public Response InsertTepTinTaiLen(TepTinTaiLenModel tepTinTaiLen)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO tep_tin_tai_len (decoded, ngay_tai_len, id_user, ten_tep, extension) " +
                                   "VALUES (@decoded, @ngay_tai_len, @id_user, @ten_tep, @extension); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@decoded", tepTinTaiLen.decoded);
                        command.Parameters.AddWithValue("@ngay_tai_len", tepTinTaiLen.ngay_tai_len);
                        command.Parameters.AddWithValue("@id_user", tepTinTaiLen.id_user);
                        command.Parameters.AddWithValue("@ten_tep", tepTinTaiLen.ten_tep);
                        command.Parameters.AddWithValue("@extension", tepTinTaiLen.extension);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            state = true,
                            message = "Thêm tệp tin tải lên thành công",
                            insertedId = insertedId,
                        };
                    }
                }
            });
        }

        public Response UpdateTepTinTaiLen(TepTinTaiLenModel tepTinTaiLen)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE tep_tin_tai_len SET decoded = @decoded, " +
                                   "ngay_tai_len = @ngay_tai_len, id_user = @id_user, " +
                                   "ten_tep = @ten_tep, extension = @extension " +
                                   "WHERE id_tep_tin_tai_len = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@decoded", tepTinTaiLen.decoded);
                        command.Parameters.AddWithValue("@ngay_tai_len", tepTinTaiLen.ngay_tai_len);
                        command.Parameters.AddWithValue("@id_user", tepTinTaiLen.id_user);
                        command.Parameters.AddWithValue("@ten_tep", tepTinTaiLen.ten_tep);
                        command.Parameters.AddWithValue("@extension", tepTinTaiLen.extension);
                        command.Parameters.AddWithValue("@Id", tepTinTaiLen.id_tep_tin_tai_len);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật thông tin tệp tin tải lên thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }

        public Response DeleteTepTinTaiLen(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM tep_tin_tai_len WHERE id_tep_tin_tai_len = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Xóa tệp tin tải lên thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }
}
