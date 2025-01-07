namespace ExpenseTracker.Commands
{
    /*
     * Command tasarım kalıbı burada kullanılıyor çünkü:
     * 1. Harcama işlemlerini (ekleme, silme) nesneleştirmek ve kapsüllemek gerekiyor
     * 2. İşlemlerin geri alınabilir olması gerekiyor
     * 3. İşlem geçmişinin tutulması ve yönetilmesi gerekiyor
     * 
     * Bu arayüz Command pattern'in temelini oluşturuyor:
     * - Execute(): Komutu çalıştırır (örn: harcama ekleme)
     * - Undo(): Komutu geri alır (örn: eklenen harcamayı silme)
     * 
     * Bu sayede:
     * - Her işlem bir komut nesnesi olarak temsil edilebilir
     * - İşlemler geri alınabilir
     * - İşlem geçmişi takip edilebilir
     */
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}
