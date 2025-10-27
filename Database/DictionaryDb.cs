using MySql.Data.MySqlClient;

namespace Dictionary.Database;

public static class DictionaryDb
{
    public static MySqlConnection GetConnection()
    {
        string password = Environment.GetEnvironmentVariable("DB_PASSWORD");

        var builder = new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            Port = 3306,
            Database = "dictionary",
            UserID = "root",
            Password = password
        };

        return new MySqlConnection(builder.ConnectionString);
    }
}
