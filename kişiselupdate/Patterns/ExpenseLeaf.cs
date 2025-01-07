using ExpenseTracker.Models;

namespace ExpenseTracker.Patterns
{
    /*
     * Composite tasarım kalıbının Leaf (Yaprak) rolünü üstlenen sınıf:
     * 1. Hiyerarşinin en alt seviyesindeki tekil harcamaları temsil eder
     * 2. Alt bileşenleri yoktur (Add/Remove metodları bulunmaz)
     * 3. GetTotal() metodu doğrudan harcama tutarını döndürür
     * 4. Display() metodu sadece kendi bilgilerini görüntüler
     * 
     * Bu sınıf sayesinde:
     * - Tekil harcamalar hiyerarşiye eklenebilir
     * - Harcama tutarları kategoriler içinde toplanabilir
     * - Hiyerarşik görüntülemede harcama detayları gösterilebilir
     */
    public class ExpenseLeaf : ExpenseComponent
    {
        private readonly Expense _expense;

        public ExpenseLeaf(Expense expense, string name) : base(name)
        {
            _expense = expense;
        }

        public override double GetTotal()
        {
            return _expense.Amount;
        }

        public override void Display(int depth = 0)
        {
            Console.WriteLine($"{new string('-', depth)} {Name}: {_expense.Amount:C2}");
        }
    }
}
