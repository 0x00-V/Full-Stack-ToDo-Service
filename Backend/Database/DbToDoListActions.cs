using api.Models;
using Microsoft.Data.Sqlite;

namespace api.dbactions
{
    public static class ToDoActions
    {
        public static bool CreateEntry(string username, CreateToDoItem item)
        {
            using var connection = new SqliteConnection("Data Source=database.db");
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = """
            SELECT userId FROM users WHERE username = $username LIMIT 1;
            """;
            command.Parameters.AddWithValue("$username", username);
            var result = command.ExecuteScalar();
            if (result == null) return false;
            int userId =  Convert.ToInt32(result);
            command.Parameters.Clear();
            command.CommandText = """
            INSERT INTO todoitems (userId, title, description, created_at)
            VALUES ($userId, $title, $description, $created_at);
            """;
            command.Parameters.AddWithValue("$userId", userId);
            command.Parameters.AddWithValue("$title", item.Title);
            command.Parameters.AddWithValue("$description", item.Description);
            command.Parameters.AddWithValue("$created_at", DateTime.UtcNow);
            command.ExecuteNonQuery();
            return true;
        }

    
        public static List<ToDoItem> GetItems(string username)
        {
            try
            {
                using var connection = new SqliteConnection("Data Source=database.db");
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT userId
                    FROM users
                    WHERE username = $username
                    LIMIT 1;
                """;
                command.Parameters.AddWithValue("$username", username);
                var result = command.ExecuteScalar();
                if (result == null)
                    return new List<ToDoItem>();
                int userId = Convert.ToInt32(result);
                command.Parameters.Clear();
                command.CommandText = """
                    SELECT ItemId, userId, title, description, created_at
                    FROM todoitems
                    WHERE userId = $userId;
                """;
                command.Parameters.AddWithValue("$userId", userId);
                using var reader = command.ExecuteReader();
                var items = new List<ToDoItem>();
                while (reader.Read())
                {
                    items.Add(new ToDoItem
                    {
                        ItemId = reader.GetInt32(0),
                        UserId = reader.GetInt32(1),
                        Title = reader.GetString(2),
                        Description = reader.IsDBNull(3) ? null! : reader.GetString(3),
                        CreatedAt = reader.GetDateTime(4)
                    });
                }
                return items;
            }
            catch (SqliteException)
            {
                return new List<ToDoItem>();
            }
        }

    }
}


/*

ToDoItems.cs Ref
namespace api.Models
{
    public class ToDoItem
    {
        public int ItemId {get;set;}
        //public required int Owner {get;set;}
        public required string Title {get;set;}
        public string Description {get;set;} = default!;
        public bool Completed {get;set;} = false;
    }
}
*/