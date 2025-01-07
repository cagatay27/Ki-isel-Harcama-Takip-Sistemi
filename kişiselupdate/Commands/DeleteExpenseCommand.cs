using ExpenseTracker.Models;
using ExpenseTracker.Database;
using Microsoft.Data.Sqlite;

namespace ExpenseTracker.Commands
{
    /*
     * Command tasarım kalıbının ConcreteCommand rolünü üstlenen sınıf:
     * 1. Var olan bir harcamayı silme işlemini gerçekleştirir
     * 2. Execute() metodunda:
     *    - Silinecek harcamanın bilgileri saklanır (geri alma için)
     *    - İşlem kaydı tutulur
     *    - Harcama veritabanından silinir
     * 3. Undo() metodunda:
     *    - Silinen harcama geri yüklenir
     *    - Silme işleminin kaydı silinir
     * 
     * Bu sınıf sayesinde:
     * - Harcama silme işlemi kapsüllenir
     * - Silinen harcamalar geri getirilebilir
     * - Veritabanı tutarlılığı korunur
     */
    public class DeleteExpenseCommand : ICommand
    {
        private readonly int _expenseId;
        private Expense? _deletedExpense;

        public DeleteExpenseCommand(int expenseId)
        {
            _expenseId = expenseId;
        }

        public void Execute()
        {
            var connection = DatabaseConnection.Instance.GetConnection();
            
            // Önce silinecek veriyi kaydet
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT category_id, description, amount, date FROM expenses WHERE id = @id";
                command.Parameters.AddWithValue("@id", _expenseId);
                
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    _deletedExpense = new Expense(
                        _expenseId,
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetDouble(2),
                        reader.GetString(3)
                    );
                }
            }

            // Sonra sil
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM expenses WHERE id = @id";
                command.Parameters.AddWithValue("@id", _expenseId);
                command.ExecuteNonQuery();
            }
        }

        public void Undo()
        {
            if (_deletedExpense == null) return;

            var connection = DatabaseConnection.Instance.GetConnection();
            using var command = connection.CreateCommand();
            
            command.CommandText = @"
                INSERT INTO expenses (id, category_id, description, amount, date)
                VALUES (@id, @categoryId, @description, @amount, @date)";

            command.Parameters.AddWithValue("@id", _deletedExpense.Id);
            command.Parameters.AddWithValue("@categoryId", _deletedExpense.CategoryId);
            command.Parameters.AddWithValue("@description", _deletedExpense.Description);
            command.Parameters.AddWithValue("@amount", _deletedExpense.Amount);
            command.Parameters.AddWithValue("@date", _deletedExpense.Date);

            command.ExecuteNonQuery();
        }
    }
}
