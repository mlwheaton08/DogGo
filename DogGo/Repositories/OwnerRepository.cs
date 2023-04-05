using DogGo.Models;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Plugins;
using System.Data.Common;

namespace DogGo.Repositories;

public class OwnerRepository : IOwnerRepository
{
    private readonly IConfiguration _config;

    public OwnerRepository(IConfiguration config)
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

    public List<Owner> GetAllOwners()
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT Id, Email, [Name], Address, NeighborhoodId, Phone
                                    FROM Owner";

                SqlDataReader reader = cmd.ExecuteReader();

                List<Owner> owners = new List<Owner>();
                while (reader.Read())
                {
                    Owner owner = new Owner()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Address = reader.GetString(reader.GetOrdinal("Address")),
                        Phone = reader.GetString(reader.GetOrdinal("Phone")),
                        NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                    };

                    owners.Add(owner);
                }

                reader.Close();
                return owners;
            }
        }
    }

    public Owner GetOwnerById(int id)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT
	                                    o.Id,
	                                    o.Email,
	                                    o.Name,
	                                    o.Address,
	                                    o.NeighborhoodId,
	                                    o.Phone,
	                                    d.Id as DogId,
	                                    d.Name as Dog,
	                                    d.Breed
                                    FROM Owner o
                                    JOIN Dog d
	                                    ON o.Id = d.OwnerId
                                    WHERE o.Id = @id";

                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                Owner owner = null;
                while (reader.Read())
                {
                    if (owner == null)
                    {
                        owner = new Owner()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Dogs = new List<Dog>()
                        };
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("DogId")))
                    {
                        owner.Dogs.Add(new Dog()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("DogId")),
                            Name = reader.GetString(reader.GetOrdinal("Dog")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed"))
                        });
                    }
                }

                reader.Close();
                return owner;
            }
        }
    }
}