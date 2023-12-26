using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class DuongLinkModel
    {
        public int id_duong_link { get; set; }
        public string link { get; set; }
        public DateTime? ngay_dang { get; set; }
        public int id_giang_vien { get; set; }
        public int id_muc { get; set; }
        public string tieu_de { get; set; }
    }

    public class DuongLinkRepository
    {
        private readonly string connectionString;

        public DuongLinkRepository()
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

        public List<DuongLinkModel> GetAllDuongLink()
        {
            List<DuongLinkModel> duongLinkList = new List<DuongLinkModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM duong_link";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DuongLinkModel duongLink = new DuongLinkModel
                            {
                                id_duong_link = Convert.ToInt32(reader["id_duong_link"]),
                                link = reader["link"].ToString(),
                                ngay_dang = reader["ngay_dang"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["ngay_dang"])
                                    : (DateTime?)null,
                                id_giang_vien = Convert.ToInt32(reader["id_giang_vien"]),
                                id_muc = Convert.ToInt32(reader["id_muc"]),
                                tieu_de = reader["tieu_de"].ToString()
                            };

                            duongLinkList.Add(duongLink);
                        }
                    }
                }
            }

            return duongLinkList;
        }

        public Response InsertDuongLink(DuongLinkModel duongLink)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO duong_link (link, ngay_dang, id_giang_vien, id_muc, tieu_de) 
                                     VALUES (@Link, @NgayDang, @IdGiangVien, @IdMuc, @TieuDe); 
                                     SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Link", duongLink.link);
                        command.Parameters.AddWithValue("@NgayDang", duongLink.ngay_dang);
                        command.Parameters.AddWithValue("@IdGiangVien", duongLink.id_giang_vien);
                        command.Parameters.AddWithValue("@IdMuc", duongLink.id_muc);
                        command.Parameters.AddWithValue("@TieuDe", duongLink.tieu_de);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            State = true,
                            Message = "Thêm đường link thành công",
                            InsertedId = insertedId,
                        };
                    }
                }
            });
        }

        public Response UpdateDuongLink(DuongLinkModel duongLink)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"UPDATE duong_link SET link = @Link, ngay_dang = @NgayDang, 
                                     id_giang_vien = @IdGiangVien, id_muc = @IdMuc, tieu_de = @TieuDe
                                     WHERE id_duong_link = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Link", duongLink.link);
                        command.Parameters.AddWithValue("@NgayDang", duongLink.ngay_dang);
                        command.Parameters.AddWithValue("@IdGiangVien", duongLink.id_giang_vien);
                        command.Parameters.AddWithValue("@IdMuc", duongLink.id_muc);
                        command.Parameters.AddWithValue("@TieuDe", duongLink.tieu_de);
                        command.Parameters.AddWithValue("@Id", duongLink.id_duong_link);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Cập nhật thông tin đường link thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
                        };
                    }
                }
            });
        }

        public DuongLinkModel? GetDuongLinkById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM duong_link WHERE id_duong_link = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new DuongLinkModel
                            {
                                id_duong_link = Convert.ToInt32(reader["id_duong_link"]),
                                link = reader["link"].ToString(),
                                ngay_dang = reader["ngay_dang"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["ngay_dang"])
                                    : (DateTime?)null,
                                id_giang_vien = Convert.ToInt32(reader["id_giang_vien"]),
                                id_muc = Convert.ToInt32(reader["id_muc"]),
                                tieu_de = reader["tieu_de"].ToString()
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

        public Response DeleteDuongLink(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM duong_link WHERE id_duong_link = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Xóa đường link thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }
}
