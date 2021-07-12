using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SimpleAds.Data
{
    public class Ad
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PhoneNumber { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }

    public class AdDb
    {
        private readonly string _connectionString;

        public AdDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(User user, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash) " +
                "Values (@name, @email, @passwordHash)";
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public List<Ad> GetAdds()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT a.*, u.Name FROM Ads a JOIN Users u ON u.Id = a.UserId";
            connection.Open();
            var reader = cmd.ExecuteReader();
            List<Ad> ads = new();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    Title = (string)reader["Title"],
                    Description = (string)reader["Description"],
                    PhoneNumber = (int)reader["PhoneNumber"],
                    UserName = (string)reader["Name"]
                });
            };
                return ads;
            }

        public User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            return new User
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Email = (string)reader["Email"],
                PasswordHash = (string)reader["PasswordHash"]
            };
        }

        public User LogIn(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
            bool IsValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return IsValid ? user : null;
        }
        public int GetUserId(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT UserId From Ads WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            return (int)cmd.ExecuteScalar();
        }

        public void NewAd(Ad ad)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Ads (Title, Description, PhoneNumber, UserId) " +
                "VALUES (@title, @description, @phoneNumber, @userId)";
            cmd.Parameters.AddWithValue("@title", ad.Title);
            cmd.Parameters.AddWithValue("@description", ad.Description);
            cmd.Parameters.AddWithValue("@phoneNumber", ad.PhoneNumber);
            cmd.Parameters.AddWithValue("@userId", ad.UserId);
            connection.Open();
            cmd.ExecuteNonQuery();

        }

        public void Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE from Ads WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public List<Ad> GetAddsForUser(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT a.*, u.Name FROM Ads a JOIN Users u ON u.Id = a.UserId " +
                             "WHERE a.UserId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();
            List<Ad> ads = new();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    Title = (string)reader["Title"],
                    Description = (string)reader["Description"],
                    PhoneNumber = (int)reader["PhoneNumber"],
                    UserName = (string)reader["Name"]
                });
            };
            return ads;
        }

    }
    }

