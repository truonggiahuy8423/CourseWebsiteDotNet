using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class ViTriTepTinModel
    {
        public int id_muc { get; set; }
        public int id_tep_tin_tai_len { get; set; }
        public DateTime? ngay_dang { get; set; }
    }

    public class ViTriTepTinRepository
    {
        private readonly string connectionString;

        public ViTriTepTinRepository()
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

        public List<ViTriTepTinModel> GetAllViTriTepTin()
        {
            List<ViTriTepTinModel> viTriTepTinList = new List<ViTriTepTinModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM vi_tri_tep_tin";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ViTriTepTinModel viTriTepTin = new ViTriTepTinModel
                            {
                                id_muc = Convert.ToInt32(reader["id_muc"]),
                                id_tep_tin_tai_len = Convert.ToInt32(reader["id_tep_tin_tai_len"]),
                                ngay_dang = reader["ngay_dang"] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader["ngay_dang"])
                            };

                            viTriTepTinList.Add(viTriTepTin);
                        }
                    }
                }
            }

            return viTriTepTinList;
        }

        public ViTriTepTinModel? GetViTriTepTin(int idMuc, int idTepTinTaiLen)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM vi_tri_tep_tin WHERE id_muc = @IdMuc AND id_tep_tin_tai_len = @IdTepTinTaiLen";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdMuc", idMuc);
                    command.Parameters.AddWithValue("@IdTepTinTaiLen", idTepTinTaiLen);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ViTriTepTinModel
                            {
                                id_muc = Convert.ToInt32(reader["id_muc"]),
                                id_tep_tin_tai_len = Convert.ToInt32(reader["id_tep_tin_tai_len"]),
                                ngay_dang = reader["ngay_dang"] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader["ngay_dang"])
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

        public Response InsertViTriTepTin(ViTriTepTinModel viTriTepTin)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO vi_tri_tep_tin (id_muc, id_tep_tin_tai_len, ngay_dang) " +
                                   "VALUES (@IdMuc, @IdTepTinTaiLen, @NgayDang);";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdMuc", viTriTepTin.id_muc);
                        command.Parameters.AddWithValue("@IdTepTinTaiLen", viTriTepTin.id_tep_tin_tai_len);
                        command.Parameters.AddWithValue("@NgayDang", viTriTepTin.ngay_dang);

                        int insertedId = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Thêm vị trí tệp tin thành công",
                            insertedId = insertedId,
                        };
                    }
                }
            });
        }

        public Response UpdateViTriTepTin(ViTriTepTinModel viTriTepTin)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE vi_tri_tep_tin SET ngay_dang = @NgayDang " +
                                   "WHERE id_muc = @IdMuc AND id_tep_tin_tai_len = @IdTepTinTaiLen";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdMuc", viTriTepTin.id_muc);
                        command.Parameters.AddWithValue("@IdTepTinTaiLen", viTriTepTin.id_tep_tin_tai_len);
                        command.Parameters.AddWithValue("@NgayDang", viTriTepTin.ngay_dang);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật vị trí tệp tin thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }

        public Response DeleteViTriTepTin(int idMuc, int idTepTinTaiLen)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM vi_tri_tep_tin WHERE id_muc = @IdMuc AND id_tep_tin_tai_len = @IdTepTinTaiLen;";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdMuc", idMuc);
                        command.Parameters.AddWithValue("@IdTepTinTaiLen", idTepTinTaiLen);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Xóa vị trí tệp tin thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }
}
