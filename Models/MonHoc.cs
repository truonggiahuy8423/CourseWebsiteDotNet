﻿using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class MonHocModel
    {
        public int id_mon_hoc { get; set; }
        public string? ten_mon_hoc { get; set; }
    }

    public class MonHocRepository
    {
        private readonly string connectionString;

        public MonHocRepository()
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

        public List<MonHocModel> GetAllMonHoc()
        {
            List<MonHocModel> monHocList = new List<MonHocModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM mon_hoc";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MonHocModel monHoc = new MonHocModel
                            {
                                id_mon_hoc = Convert.ToInt32(reader["id_mon_hoc"]),
                                ten_mon_hoc = reader["ten_mon_hoc"] != DBNull.Value ? reader["ten_mon_hoc"].ToString() : null
                            };

                            monHocList.Add(monHoc);
                        }
                    }
                }
            }

            return monHocList;
        }

        public MonHocModel? GetMonHocById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM mon_hoc WHERE id_mon_hoc = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new MonHocModel
                            {
                                id_mon_hoc = Convert.ToInt32(reader["id_mon_hoc"]),
                                ten_mon_hoc = reader["ten_mon_hoc"] != DBNull.Value ? reader["ten_mon_hoc"].ToString() : null
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

        public Response InsertMonHoc(MonHocModel monHoc)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO mon_hoc (ten_mon_hoc) " +
                                   "VALUES (@TenMonHoc); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TenMonHoc", monHoc.ten_mon_hoc);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());
                        if (insertedId > 0)
                        {
                            return new Response
                            {
                                state = true,
                                message = "Thêm môn học thành công",
                                insertedId = insertedId,
                            };
                        }
                        else
                        {
                            return new Response
                            {
                                state = false,
                                message = "Thêm môn học thất bại",
                                insertedId = insertedId,
                            };
                        }

                    }
                }
            });
        }

        public Response UpdateMonHoc(MonHocModel monHoc)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE mon_hoc SET ten_mon_hoc = @TenMonHoc " +
                                   "WHERE id_mon_hoc = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TenMonHoc", monHoc.ten_mon_hoc);
                        command.Parameters.AddWithValue("@Id", monHoc.id_mon_hoc);

                        int effectedRows = command.ExecuteNonQuery();
                        if (effectedRows > 0)
                        {
                            return new Response
                            {
                                state = true,
                                message = "Cập nhật thông tin môn học thành công",
                                insertedId = null,
                                effectedRows = effectedRows
                            };
                        }
                        else
                        {
                            return new Response
                            {
                                state = false,
                                message = "Cập nhật thông tin môn học thất bại",
                                insertedId = null,
                                effectedRows = effectedRows
                            };
                        }
                    }
                }
            });
        }

        public Response DeleteMonHoc(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM mon_hoc WHERE id_mon_hoc = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();
                        if (effectedRows > 0)
                        {
                            return new Response
                            {
                                state = true,
                                message = "Xóa môn học thành công",
                                insertedId = null,
                                effectedRows = effectedRows
                            };
                        }
                        else
                        {
                            return new Response
                            {
                                state = false,
                                message = "Xóa môn học thất bại",
                                insertedId = null,
                                effectedRows = effectedRows
                            };
                        }

                    }
                }
            });
        }
    }
}
