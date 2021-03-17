using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace imagesharepassword.Data
{
    public class ShareImagesDB
    {
        private readonly string _connectionString;
        public ShareImagesDB(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddImage(Image i)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Images(FileName,Views,Password)
                                    VALUES (@fileName,0,@password)
                                    SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@fileName", i.ImageName);
            command.Parameters.AddWithValue("@password", i.Password);
            connection.Open();
            i.Id=(int)(decimal)command.ExecuteScalar();
        }
        public Image GetImage(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Images WHERE id=@id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = command.ExecuteReader();
            reader.Read();
            Image i = new Image()
            {
                Id = id,
                ImageName = (string)reader["FileName"],
                Password = (string)reader["Password"],
                Views = (int)reader["views"]
            };                          
            return i;
        }
        public void UpdateViews(Image i)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE Images SET Views= Views+1 WHERE id=@id";
            command.Parameters.AddWithValue("@id", i.Id);
            connection.Open();
            command.ExecuteNonQuery();
            i.Views++;
        }

    }
}
