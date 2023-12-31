﻿using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class BaiNopModel
    {
        public int? id_bai_nop { get; set; }
        public DateTime? thoi_gian_nop { get; set; }
        public int? id_bai_tap { get; set; }
        public int? id_hoc_vien { get; set; }
    }


    // Lớp BaiNopRepository chứa các hàm thao tác với cơ sở dữ liệu
    public class BaiNopRepository
    {
        private readonly string connectionString;

        public BaiNopRepository()
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

        // Trả về List<BaiNopModel>
        public List<BaiNopModel> GetAllBaiNop()
        {
            List<BaiNopModel> baiNopList = new List<BaiNopModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM bai_nop";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BaiNopModel baiNop = new BaiNopModel();
                            baiNop.id_bai_nop = reader["id_bai_nop"] != DBNull.Value ? Convert.ToInt32(reader["id_bai_nop"]) : (int?)null;
                            baiNop.thoi_gian_nop = reader["thoi_gian_nop"] != DBNull.Value ? Convert.ToDateTime(reader["thoi_gian_nop"]) : (DateTime?)null;
                            baiNop.id_bai_tap = reader["id_bai_tap"] != DBNull.Value ? Convert.ToInt32(reader["id_bai_tap"]) : (int?)null;
                            baiNop.id_hoc_vien = reader["id_hoc_vien"] != DBNull.Value ? Convert.ToInt32(reader["id_hoc_vien"]) : (int?)null;
                            baiNopList.Add(baiNop);
                        }
                    }
                }
            }
                
            return baiNopList;
        }

        // Trả về 1 BaiNopModel
        public BaiNopModel? GetBaiNopById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM bai_nop WHERE id_bai_nop = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new BaiNopModel // Trả về 1 BaiNopModel
                            {
                                id_bai_nop = reader["id_bai_nop"] != DBNull.Value ? Convert.ToInt32(reader["id_bai_nop"]) : (int?)null,
                                thoi_gian_nop = reader["thoi_gian_nop"] != DBNull.Value ? Convert.ToDateTime(reader["thoi_gian_nop"]) : (DateTime?)null,
                                id_bai_tap = reader["id_bai_tap"] != DBNull.Value ? Convert.ToInt32(reader["id_bai_tap"]) : (int?)null,
                                id_hoc_vien = reader["id_hoc_vien"] != DBNull.Value ? Convert.ToInt32(reader["id_hoc_vien"]) : (int?)null
                            };
                        }
                        else
                            return null; // Không có trả về null
                    }
                }
            }
        }

        // Trả về Response
        public Response InsertBaiNop(BaiNopModel BaiNop)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO bai_nop (thoi_gian_nop, id_bai_tap, id_hoc_vien) " +
                                    "VALUES (@thoi_gian_nop, @id_bai_tap, @id_hoc_vien); " +
                                    "SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@thoi_gian_nop", BaiNop.thoi_gian_nop);
                        command.Parameters.AddWithValue("@id_bai_tap", BaiNop.id_bai_tap);
                        command.Parameters.AddWithValue("@id_hoc_vien", BaiNop.id_hoc_vien);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            state = true,
                            message = "Thêm bài nộp thành công",
                            insertedId = insertedId,
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response UpdateBaiNop(BaiNopModel BaiNop)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE bai_nop " +
                                    "SET thoi_gian_nop = @thoi_gian_nop " +
                                        "id_bai_tap = @id_bai_tap " +
                                        "id_hoc_vien = @id_hoc_vien " +
                                    "WHERE id_bai_nop = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@thoi_gian_nop", BaiNop.thoi_gian_nop);
                        command.Parameters.AddWithValue("@id_bai_tap", BaiNop.id_bai_tap);
                        command.Parameters.AddWithValue("@id_hoc_vien", BaiNop.id_hoc_vien);
                        command.Parameters.AddWithValue("@Id", BaiNop.id_bai_nop);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật thông tin bài nộp thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response DeleteBaiNop(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM bai_nop WHERE id_bai_nop = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            state = true,
                            message = "Cập nhật thông tin bài nộp thành công",
                            insertedId = null,
                            effectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }
}
