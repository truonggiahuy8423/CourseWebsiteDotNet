using MySql.Data.MySqlClient;

namespace CourseWebsiteDotNet.Models
{
    public class AdModel
    {
        public int id_ad {  get; set; }
        public string? ho_ten { get; set; }
        public string email { get; set;}
    }
    public class AdRepository
    {
        private readonly string connectionString;
        public AdRepository()
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

        // Trả về List<AdModel>
        public List<AdModel> GetAllAd()
        {
            List<AdModel> adList = new List<AdModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM ad";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AdModel ad = new AdModel();
                            ad.id_ad = Convert.ToInt32(reader["id_ad"]);
                            ad.ho_ten = Convert.ToString(reader["ho_ten"]);
                            ad.email = Convert.ToString(reader["email"]);
                            adList.Add(ad);
                        }
                    }
                }
            }

            return adList;
        }

        // Trả về 1 AdModel
        public AdModel? GetAdById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM ad WHERE id_ad = @Id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new AdModel // Trả về 1 AdModel
                            {
                                id_ad = Convert.ToInt32(reader["id_ad"]),
                                ho_ten = Convert.ToString(reader["ho_ten"]),
                                email = Convert.ToString(reader["email"])
                            };
                        }
                        else
                            return null; // Không có trả về null
                    }
                }
            }
        }

        // Trả về Response
        public Response InsertAd(AdModel ad)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO ad (ho_ten, email) " +
                                   "VALUES (@ho_ten, @email); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ho_ten", ad.ho_ten);
                        command.Parameters.AddWithValue("@email", ad.email);

                        int insertedId = Convert.ToInt32(command.ExecuteScalar());

                        return new Response
                        {
                            State = true,
                            Message = "Thêm quản trị viên thành công",
                            InsertedId = insertedId,
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response UpdateAd(AdModel ad)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE ad SET ho_ten = @ho_ten, " +
                                   "email = @email " +
                    "WHERE id_ad = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ho_ten", ad.ho_ten);
                        command.Parameters.AddWithValue("@email", ad.email);
                        command.Parameters.AddWithValue("@Id", ad.id_ad);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Cập nhật thông tin quản trị viên thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
                        };
                    }
                }
            });
        }

        // Trả về Response
        public Response DeleteAd(int id)
        {
            return ExecuteDatabaseOperation(() =>
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM ad WHERE id_ad = @Id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int effectedRows = command.ExecuteNonQuery();

                        return new Response
                        {
                            State = true,
                            Message = "Cập nhật thông tin quản trị viên thành công",
                            InsertedId = null,
                            EffectedRows = effectedRows
                        };
                    }
                }
            });
        }
    }
}
