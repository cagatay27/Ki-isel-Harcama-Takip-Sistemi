﻿using ExpenseTracker.Forms;
using ExpenseTracker.Database;

namespace ExpenseTracker
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                ApplicationConfiguration.Initialize();
                
                // Veritabanı işlemlerini ayrı try-catch bloğuna alalım
                try
                {
                    DatabaseInitializer.Initialize();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Veritabanı başlatılırken hata oluştu:\n{ex.Message}\n\nUygulama kapatılacak.");
                    return; // Hata durumunda uygulamayı başlatma
                }

                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Uygulama başlatılırken hata oluştu:\n{ex.Message}");
            }
        }
    }
}
