using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class ThongBaoModel
    {
        public int id_thong_bao { get; set; }
        public string noi_dung { get; set; }
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
                                noi_dung = reader["noi_dung"].ToString(),
                                ngay_dang = reader["ngay_dang"] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader["ngay_dang"]),
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
                                noi_dung = reader["noi_dung"].ToString(),
                                ngay_dang = reader["ngay_dang"] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader["ngay_dang"]),
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

                    string query = "INSERT INTO thong_bao (noi_dung, ngay_dang, id_giang_vien, id_muc, tieu_de) " +
                                   "VALUES (@NoiDung, @NgayDang, @IdGiangVien, @IdMuc, @TieuDe); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NoiDung", thongBao.noi_dung);
                        command.Parameters.AddWithValue("@NgayDang", thongBao.ngay_dang);
                        command.Parameters.AddWithValue("@IdGiangVien", thongBao.id_giang_vien);
                        command.Parameters.AddWithValue("@IdMuc", thongBao.id_muc);
                        command.Parameters.AddWithValue("@TieuDe", thongBao.tieu_de);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            State = true,
                            Message = "Th�m th�ng b�o th�nh c�ng",
                            InsertedId = insertedId,
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

                string query = "UPDATE thong_bao SET noi_dung = @NoiDung, " +
                               "ngay_dang = @NgayDang, id_giang_vien = @IdGiangVien, " +
                               "id_muc = @IdMuc, tieu_de = @TieuDe " +
                               "WHERE id_thong_bao = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NoiDung", thongBao.noi_dung);
                    command.Parameters.AddWithValue("@NgayD