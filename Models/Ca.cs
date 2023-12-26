using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class CaModel
    {
        public int id_ca { get; set; }
        public DateTime? thoi_gian_bat_dau { get; set; }
        public DateTime? thoi_gian_ket_thuc { get; set; }
    }

    public class CaRepository
    {
        private readonly string connectionString;

        public CaRepository()
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

        public List<CaModel> GetAllCa()
        {
            List<CaModel> caList = new List<CaModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM ca";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CaModel ca = new CaModel
                            {
                                id_ca = Convert.ToInt32(reader["id_ca"]),
                                thoi_gian_bat_dau = reader["thoi_gian_bat_dau"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["thoi_gian_bat_dau"])
                                    : (DateTime?)null,
                                thoi_gian_ket_thuc = reader["thoi_gian_ket_thuc"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["thoi_gian_ket_thuc"])
                                    : (DateTime?)null
                            };

                            caList.Add(ca);
                        }
                    }
                }
            }

            return caList;
        }

        public CaModel? GetCaById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM ca WHERE id_ca = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new CaModel
                            {
                                id_ca = Convert.ToInt32(reader["id_ca"]),
                                thoi_gian_bat_dau = reader["thoi_gian_bat_dau"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["thoi_gian_bat_dau"])
                                    : (DateTime?)null,
                                thoi_gian_ket_thuc = reader["thoi_gian_ket_thuc"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["thoi_gian_ket_thuc"])
                                    : (DateTime?)null
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

        public Response InsertCa(CaModel ca)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO ca (thoi_gian_bat_dau, thoi_gian_ket_thuc) " +
                                   "VALUES (@ThoiGianBatDau, @ThoiGianKetThuc); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ThoiGianBatDau", ca.thoi_gian_bat_dau);
                        command.Parameters.AddWithValue("@ThoiGianKetThuc", ca.thoi_gian_ket_thuc);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            State = true,
                            Message = "Thêm ca thành công",
                            InsertedId = insertedId,
                        };
                    }
                }
            });
        }

        public Response UpdateCa(CaModel ca)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE ca SET thoi_gian_bat_dau = @ThoiGianBatDau, " +
                                   "thoi_gian_ket_thuc = @ThoiGianKetThuc " +
                                   "WHERE id_ca = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ThoiGianBatDau", ca.thoi_gian_bat_dau);
                        command.Parameters.AddWithValue("@ThoiGianKetThuc", ca.thoi_gian_ket_thuc);
                        command.Parameters.AddWithValue("@Id", ca.id_ca);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Cập nhật thông tin ca thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
                        };
                    }
                }
            });
        }

        public Response DeleteCa(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM ca WHERE id_ca = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Xóa ca thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }
}
