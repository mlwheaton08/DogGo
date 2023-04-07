using DogGo.Models;
using Microsoft.Data.SqlClient;

namespace DogGo.Repositories;

public class DogRepository : IDogRepository
{
    private readonly IConfiguration _config;

    public DogRepository(IConfiguration config)
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

    public List<Dog> GetAllDogs()
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT Id, [Name], Breed, OwnerId
                                    FROM Dog";

                SqlDataReader reader = cmd.ExecuteReader();

                List<Dog> dogs = new List<Dog>();
                while (reader.Read())
                {
                    Dog dog = new Dog()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Breed = reader.GetString(reader.GetOrdinal("Breed")),
                        OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId"))
                    };

                    dogs.Add(dog);
                }

                reader.Close();
                return dogs;
            }
        }
    }

    public void AddDog(Dog dog)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Dog ([Name], Breed, OwnerId)
                                    OUTPUT INSERTED.ID
                                    VALUES (@name, @breed, @ownerId);";

                cmd.Parameters.AddWithValue("@name", dog.Name);
                cmd.Parameters.AddWithValue("@breed", dog.Breed);
                cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);

                int id = (int)cmd.ExecuteScalar();

                dog.Id = id;
            }
        }
    }

    public void UpdateDog(Dog dog)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                            UPDATE Owner
                            SET 
                                [Name] = @name, 
                                Breed = @breed, 
                                OwnerId = @ownerId
                            WHERE Id = @id";

                cmd.Parameters.AddWithValue("@name", dog.Name);
                cmd.Parameters.AddWithValue("@breed", dog.Breed);
                cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);
                cmd.Parameters.AddWithValue("@id", dog.Id);

                cmd.ExecuteNonQuery();
            }
        }
    }

    public void DeleteDog(int dogId) { }
}
