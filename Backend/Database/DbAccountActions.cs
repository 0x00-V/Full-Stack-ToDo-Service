using Microsoft.Data.Sqlite;
using BCrypt.Net;
using System.Text.RegularExpressions;

namespace api.dbactions
{
     public static class UsernameValidator
    {
        private static readonly Regex _regex =
            new(@"^[a-zA-Z0-9_]{3,32}$", RegexOptions.Compiled);
        public static bool IsValid(string username)
            => _regex.IsMatch(username);
    }
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }
        public static bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }

    public static class AccountManagement
    {
        public static bool CreateUserAccount(string username, string password)
        {
            try
            {
                using var connection = new SqliteConnection("Data Source=database.db");
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = """
                INSERT INTO users (username, password) VALUES ($username, $password);
                """;
                var hashedPassword = PasswordHasher.Hash(password);
                command.Parameters.AddWithValue("$username", username);
                command.Parameters.AddWithValue("$password", hashedPassword);
                command.ExecuteNonQuery();
                return true;
            } catch(SqliteException err)
            {
                Console.WriteLine($"There was an error in AccountManagement.CreateUserAccount():\n{err}");
                return false;
            }
        }

        public static bool UserAccountLogin(string username, string password)
        {
            try
            {
                using var connection = new SqliteConnection("Data Source=database.db");
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT password
                    FROM users
                    WHERE username = $username;
                """;
                command.Parameters.AddWithValue("$username", username);
                using var reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    return false;
                }
                var storedHash = reader.GetString(0);
                return PasswordHasher.Verify(password, storedHash);
            }
            catch (SqliteException err)
            {
                Console.WriteLine($"There was an error in AccountManagement.UserAccountLogin():\n{err}");
                return false;
            }
        }
    }
}