using DogGo.Models;
using Microsoft.Data.SqlClient;

namespace DogGo.Repositories;

public class WalksRepository : IWalksRepository
{
    private readonly IConfiguration _config;

    public WalksRepository(IConfiguration config)
    {
        _config = config;
    }

    public SqlConnection Connection
    {
        get
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }
    }

    public List<Walks> GetWalksByWalkerId(int walkerId)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT
	                                    Walker.Id as WalkerId,
	                                    w.Id as WalkId,
	                                    w.Date,
	                                    w.Duration,
	                                    d.Id as DogId,
	                                    d.Name as DogName,
	                                    d.OwnerId as DogOwnerId,
	                                    d.Breed as DogBreed
                                    FROM Walker
                                    JOIN Walks w
	                                    ON Walker.Id = w.WalkerId
                                    JOIN Dog d
	                                    ON w.DogId = d.Id
                                    WHERE w.WalkerId = @walkerId";

                cmd.Parameters.AddWithValue("@walkerId", walkerId);

                SqlDataReader reader = cmd.ExecuteReader();

                List<Walks> walks = new List<Walks>();

                while (reader.Read())
                {
                    Walks walk = new Walks()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("WalkId")),
                        Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                        Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                        WalkerId = walkerId,
                        DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                        Dog = new Dog()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("DogId")),
                            Name = reader.GetString(reader.GetOrdinal("DogName")),
                            Breed = reader.GetString(reader.GetOrdinal("DogBreed")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("DogOwnerId"))
                        }
                    };

                    walks.Add(walk);
                }
                reader.Close();
                return walks;
            }
        }
    }
}