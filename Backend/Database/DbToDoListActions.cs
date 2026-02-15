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
                    SELECT ItemId, userId, title, description, completed, created_at
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
                        Completed = reader.GetBoolean(4),
                        CreatedAt = reader.GetDateTime(5)
                    });
                }
                return items;
            }
            catch (SqliteException)
            {
                return new List<ToDoItem>();
            }
        }

        public static ToDoItem? GetItem(int item_id, string username)
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
                    return null;
                int userId = Convert.ToInt32(result);
                command.Parameters.Clear();
                command.CommandText = """
                    SELECT ItemId, userId, title, description, completed, created_at
                    FROM todoitems
                    WHERE ItemId = $itemId and userId = $userId;
                """;
                command.Parameters.AddWithValue("$itemId", item_id);
                command.Parameters.AddWithValue("$userId", userId);
                using var reader = command.ExecuteReader();
                if(!reader.Read()) return null;
                return  new ToDoItem
                {
                    ItemId = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    Title = reader.GetString(2),
                    Description = reader.IsDBNull(3) ? null! : reader.GetString(3),
                    Completed = reader.GetBoolean(4),
                    CreatedAt = reader.GetDateTime(5)
                }; 
            }
            catch (SqliteException)
            {
                return null;
            }
        }


        public static bool UpdateItem(int item_id, string username, string title, string description)
        {
            try
            {
                using var connection = new SqliteConnection("Data Source = database.db");
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = """
                SELECT userId FROM users WHERE username = $username LIMIT 1;
                """;
                command.Parameters.AddWithValue("$username", username);
                var result = command.ExecuteScalar();
                if(result == null) return false;
                int userId = Convert.ToInt32(result);
                command.Parameters.Clear();
                command.CommandText = """
                SELECT completed FROM todoitems WHERE ItemId = $itemId and userId = $userId LIMIT 1;
                """;
                command.Parameters.AddWithValue("$itemId", item_id);
                command.Parameters.AddWithValue("$userId", userId);
                var response = command.ExecuteScalar();
                bool item_status = Convert.ToBoolean(response);
                command.Parameters.Clear();

                command.CommandText = """
                UPDATE todoitems SET title = $title, description = $description WHERE ItemId = $itemId and userId = $userId;
                """;
                command.Parameters.AddWithValue("$title", title);
                command.Parameters.AddWithValue("$description", description);
                command.Parameters.AddWithValue("$itemId", item_id);
                command.Parameters.AddWithValue("$userId", userId);
                command.ExecuteNonQuery();
                return true;
            } catch(SqliteException e)
            {
                Console.WriteLine($"UpdateItem Error: {e}");
                return false;
            }
        }

        public static bool ToggleItem(int item_id, string username)
        {
            try
            {
                using var connection = new SqliteConnection("Data Source = database.db");
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = """
                SELECT userId FROM users WHERE username = $username LIMIT 1;
                """;
                command.Parameters.AddWithValue("$username", username);
                var result = command.ExecuteScalar();
                if(result == null) return false;
                int userId = Convert.ToInt32(result);
                command.Parameters.Clear();
                command.CommandText = """
                SELECT completed FROM todoitems WHERE ItemId = $itemId and userId = $userId LIMIT 1;
                """;
                command.Parameters.AddWithValue("$itemId", item_id);
                command.Parameters.AddWithValue("$userId", userId);
                var response = command.ExecuteScalar();
                bool item_status = Convert.ToBoolean(response);
                command.Parameters.Clear();

                command.CommandText = """
                UPDATE todoitems SET completed = $status WHERE ItemId = $itemId and userId = $userId;
                """;
                bool new_status = false;
                if(item_status) new_status = false;
                else new_status = true;
                command.Parameters.AddWithValue("$status", new_status);
                command.Parameters.AddWithValue("$itemId", item_id);
                command.Parameters.AddWithValue("$userId", userId);
                command.ExecuteNonQuery();
                return true;
            } catch(SqliteException e)
            {
                Console.WriteLine($"ToggleItem Error: {e}");
                return false;
            }
        }


        public static bool DeleteItem(int item_id, string username)
        {
            try
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
                DELETE FROM todoitems WHERE ItemId = $itemId and userId = $userId;
                """;
                command.Parameters.AddWithValue("$itemId", item_id);
                command.Parameters.AddWithValue("$userId", userId);
                int rowsDeleted = command.ExecuteNonQuery();
                if(rowsDeleted < 1) return false;
                return true;
            } catch (SqliteException e) 
            {
                Console.WriteLine($"DeleteItem Error: {e}");
                return false;
            }
            
        }

    }
}