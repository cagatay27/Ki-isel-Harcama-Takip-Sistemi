using Microsoft.Data.Sqlite;

namespace ExpenseTracker.Database
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            try
            {
                var connection = DatabaseConnection.Instance.GetConnection();

                // Foreign key desteğini aktif et
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "PRAGMA foreign_keys = ON";
                    command.ExecuteNonQuery();
                }

                // Tabloları temizle
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        DROP TABLE IF EXISTS expenses;
                        DROP TABLE IF EXISTS categories;";
                    command.ExecuteNonQuery();
                }

                // Categories tablosunu oluştur
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS categories (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            name TEXT NOT NULL UNIQUE
                        )";
                    command.ExecuteNonQuery();
                }

                // Expenses tablosunu oluştur
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS expenses (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            category_id INTEGER NOT NULL,
                            description TEXT NOT NULL,
                            amount REAL NOT NULL,
                            date TEXT NOT NULL,
                            FOREIGN KEY(category_id) REFERENCES categories(id)
                            ON DELETE CASCADE
                        )";
                    command.ExecuteNonQuery();
                }

                // Varsayılan kategorileri ekle
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO categories (name) VALUES 
                        ('Gıda'),
                        ('Ulaşım'),
                        ('Kira'),
                        ('Faturalar'),
                        ('Eğlence'),
                        ('Alışveriş'),
                        ('Sağlık'),
                        ('Diğer')";
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veritabanı başlatılırken hata oluştu: {ex.Message}");
            }
        }
    }
} 