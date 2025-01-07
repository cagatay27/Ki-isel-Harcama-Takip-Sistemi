using Microsoft.Data.Sqlite;

namespace ExpenseTracker.Database
{
    public class DatabaseConnection
    {
        private static DatabaseConnection? _instance;
        private readonly SqliteConnection _connection;

        private DatabaseConnection()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = "ExpenseTracker.db",
                Mode = SqliteOpenMode.ReadWriteCreate
            };

            _connection = new SqliteConnection(connectionStringBuilder.ConnectionString);
            _connection.Open();
        }

        public static DatabaseConnection Instance
        {
            get
            {
                _instance ??= new DatabaseConnection();
                return _instance;
            }
        }

        public SqliteConnection GetConnection()
        {
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }
            return _connection;
        }
    }
}
