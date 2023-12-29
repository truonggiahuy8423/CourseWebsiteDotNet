using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class ThongBaoModel
    {
        public int id_thong_bao { get; set; }
        public string? noi_dung { get; set; }
        public DateTime? ngay_dang { get; set; }
        public int id_giang_vien { get; set; }
        public int id_muc { get; set; }
        public string tieu_de { get; set; }
    }

    public class ThongBaoRepository
    {
        private readonly string connectionString;

        public ThongBaoRepository()
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

        public List<ThongBaoModel> GetAllThongBao()
        {
            List<ThongBaoModel> thongBaoList = new List<ThongBaoModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM thong_bao";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ThongBaoModel thongBao = new ThongBaoModel
                            {
                                id_thong_bao = Convert.ToInt32(reader["id_thong_bao"]),
                                noi_dung = reader["noi_dung"] != DBNull.Value ? reader["noi_dung"].ToString() : null,
                                ngay_dang = reader["ngay_dang"] != DBNull.Value ? Convert.ToDateTime(reader["ngay_dang"]) : (DateTime?)null,
                                id_giang_vien = Convert.ToInt32(reader["id_giang_vien"]),
                                id_muc = Convert.ToInt32(reader["id_muc"]),
                                tieu_de = reader["tieu_de"].ToString()
                            };

                            thongBaoList.Add(thongBao);
                        }
                    }
                }
            }

            return thongBaoList;
        }

        public DataTable GetThongBaoByCourseId(int id)
        {
            DataTable dataTable = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = $@"SELECT thong_bao.*, giang_vien.ho_ten 
                FROM thong_bao inner join muc on thong_bao.id_muc = muc.id_muc inner join giang_vien on giang_vien.id_giang_vien = thong_bao.id_giang_vien 
                where muc.id_lop_hoc = @Id order by thong_bao.ngay_dang ASC";

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

        public ThongBaoModel? GetThongBaoById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM thong_bao WHERE id_thong_bao = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ThongBaoModel
                            {
                                id_thong_bao = Convert.ToInt32(reader["id_thong_bao"]),
                                noi_dung = reader["noi_dung"] != DBNull.Value ? reader["noi_dung"].ToString() : null,
                                ngay_dang = reader["ngay_dang"] != DBNull.Value ? Convert.ToDateTime(reader["ngay_dang"]) : (DateTime?)null,
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

        public Response InsertThongBao(ThongBaoModel thongBao)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO thong_bao (noi_dung, ngay_dang, id_giang_vien, id_muc, tieu_de) 
                                     VALUES (@NoiDung, @NgayDang, @IdGiangVien, @IdMuc, @TieuDe); 
                                     SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NoiDung", thongBao.noi_dung ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@NgayDang", thongBao.ngay_dang ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IdGiangVien", thongBao.id_giang_vien);
                        command.Parameters.AddWithValue("@IdMuc", thongBao.id_muc);
                        command.Parameters.AddWithValue("@TieuDe", thongBao.tieu_de);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            state = true,
                            message = "Thêm thông báo thành công",
                            insertedId = insertedId,
                        };
                    }
                }
            });
        }

        public Response UpdateThongBao(ThongBaoModel thongBao)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"UPDATE thong_bao SET noi_dung = @NoiDung, ngay_dang = @NgayDang, 
                                     id_giang_vien = @IdGiangVien, id_muc = @IdMuc, tieu_de = @TieuDe
                                     WHERE id_thong_bao = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NoiDung", thongBao.noi_dung ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@NgayDang", thongBao.ngay_dang ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IdGiangVien", thongBao.id_giang_vien);
                        command.Parameters.AddWithValue("@IdMuc", thongBao.id_muc);
                        command.Parameters.AddWithValue("@TieuDe", thongBao.tieu_de);
                        command.Parameters.AddWithValue("@Id", thongBao.id_thong_bao);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật thông tin thông báo thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }

        public Response DeleteThongBao(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM thong_bao WHERE id_thong_bao = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Xóa thông báo thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }
}
