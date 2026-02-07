using Microsoft.Data.Sqlite;

namespace api.dbactions
{
    public static class DbInitialiser
    {
        public static void InitialiseDB()
        {
            using var connection = new SqliteConnection("Data Source=database.db");
            
            connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText = "PRAGMA foreign_keys = ON;";
            command.ExecuteNonQuery();

            command.CommandText = """
            CREATE TABLE IF NOT EXISTS users (
            userId INTEGER PRIMARY KEY AUTOINCREMENT,
            username TEXT UNIQUE NOT NULL,
            password TEXT NOT NULL
            );
            """;
            command.ExecuteNonQuery();

            command.CommandText = """
            CREATE TABLE IF NOT EXISTS todoitems (
            ItemId INTEGER PRIMARY KEY AUTOINCREMENT,
            userId INTEGER NOT NULL,
            title TEXT NOT NULL,
            description TEXT,
            created_at DATETIME NOT NULL,
            FOREIGN KEY (userId) REFERENCES users(userId) ON DELETE CASCADE
            );
            """;
            command.ExecuteNonQuery();
        }
    }
}
