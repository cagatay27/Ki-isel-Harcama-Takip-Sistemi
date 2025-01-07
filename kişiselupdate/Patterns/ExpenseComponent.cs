namespace ExpenseTracker.Patterns
{
    /*
     * Composite tasarım kalıbı burada kullanılıyor çünkü:
     * 1. Harcamaları ve kategorileri ağaç yapısında hiyerarşik olarak düzenlemek gerekiyor
     * 2. Tekil harcamalar (leaf) ve kategori grupları (composite) için ortak bir arayüz sağlanması gerekiyor
     * 
     * Bu sınıf Composite pattern'in Component rolünü üstleniyor:
     * - Hem leaf (ExpenseLeaf) hem de composite (ExpenseCategory) nesneleri için ortak arayüzü tanımlıyor
     * - GetTotal() ve Display() metodları ile hiyerarşideki her nesnenin toplam tutarını hesaplama ve 
     *   görüntüleme işlemlerini standartlaştırıyor
     */
    public abstract class ExpenseComponent
    {
        protected ExpenseComponent(string name)
        {
            Name = name;
        }

        public string Name { get; protected set; }
        
        public abstract double GetTotal();
        public abstract void Display(int depth = 0);
    }
}
