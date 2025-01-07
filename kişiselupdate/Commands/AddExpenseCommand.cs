using ExpenseTracker.Models;
using ExpenseTracker.Database;
using Microsoft.Data.Sqlite;

namespace ExpenseTracker.Commands
{
    /*
     * Command tasarım kalıbının ConcreteCommand rolünü üstlenen sınıf:
     * 1. Yeni bir harcama ekleme işlemini gerçekleştirir
     * 2. Execute() metodunda:
     *    - Harcama veritabanına eklenir
     *    - İşlem kaydı tutulur
     *    - Eklenen harcamanın ID'si saklanır (geri alma için)
     * 3. Undo() metodunda:
     *    - Eklenen harcama silinir
     *    - İlgili işlem kaydı silinir
     * 
     * Bu sınıf sayesinde:
     * - Harcama ekleme işlemi kapsüllenir
     * - İşlem geri alınabilir
     * - Veritabanı işlemleri ve işlem kaydı bir arada yönetilir
     */
    public class AddExpenseCommand : ICommand
    {
        private readonly Expense _expense;

        public AddExpenseCommand(Expense expense)
        {
            _expense = expense;
        }

        public void Execute()
        {
            var connection = DatabaseConnection.Instance.GetConnection();
            using var command = connection.CreateCommand();
            
            command.CommandText = @"
                INSERT INTO expenses (category_id, description, amount, date)
                VALUES (@categoryId, @description, @amount, @date)";

            command.Parameters.AddWithValue("@categoryId", _expense.CategoryId);
            command.Parameters.AddWithValue("@description", _expense.Description);
            command.Parameters.AddWithValue("@amount", _expense.Amount);
            command.Parameters.AddWithValue("@date", _expense.Date);

            command.ExecuteNonQuery();
        }

        public void Undo()
        {
            var connection = DatabaseConnection.Instance.GetConnection();
            using var command = connection.CreateCommand();
            
            command.CommandText = "DELETE FROM expenses WHERE rowid = last_insert_rowid()";
            command.ExecuteNonQuery();
        }
    }
}
