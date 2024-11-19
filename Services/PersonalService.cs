using Module07Data_Access.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Module07Data_Access.Services
{
    public class PersonalService
    {
        private readonly string _connectionString;

        public PersonalService()
        {
            var dbService = new DatabaseConnectionService();
            _connectionString = dbService.GetConnectionString();
        }

        public async Task<List<Personal>> GetAllPersonalsAsync()
        {
            var personalList = new List<Personal>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var cmd = new MySqlCommand("SELECT * FROM tblPersonal", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        personalList.Add(new Personal
                        {
                            ID = reader.GetInt32("ID"),
                            Name = reader.GetString("Name"),
                            Gender = reader.GetString("Gender"),
                            ContactNo = reader.GetString("ContactNo")
                        });
                    }
                }
            }
            return personalList;
        }

        public async Task<bool> AddPersonalAsync(Personal newPerson)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("INSERT INTO tblPersonal (Name, Gender, ContactNo) VALUES (@Name, @Gender, @ContactNo)", conn);
                    cmd.Parameters.AddWithValue("@Name", newPerson.Name);
                    cmd.Parameters.AddWithValue("@Gender", newPerson.Gender);
                    cmd.Parameters.AddWithValue("@ContactNo", newPerson.ContactNo);

                    var result = await cmd.ExecuteNonQueryAsync();

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding personal record: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeletePersonalAsync(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("DELETE FROM tblPersonal WHERE ID = @ID", conn);
                    cmd.Parameters.AddWithValue("@ID", id);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting personal record: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Personal>> SearchPersonalsAsync(string searchText)
        {
            var personalList = new List<Personal>();

            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var cmd = new MySqlCommand("SELECT * FROM tblPersonal WHERE Name LIKE @Search OR Gender LIKE @Search OR ContactNo LIKE @Search", conn);
                cmd.Parameters.AddWithValue("@Search", $"%{searchText}%");

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        personalList.Add(new Personal
                        {
                            ID = reader.GetInt32("ID"),
                            Name = reader.GetString("Name"),
                            Gender = reader.GetString("Gender"),
                            ContactNo = reader.GetString("ContactNo")
                        });
                    }
                }
            }

            return personalList;
        }

        public async Task<bool> UpdatePersonalAsync(Personal updatedPerson)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand(
                        "UPDATE tblPersonal SET Name = @Name, Gender = @Gender, ContactNo = @ContactNo WHERE ID = @ID", conn);

                    cmd.Parameters.AddWithValue("@Name", updatedPerson.Name);
                    cmd.Parameters.AddWithValue("@Gender", updatedPerson.Gender);
                    cmd.Parameters.AddWithValue("@ContactNo", updatedPerson.ContactNo);
                    cmd.Parameters.AddWithValue("@ID", updatedPerson.ID);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating personal record: {ex.Message}");
                return false;
            }
        }
    }
}
