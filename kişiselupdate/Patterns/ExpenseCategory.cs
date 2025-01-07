namespace ExpenseTracker.Patterns
{
    /*
     * Composite tasarım kalıbının Composite rolünü üstlenen sınıf:
     * 1. Alt bileşenleri (hem kategorileri hem de tekil harcamaları) içerebilir
     * 2. Add() ve Remove() metodları ile alt bileşenlerin yönetimini sağlar
     * 3. GetTotal() metodu ile tüm alt bileşenlerin toplamını hesaplar
     * 4. Display() metodu ile hiyerarşik görüntülemeyi sağlar
     * 
     * Bu sayede:
     * - Kategoriler iç içe oluşturulabilir (örn: Ana Kategori > Alt Kategori > Harcama)
     * - Herhangi bir seviyedeki toplam harcama miktarı hesaplanabilir
     * - Hiyerarşik yapı görsel olarak temsil edilebilir
     */
    public class ExpenseCategory : ExpenseComponent
    {
        private readonly List<ExpenseComponent> _expenses = new();

        public ExpenseCategory(string name) : base(name)
        {
        }

        public void Add(ExpenseComponent component)
        {
            _expenses.Add(component);
        }

        public void Remove(ExpenseComponent component)
        {
            _expenses.Remove(component);
        }

        public override double GetTotal()
        {
            return _expenses.Sum(x => x.GetTotal());
        }

        public override void Display(int depth = 0)
        {
            Console.WriteLine($"{new string('-', depth)} {Name} (Toplam: {GetTotal():C2})");
            foreach (var expense in _expenses)
            {
                expense.Display(depth + 2);
            }
        }
    }
}
