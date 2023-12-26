using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class PhongModel
    {
        public int id_phong { get; set; }
    }

    public class PhongRepository
    {
        private readonly string connectionString;

        public PhongRepository()
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

        public List<PhongModel> GetAllPhong()
        {
            List<PhongModel> phongList = new List<PhongModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM phong";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PhongModel phong = new PhongModel
                            {
                                id_phong = Convert.ToInt32(reader["id_phong"])
                            };

                            phongList.Add(phong);
                        }
                    }
                }
            }

            return phongList;
        }

        public PhongModel? GetPhongById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM phong WHERE id_phong = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new PhongModel
                            {
                                id_phong = Convert.ToInt32(reader["id_phong"])
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

        public Response InsertPhong(PhongModel phong)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO phong (id_phong) " +
                                   "VALUES (@IdPhong); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdPhong", phong.id_phong);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            State = true,
                            Message = "Thêm phòng thành công",
                            InsertedId = insertedId,
                        };
                    }
                }
            });
        }

        public Response UpdatePhong(PhongModel phong)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE phong SET id_phong = @IdPhong " +
                                   "WHERE id_phong = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdPhong", phong.id_phong);
                        command.Parameters.AddWithValue("@Id", phong.id_phong);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Cập nhật thông tin phòng thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
                        };
                    }
                }
            });
        }

        public Response DeletePhong(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM phong WHERE id_phong = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Xóa phòng thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }
}
