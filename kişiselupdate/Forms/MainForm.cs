using ExpenseTracker.Commands;
using ExpenseTracker.Database;
using ExpenseTracker.Models;
using System.Windows.Forms.DataVisualization.Charting;

namespace ExpenseTracker.Forms
{
    public partial class MainForm : Form
    {
        private static Stack<ICommand> _commandHistory = new();
        private DataGridView dgvExpenses = null!;
        private ComboBox cmbCategories = null!;
        private TextBox txtDescription = null!;
        private TextBox txtAmount = null!;
        private Button btnAdd = null!;
        private Button btnDelete = null!;
        private Button btnUndo = null!;
        private System.Windows.Forms.DataVisualization.Charting.Chart pieChart = null!;
        private Label lblAdvice = null!;

        public MainForm()
        {
            InitializeComponent();
            LoadCategories();
            LoadExpenses();
        }

        private void InitializeComponent()
        {
            this.Text = "Harcama Takip";
            this.Size = new Size(1200, 800);

            // DataGridView
            dgvExpenses = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(700, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            // Kategori Combobox
            cmbCategories = new ComboBox
            {
                Location = new Point(20, 340),
                Size = new Size(150, 25)
            };

            // Açıklama TextBox
            txtDescription = new TextBox
            {
                Location = new Point(180, 340),
                Size = new Size(200, 25),
                PlaceholderText = "Açıklama"
            };

            // Tutar TextBox
            txtAmount = new TextBox
            {
                Location = new Point(390, 340),
                Size = new Size(100, 25),
                PlaceholderText = "Tutar"
            };

            // Ekle Butonu
            btnAdd = new Button
            {
                Text = "Ekle",
                Location = new Point(500, 340),
                Size = new Size(70, 25)
            };
            btnAdd.Click += BtnAdd_Click;

            // Sil Butonu
            btnDelete = new Button
            {
                Text = "Sil",
                Location = new Point(580, 340),
                Size = new Size(70, 25)
            };
            btnDelete.Click += BtnDelete_Click;

            // Geri Al Butonu
            btnUndo = new Button
            {
                Text = "Geri Al",
                Location = new Point(660, 340),
                Size = new Size(70, 25)
            };
            btnUndo.Click += BtnUndo_Click;

            // Pie Chart
            pieChart = new System.Windows.Forms.DataVisualization.Charting.Chart
            {
                Location = new Point(750, 20),
                Size = new Size(400, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // ChartArea ekle
            pieChart.ChartAreas.Add(new ChartArea());
            
            pieChart.Titles.Add("Harcama Dağılımı");
            var series = pieChart.Series.Add("Harcamalar");
            series.ChartType = SeriesChartType.Pie;

            // Legend (gösterge) ayarları
            var legend = new System.Windows.Forms.DataVisualization.Charting.Legend();
            legend.Enabled = true;
            legend.Font = new Font("Segoe UI", 10);
            legend.BackColor = Color.Transparent;
            legend.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            pieChart.Legends.Add(legend);

            // Tavsiye Etiketi ayarları
            lblAdvice = new Label
            {
                Location = new Point(750, 330),
                Size = new Size(400, 200),
                AutoSize = false,
                Font = new Font("Segoe UI", 9.5f),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Metin kaydırma özelliğini aktif et
            lblAdvice.AutoEllipsis = true;

            // Kontrolleri forma ekle
            Controls.AddRange(new Control[] {
                dgvExpenses,
                cmbCategories,
                txtDescription,
                txtAmount,
                btnAdd,
                btnDelete,
                btnUndo,
                pieChart,
                lblAdvice
            });
        }

        private void LoadExpenses()
        {
            var connection = DatabaseConnection.Instance.GetConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT 
                    e.id as 'ID',
                    c.name as 'Kategori',
                    e.description as 'Açıklama',
                    e.amount as 'Tutar',
                    e.date as 'Tarih'
                FROM expenses e
                JOIN categories c ON e.category_id = c.id
                ORDER BY e.date DESC, e.id DESC";

            var dt = new System.Data.DataTable();
            using (var reader = command.ExecuteReader())
            {
                dt.Load(reader);
            }
            dgvExpenses.DataSource = dt;

            if (dgvExpenses.Columns.Count > 0)
            {
                dgvExpenses.Columns["ID"].Visible = false;
                dgvExpenses.Columns["Tutar"].DefaultCellStyle.Format = "C2";
            }

            UpdatePieChart();
            UpdateAdviceText();
        }

        private void UpdatePieChart()
        {
            try
            {
                var connection = DatabaseConnection.Instance.GetConnection();
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT c.name as category, SUM(e.amount) as total
                    FROM expenses e
                    JOIN categories c ON e.category_id = c.id
                    GROUP BY c.name
                    ORDER BY total DESC";

                pieChart.Series[0].Points.Clear();

                // Pasta dilimi etiket ayarları
                pieChart.Series[0]["PieLabelStyle"] = "Outside";
                pieChart.Series[0]["PieLineColor"] = "Black";
                pieChart.Series[0]["PieDrawingStyle"] = "Default";

                // Önce toplam tutarı hesapla
                decimal totalAmount = 0;
                using (var firstReader = command.ExecuteReader())
                {
                    while (firstReader.Read())
                    {
                        totalAmount += (decimal)firstReader.GetDouble(1);
                    }
                }

                // Verileri ekle
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var category = reader.GetString(0);
                    var amount = reader.GetDouble(1);
                    var percentage = (decimal)amount / totalAmount * 100;

                    var point = pieChart.Series[0].Points.Add(amount);
                    point.LegendText = $"{category} (%{percentage:F1})";  // Kategori adı ve yüzde
                    point.Label = $"{amount:C0}";  // Tutar
                }

                if (pieChart.Series[0].Points.Count == 0)
                {
                    var point = pieChart.Series[0].Points.Add(1);
                    point.Color = Color.LightGray;
                    point.LegendText = "Veri Yok";
                    point.Label = "Harcama Verisi\nBulunmuyor";
                }
            }
            catch (Exception ex)
            {
                pieChart.Series[0].Points.Clear();
                MessageBox.Show($"Grafik güncellenirken hata oluştu: {ex.Message}");
            }
        }

        private void UpdateAdviceText()
        {
            try
            {
                var connection = DatabaseConnection.Instance.GetConnection();
                
                // Toplam harcama ve kategori bazlı harcamaları al
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    WITH CategoryTotals AS (
                        SELECT 
                            c.name as category,
                            SUM(e.amount) as total,
                            COUNT(*) as transaction_count,
                            MAX(e.amount) as max_expense,
                            AVG(e.amount) as avg_expense
                        FROM expenses e
                        JOIN categories c ON e.category_id = c.id
                        GROUP BY c.name
                    ),
                    TotalExpense AS (
                        SELECT SUM(total) as grand_total
                        FROM CategoryTotals
                    )
                    SELECT 
                        ct.*,
                        (ct.total * 100.0 / te.grand_total) as percentage
                    FROM CategoryTotals ct, TotalExpense te
                    ORDER BY ct.total DESC
                    LIMIT 1";

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var category = reader.GetString(0);
                    var total = reader.GetDouble(1);
                    var transactionCount = reader.GetInt32(2);
                    var maxExpense = reader.GetDouble(3);
                    var avgExpense = reader.GetDouble(4);
                    var percentage = reader.GetDouble(5);

                    var advice = GenerateAdvice(
                        category, 
                        total, 
                        transactionCount, 
                        maxExpense, 
                        avgExpense, 
                        percentage);

                    lblAdvice.Text = advice;
                }
                else
                {
                    lblAdvice.Text = "Henüz harcama kaydı bulunmuyor. Harcamalarınızı takip etmek için kayıt ekleyebilirsiniz.";
                }
            }
            catch (Exception ex)
            {
                lblAdvice.Text = "Tavsiye metni oluşturulurken bir hata oluştu.";
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }

        private string GenerateAdvice(
            string category, 
            double total, 
            int transactionCount, 
            double maxExpense, 
            double avgExpense, 
            double percentage)
        {
            var sb = new System.Text.StringBuilder();
            
            // Ana tavsiye
            sb.AppendLine($"📊 {category} kategorisinde dikkat çeken noktalar:");
            sb.AppendLine();

            // Yüzde analizi
            if (percentage > 50)
            {
                sb.AppendLine($"⚠️ Bu kategori toplam harcamanızın %{percentage:F1}'ini oluşturuyor! ");
                sb.AppendLine("Bütçe dengeniz için bu oranı düşürmeyi düşünebilirsiniz.");
            }
            else if (percentage > 30)
            {
                sb.AppendLine($"❗ Bu kategori harcamalarınızın %{percentage:F1}'lik önemli bir kısmını oluşturuyor.");
            }

            // Kategori bazlı özel tavsiyeler
            switch (category.ToLower())
            {
                case "gıda":
                    if (avgExpense > 200)
                        sb.AppendLine("💡 Market alışverişlerinizi liste yaparak ve kampanyaları takip ederek optimize edebilirsiniz.");
                    break;

                case "ulaşım":
                    if (total > 1000)
                        sb.AppendLine("💡 Toplu taşıma veya alternatif ulaşım yöntemlerini değerlendirebilirsiniz.");
                    break;

                case "eğlence":
                    if (percentage > 20)
                        sb.AppendLine("💡 Ücretsiz etkinlikleri araştırarak eğlence bütçenizi dengeleyebilirsiniz.");
                    break;

                case "faturalar":
                    if (maxExpense > avgExpense * 1.5)
                        sb.AppendLine("💡 Fatura tutarlarınızda ani yükselmeler var. Enerji tasarrufu önlemleri alabilirsiniz.");
                    break;

                case "alışveriş":
                    if (transactionCount > 10)
                        sb.AppendLine("💡 Sık sık küçük alışverişler yerine planlı ve toplu alışverişler yapabilirsiniz.");
                    break;

                case "sağlık":
                    if (total > 1000)
                        sb.AppendLine("💡 Sağlık sigortası veya tamamlayıcı sağlık sigortası seçeneklerini değerlendirebilirsiniz.");
                    break;
            }

            // İşlem sıklığı analizi
            if (transactionCount > 15)
            {
                sb.AppendLine($"📈 Bu kategoride ayda {transactionCount} işlem yapmışsınız.");
                sb.AppendLine("İşlem sayısını azaltmak için harcamalarınızı birleştirebilirsiniz.");
            }

            // Ortalama harcama analizi
            if (maxExpense > avgExpense * 2)
            {
                sb.AppendLine();
                sb.AppendLine($"💰 En yüksek harcamanız ({maxExpense:C2}) ortalamadan çok yüksek.");
                sb.AppendLine("Büyük harcamalar için bütçe planlaması yapmanızı öneririz.");
            }

            return sb.ToString();
        }

        private void LoadCategories()
        {
            cmbCategories.Items.Clear();
            
            var connection = DatabaseConnection.Instance.GetConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT id, name FROM categories ORDER BY name";
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                cmbCategories.Items.Add(new CategoryItem(
                    reader.GetInt32(0),
                    reader.GetString(1)
                ));
            }

            if (cmbCategories.Items.Count > 0)
            {
                cmbCategories.SelectedIndex = 0;
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (cmbCategories.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtDescription.Text) ||
                !double.TryParse(txtAmount.Text, out double amount))
            {
                MessageBox.Show("Lütfen tüm alanları doğru şekilde doldurun.");
                return;
            }

            try
            {
                var selectedCategory = (CategoryItem)cmbCategories.SelectedItem;
                
                var expense = new Expense(
                    0,
                    selectedCategory.Id,
                    txtDescription.Text,
                    amount,
                    DateTime.Now.ToString("yyyy-MM-dd")
                );

                var command = new AddExpenseCommand(expense);
                command.Execute();
                _commandHistory.Push(command);
                
                LoadExpenses();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvExpenses.CurrentRow == null)
            {
                MessageBox.Show("Lütfen silinecek bir harcama seçin.");
                return;
            }

            try
            {
                int expenseId = Convert.ToInt32(dgvExpenses.CurrentRow.Cells["ID"].Value);
                var command = new DeleteExpenseCommand(expenseId);
                command.Execute();
                _commandHistory.Push(command);
                
                LoadExpenses();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }

        private void BtnUndo_Click(object? sender, EventArgs e)
        {
            if (_commandHistory.Count > 0)
            {
                try
                {
                    var command = _commandHistory.Pop();
                    command.Undo();
                    LoadExpenses();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Geri alma işlemi başarısız: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Geri alınacak işlem bulunmuyor.");
            }
        }

        private void ClearInputs()
        {
            txtDescription.Clear();
            txtAmount.Clear();
            if (cmbCategories.Items.Count > 0)
                cmbCategories.SelectedIndex = 0;
        }
    }
} 