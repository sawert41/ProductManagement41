using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ProductManagement41
{
    public partial class Form1 : Form
    {

        private string connectionString = "Server=VEYSELPC\\SQLEXPRESS;Database=ProductManagementDB;Integrated Security=True;TrustServerCertificate=True";

        // UI Elemanları Tanımlamaları
        private Button btnShowProducts;
        private Button btnAddProduct;
        private Button btnUpdateProduct; // Yeni: Ürün Güncelle Butonu
        private Button btnDeleteProduct; // Yeni: Ürün Sil Butonu
        private DataGridView dgvProducts;
        private TextBox txtProductName;
        private TextBox txtProductPrice;
        private TextBox txtProductSize;
        private TextBox txtProductColor;
        private ComboBox cmbCategory;
        private ComboBox cmbSupplier;
        private Label lblMessage;

        // Verileri tutmak için listeler
        private List<Category> categories = new List<Category>();
        private List<Supplier> suppliers = new List<Supplier>();

        // Seçili ürünün ID'sini tutmak için (güncelleme/silme için gerekli)
        private int selectedProductId = -1;

        public Form1()
        {
            InitializeComponent(); // Designer.cs'den gelen temel form ayarları

            this.Text = "Ürün Yönetim Sistemi"; // Form başlığı
            this.Size = new Size(1200, 700);      // Pencere boyutu
            this.StartPosition = FormStartPosition.CenterScreen; // Ekranın ortasında başla

            this.Load += Form1_Load; // Form yüklendiğinde çalışacak olay

            // Formun yeniden boyutlandırılmasını engeller, maksimum boyutu kapatır
            this.AutoSize = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateUIElements(); // Tüm arayüz elemanlarını oluştur
            LoadCategoriesIntoComboBox(); // Kategorileri ComboBox'a yükle
            LoadSuppliersIntoComboBox();  // Tedarikçileri ComboBox'a yükle
            GetAllProducts();           // Mevcut tüm ürünleri DataGridView'e yükle
            ClearProductInputs();       // Başlangıçta giriş alanlarını ve butonları sıfırla
        }

        // Tüm UI Elemanlarını Oluşturan Metot
        private void CreateUIElements()
        {
            // Yeni Ürün Ekle Bölümü Başlığı
            Label lblAddProductHeader = new Label();
            lblAddProductHeader.Text = "Yeni Ürün Ekle / Düzenle"; // Başlık güncellendi
            lblAddProductHeader.Location = new Point(20, 20);
            lblAddProductHeader.AutoSize = true;
            lblAddProductHeader.Font = new Font("Arial", 12, FontStyle.Bold);
            this.Controls.Add(lblAddProductHeader);

            // UI elemanlarının konumlandırma ayarları
            int currentY = 50;
            int labelX = 20;
            int controlX = 120;
            int controlWidth = 200;
            int rowHeight = 30;

            // Ürün Adı Giriş Alanı
            AddLabelAndTextBox(labelX, currentY, "Ürün Adı:", ref txtProductName, controlX, controlWidth);
            currentY += rowHeight;

            // Fiyat Giriş Alanı
            AddLabelAndTextBox(labelX, currentY, "Fiyat:", ref txtProductPrice, controlX, controlWidth);
            currentY += rowHeight;

            // Kategori Seçim ComboBox'ı
            AddLabelAndComboBox(labelX, currentY, "Kategori:", ref cmbCategory, controlX, controlWidth);
            currentY += rowHeight;

            // Tedarikçi Seçim ComboBox'ı
            AddLabelAndComboBox(labelX, currentY, "Tedarikçi:", ref cmbSupplier, controlX, controlWidth);
            currentY += rowHeight;

            // Beden (Size) Giriş Alanı
            AddLabelAndTextBox(labelX, currentY, "Boyut:", ref txtProductSize, controlX, controlWidth);
            currentY += rowHeight;

            // Renk (Color) Giriş Alanı
            AddLabelAndTextBox(labelX, currentY, "Renk:", ref txtProductColor, controlX, controlWidth);
            currentY += rowHeight;

            // Ürün Ekle Butonu
            btnAddProduct = new Button();
            btnAddProduct.Text = "Ürün Ekle";
            btnAddProduct.Location = new Point(controlX, currentY);
            btnAddProduct.Size = new Size(100, 30);
            btnAddProduct.Click += BtnAddProduct_Click;
            this.Controls.Add(btnAddProduct);
            currentY += rowHeight;

            // Ürün Güncelle Butonu (Başlangıçta pasif)
            btnUpdateProduct = new Button();
            btnUpdateProduct.Text = "Ürünü Güncelle";
            btnUpdateProduct.Location = new Point(controlX, currentY);
            btnUpdateProduct.Size = new Size(100, 30);
            btnUpdateProduct.Click += BtnUpdateProduct_Click;
            btnUpdateProduct.Enabled = false; // Başlangıçta pasif
            this.Controls.Add(btnUpdateProduct);
            currentY += rowHeight;

            // Ürün Sil Butonu (Başlangıçta pasif)
            btnDeleteProduct = new Button();
            btnDeleteProduct.Text = "Ürünü Sil";
            btnDeleteProduct.Location = new Point(controlX, currentY);
            btnDeleteProduct.Size = new Size(100, 30);
            btnDeleteProduct.Click += BtnDeleteProduct_Click;
            btnDeleteProduct.Enabled = false; // Başlangıçta pasif
            this.Controls.Add(btnDeleteProduct);
            currentY += rowHeight + 10; // Mesaj label'ına biraz boşluk

            // Mesaj Görüntüleme Label'ı
            lblMessage = new Label();
            lblMessage.Text = "";
            lblMessage.Location = new Point(labelX, currentY);
            lblMessage.AutoSize = true;
            lblMessage.ForeColor = Color.DarkGreen;
            this.Controls.Add(lblMessage);

            // Mevcut Ürünler Bölümü Başlığı
            Label lblProductListHeader = new Label();
            lblProductListHeader.Text = "Mevcut Ürünler";
            lblProductListHeader.Location = new Point(350, 20);
            lblProductListHeader.AutoSize = true;
            lblProductListHeader.Font = new Font("Arial", 12, FontStyle.Bold);
            this.Controls.Add(lblProductListHeader);

            // Ürünleri Listeleyen DataGridView
            dgvProducts = new DataGridView();
            dgvProducts.Location = new Point(350, 50);
            dgvProducts.Size = new Size(800, 600);
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Sütunları otomatik doldur
            dgvProducts.AllowUserToAddRows = false; // Kullanıcının direkt satır eklemesini engelle
            dgvProducts.ReadOnly = true; // Sadece okunabilir yap
            this.Controls.Add(dgvProducts);

            // DataGridView'de bir hücreye tıklandığında olay
            dgvProducts.CellClick += DgvProducts_CellClick;

            // Ürünleri Yenile Butonu
            btnShowProducts = new Button();
            btnShowProducts.Text = "Ürünleri Yenile";
            btnShowProducts.Location = new Point(350, 660); // DataGridView'in altına
            btnShowProducts.Size = new Size(120, 30);
            btnShowProducts.Click += (s, ev) => GetAllProducts(); // Lambda ifadesi ile kısa olay ataması
            this.Controls.Add(btnShowProducts);
        }

        // Yardımcı Metot: Label ve TextBox Oluşturur
        private void AddLabelAndTextBox(int labelX, int y, string labelText, ref TextBox textBox, int controlX, int controlWidth, bool multiLine = false, int height = 20)
        {
            Label lbl = new Label();
            lbl.Text = labelText;
            lbl.Location = new Point(labelX, y + 3); // Dikeyde ortalamak için +3
            lbl.AutoSize = true;
            this.Controls.Add(lbl);

            textBox = new TextBox();
            textBox.Location = new Point(controlX, y);
            textBox.Width = controlWidth;
            textBox.Height = height;
            textBox.Multiline = multiLine;
            if (multiLine) textBox.ScrollBars = ScrollBars.Vertical;
            this.Controls.Add(textBox);
        }

        // Yardımcı Metot: Label ve ComboBox Oluşturur
        private void AddLabelAndComboBox(int labelX, int y, string labelText, ref ComboBox comboBox, int controlX, int controlWidth)
        {
            Label lbl = new Label();
            lbl.Text = labelText;
            lbl.Location = new Point(labelX, y + 3); // Dikeyde ortalamak için +3
            lbl.AutoSize = true;
            this.Controls.Add(lbl);

            comboBox = new ComboBox();
            comboBox.Location = new Point(controlX, y);
            comboBox.Width = controlWidth;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList; // Sadece listeden seçim yapmaya izin ver
            this.Controls.Add(comboBox);
        }

        // Ürün Giriş Alanlarını Temizler ve Buton Durumlarını Sıfırlar
        private void ClearProductInputs()
        {
            txtProductName.Clear();
            txtProductPrice.Clear();
            txtProductSize.Clear();
            txtProductColor.Clear();
            cmbCategory.SelectedIndex = -1; // ComboBox seçimini kaldır
            cmbSupplier.SelectedIndex = -1; // ComboBox seçimini kaldır
            selectedProductId = -1; // Seçili ürün ID'sini sıfırla

            // Buton durumlarını ayarla: Ekle aktif, Güncelle/Sil pasif
            btnAddProduct.Enabled = true;
            btnUpdateProduct.Enabled = false;
            btnDeleteProduct.Enabled = false;
            lblMessage.Text = ""; // Mesajı temizle
        }

        // Ürün Ekle Butonuna Tıklandığında
        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            // Zorunlu alanların kontrolü
            if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                string.IsNullOrWhiteSpace(txtProductPrice.Text) ||
                cmbCategory.SelectedItem == null ||
                cmbSupplier.SelectedItem == null)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "Lütfen tüm gerekli alanları doldurun (Ürün Adı, Fiyat, Kategori, Tedarikçi).";
                return;
            }

            try
            {
                string productName = txtProductName.Text;
                // Fiyatın sayısal olup olmadığını kontrol et
                if (!decimal.TryParse(txtProductPrice.Text, out decimal productPrice))
                {
                    lblMessage.ForeColor = Color.Red;
                    lblMessage.Text = "Lütfen geçerli bir fiyat girin.";
                    return;
                }

                Category selectedCategory = cmbCategory.SelectedItem as Category;
                Supplier selectedSupplier = cmbSupplier.SelectedItem as Supplier;

                // Category veya Supplier seçimi hatası
                if (selectedCategory == null || selectedSupplier == null)
                {
                    lblMessage.ForeColor = Color.Red;
                    lblMessage.Text = "Kategori veya Tedarikçi seçimi hatalı.";
                    return;
                }

                string productSize = txtProductSize.Text;
                string productColor = txtProductColor.Text;

                // Ürünü veritabanına ekle ve yeni ID'sini al
                int newProductId = AddProduct(productName, productPrice, selectedSupplier.Id, selectedCategory.Id, productSize, productColor);

                if (newProductId > 0)
                {
                    // Başlangıç stoğunu ekle (ürün eklendiğinde 1 adet stoğu olsun)
                    AddStock(newProductId, 1);

                    lblMessage.ForeColor = Color.DarkGreen;
                    lblMessage.Text = $"'{productName}' adlı ürün başarıyla eklendi! Ürün ID: {newProductId}";

                    ClearProductInputs(); // Alanları temizle ve butonları sıfırla
                    GetAllProducts();     // DataGridView'i yenile
                }
                else
                {
                    lblMessage.ForeColor = Color.Red;
                    lblMessage.Text = "Ürün eklenirken bir sorun oluştu.";
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "Ürün Ekleme"); // Hata yönetimi
            }
        }

        // DataGridView Hücresine Tıklandığında Çalışır
        private void DgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Geçerli bir satıra tıklandığından emin ol
            {
                DataGridViewRow row = dgvProducts.Rows[e.RowIndex];

                // Seçili ürün ID'sini al
                selectedProductId = Convert.ToInt32(row.Cells["ProductId"].Value);

                // Seçili ürün bilgilerini giriş alanlarına doldur
                txtProductName.Text = row.Cells["ProductName"].Value.ToString();
                txtProductPrice.Text = row.Cells["Price"].Value.ToString();
                // Nullable string'ler için ?? "" kullanarak null ise boş string atarız
                txtProductSize.Text = row.Cells["Size"].Value?.ToString() ?? "";
                txtProductColor.Text = row.Cells["Color"].Value?.ToString() ?? "";

                // Kategori ComboBox'ını ayarla
                string categoryName = row.Cells["CategoryName"].Value.ToString();
                Category selectedCategoryObj = categories.FirstOrDefault(c => c.Name == categoryName);
                if (selectedCategoryObj != null)
                {
                    cmbCategory.SelectedItem = selectedCategoryObj;
                }
                else
                {
                    cmbCategory.SelectedIndex = -1; // Bulunamadıysa hiçbir şey seçme
                }

                // Tedarikçi ComboBox'ını ayarla
                string supplierName = row.Cells["SupplierName"].Value.ToString();
                Supplier selectedSupplierObj = suppliers.FirstOrDefault(s => s.Name == supplierName);
                if (selectedSupplierObj != null)
                {
                    cmbSupplier.SelectedItem = selectedSupplierObj;
                }
                else
                {
                    cmbSupplier.SelectedIndex = -1; // Bulunamadıysa hiçbir şey seçme
                }

                // Buton durumlarını ayarla: Ekle pasif, Güncelle/Sil aktif
                btnAddProduct.Enabled = false;
                btnUpdateProduct.Enabled = true;
                btnDeleteProduct.Enabled = true;
                lblMessage.Text = $"Ürün ID: {selectedProductId} seçildi. Bilgileri düzenleyebilir veya silebilirsiniz.";
                lblMessage.ForeColor = Color.Blue;
            }
        }

        // Ürün Güncelle Butonuna Tıklandığında
        private void BtnUpdateProduct_Click(object sender, EventArgs e)
        {
            if (selectedProductId == -1)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "Lütfen önce güncellenecek bir ürün seçin.";
                return;
            }

            // Zorunlu alanların kontrolü
            if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                string.IsNullOrWhiteSpace(txtProductPrice.Text) ||
                cmbCategory.SelectedItem == null ||
                cmbSupplier.SelectedItem == null)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "Lütfen tüm gerekli alanları doldurun (Ürün Adı, Fiyat, Kategori, Tedarikçi).";
                return;
            }

            try
            {
                string productName = txtProductName.Text;
                if (!decimal.TryParse(txtProductPrice.Text, out decimal productPrice))
                {
                    lblMessage.ForeColor = Color.Red;
                    lblMessage.Text = "Lütfen geçerli bir fiyat girin.";
                    return;
                }

                Category selectedCategory = cmbCategory.SelectedItem as Category;
                Supplier selectedSupplier = cmbSupplier.SelectedItem as Supplier;

                if (selectedCategory == null || selectedSupplier == null)
                {
                    lblMessage.ForeColor = Color.Red;
                    lblMessage.Text = "Kategori veya Tedarikçi seçimi hatalı.";
                    return;
                }

                string productSize = txtProductSize.Text;
                string productColor = txtProductColor.Text;

                // Ürünü veritabanında güncelle
                UpdateProduct(selectedProductId, productName, productPrice, selectedSupplier.Id, selectedCategory.Id, productSize, productColor);

                lblMessage.ForeColor = Color.DarkGreen;
                lblMessage.Text = $"Ürün ID: {selectedProductId} başarıyla güncellendi.";

                ClearProductInputs(); // Alanları temizle ve butonları sıfırla
                GetAllProducts();     // DataGridView'i yenile
            }
            catch (Exception ex)
            {
                HandleException(ex, "Ürün Güncelleme"); // Hata yönetimi
            }
        }

        // Ürün Sil Butonuna Tıklandığında
        private void BtnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (selectedProductId == -1)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "Lütfen önce silinecek bir ürün seçin.";
                return;
            }

            // Kullanıcıya silme onayı sor
            DialogResult dialogResult = MessageBox.Show($"Ürün ID: {selectedProductId} ({txtProductName.Text}) silmek istediğinize emin misiniz? Bu işlem geri alınamaz.", "Ürün Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    DeleteProduct(selectedProductId); // Ürünü sil

                    lblMessage.ForeColor = Color.DarkGreen;
                    lblMessage.Text = $"Ürün ID: {selectedProductId} başarıyla silindi.";

                    ClearProductInputs(); // Alanları temizle ve butonları sıfırla
                    GetAllProducts();     // DataGridView'i yenile
                }
                catch (Exception ex)
                {
                    HandleException(ex, "Ürün Silme"); // Hata yönetimi
                }
            }
        }

        // DB İşlemleri Metotları

        // Yeni Kategori Ekle (şu an UI'da kullanılmıyor ama ihtiyaç olursa var)
        private int AddCategory(string categoryName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Categories (CategoryName) VALUES (@CategoryName); SELECT SCOPE_IDENTITY();";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CategoryName", categoryName);
                        int newId = Convert.ToInt32(command.ExecuteScalar());
                        return newId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Kategori eklenirken bir hata oluştu: {ex.Message}", ex);
            }
        }

        // Yeni Tedarikçi Ekle (şu an UI'da kullanılmıyor ama ihtiyaç olursa var)
        private int AddSupplier(string supplierName, string contactInfo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Suppliers (SupplierName, ContactInfo) VALUES (@SupplierName, @ContactInfo); SELECT SCOPE_IDENTITY();";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SupplierName", supplierName);
                        command.Parameters.AddWithValue("@ContactInfo", contactInfo); // DBNull kontrolü eklenebilir
                        int newId = Convert.ToInt32(command.ExecuteScalar());
                        return newId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Tedarikçi eklenirken bir hata oluştu: {ex.Message}", ex);
            }
        }

        // Yeni Ürün Ekleme Metodu
        private int AddProduct(string name, decimal price, int supplierId, int categoryId, string size, string color)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Products (ProductName, Price, SupplierId, CategoryId, Size, Color) VALUES (@ProductName, @Price, @SupplierId, @CategoryId, @Size, @Color); SELECT SCOPE_IDENTITY();";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductName", name);
                        command.Parameters.AddWithValue("@Price", price);
                        command.Parameters.AddWithValue("@SupplierId", supplierId);
                        command.Parameters.AddWithValue("@CategoryId", categoryId);
                        command.Parameters.AddWithValue("@Size", string.IsNullOrEmpty(size) ? (object)DBNull.Value : size); // Boşsa DBNull olarak kaydet
                        command.Parameters.AddWithValue("@Color", string.IsNullOrEmpty(color) ? (object)DBNull.Value : color); // Boşsa DBNull olarak kaydet
                        int newId = Convert.ToInt32(command.ExecuteScalar());
                        return newId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ürün veritabanına eklenirken bir hata oluştu: {ex.Message}", ex);
            }
        }

        // Ürün Güncelleme Metodu
        private void UpdateProduct(int productId, string name, decimal price, int supplierId, int categoryId, string size, string color)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Products SET ProductName = @ProductName, Price = @Price, SupplierId = @SupplierId, CategoryId = @CategoryId, Size = @Size, Color = @Color WHERE ProductId = @ProductId;";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductName", name);
                        command.Parameters.AddWithValue("@Price", price);
                        command.Parameters.AddWithValue("@SupplierId", supplierId);
                        command.Parameters.AddWithValue("@CategoryId", categoryId);
                        command.Parameters.AddWithValue("@Size", string.IsNullOrEmpty(size) ? (object)DBNull.Value : size);
                        command.Parameters.AddWithValue("@Color", string.IsNullOrEmpty(color) ? (object)DBNull.Value : color);
                        command.Parameters.AddWithValue("@ProductId", productId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ürün güncellenirken bir hata oluştu: {ex.Message}", ex);
            }
        }

        // Ürün Silme Metodu
        private void DeleteProduct(int productId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Foreign Key ilişkisi nedeniyle önce Stock tablosundan ilgili kaydı silmeliyiz (eğer ON DELETE CASCADE yoksa)
                    string deleteStockQuery = "DELETE FROM Stock WHERE ProductId = @ProductId;";
                    using (SqlCommand stockCommand = new SqlCommand(deleteStockQuery, connection))
                    {
                        stockCommand.Parameters.AddWithValue("@ProductId", productId);
                        stockCommand.ExecuteNonQuery();
                    }

                    // Sonra Products tablosundan ürünü sil
                    string deleteProductQuery = "DELETE FROM Products WHERE ProductId = @ProductId;";
                    using (SqlCommand productCommand = new SqlCommand(deleteProductQuery, connection))
                    {
                        productCommand.Parameters.AddWithValue("@ProductId", productId);
                        productCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ürün silinirken bir hata oluştu: {ex.Message}", ex);
            }
        }

        // Stok Ekleme/Güncelleme Metodu
        private void AddStock(int productId, int quantity)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        IF EXISTS (SELECT 1 FROM Stock WHERE ProductId = @ProductId)
                            UPDATE Stock SET Quantity = Quantity + @Quantity, LastUpdated = GETDATE() WHERE ProductId = @ProductId
                        ELSE
                            INSERT INTO Stock (ProductId, Quantity, LastUpdated) VALUES (@ProductId, @Quantity, GETDATE());";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductId", productId);
                        command.Parameters.AddWithValue("@Quantity", quantity);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Stok eklenirken/güncellenirken bir hata oluştu: {ex.Message}", ex);
            }
        }

        // Kategorileri ComboBox'a Yükleme Metodu
        private void LoadCategoriesIntoComboBox()
        {
            categories.Clear(); // Önceki verileri temizle
            cmbCategory.Items.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT CategoryId, CategoryName FROM Categories ORDER BY CategoryName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Category category = new Category { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                                categories.Add(category);
                                cmbCategory.Items.Add(category);
                            }
                        }
                    }
                }
                cmbCategory.DisplayMember = "Name";
                cmbCategory.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                HandleException(ex, "Kategori Yükleme");
            }
        }

        // Tedarikçileri ComboBox'a Yükleme Metodu
        private void LoadSuppliersIntoComboBox()
        {
            suppliers.Clear(); // Önceki verileri temizle
            cmbSupplier.Items.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT SupplierId, SupplierName FROM Suppliers ORDER BY SupplierName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Supplier supplier = new Supplier { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                                suppliers.Add(supplier);
                                cmbSupplier.Items.Add(supplier);
                            }
                        }
                    }
                }
                cmbSupplier.DisplayMember = "Name";
                cmbSupplier.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                HandleException(ex, "Tedarikçi Yükleme");
            }
        }

        // Tüm Ürünleri DataGridView'e Yükleme Metodu
        private void GetAllProducts()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT
                            P.ProductId,
                            P.ProductName,
                            P.Price,
                            C.CategoryName,
                            S.SupplierName,
                            P.Size,      -- Yeni eklendi
                            P.Color,     -- Yeni eklendi
                            ISNULL(ST.Quantity, 0) AS Quantity, -- Stok yoksa 0 göster
                            ST.LastUpdated
                        FROM Products P
                        JOIN Categories C ON P.CategoryId = C.CategoryId
                        JOIN Suppliers S ON P.SupplierId = S.SupplierId
                        LEFT JOIN Stock ST ON P.ProductId = ST.ProductId -- Stok olmayabilir diye LEFT JOIN
                        ORDER BY P.ProductId;
                    ";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dgvProducts.DataSource = dataTable;
                        lblMessage.ForeColor = Color.DarkGreen;
                        if (dataTable.Rows.Count > 0)
                            lblMessage.Text = "Ürünler başarıyla listelendi.";
                        else
                            lblMessage.Text = "Listelenecek ürün bulunamadı.";
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "Ürün Listeleme");
            }
        }

        // Hata Yönetimi İçin Yardımcı Metot
        private void HandleException(Exception ex, string operation)
        {
            lblMessage.ForeColor = Color.Red;
            lblMessage.Text = $"Hata ({operation}): {ex.Message}";
            if (ex.InnerException != null)
            {
                lblMessage.Text += $"\nİç Hata: {ex.InnerException.Message}";
            }
            MessageBox.Show(lblMessage.Text, "Hata Oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Kategori Sınıfı
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name; // ComboBox'ta sadece adı göster
        }
    }

    // Tedarikçi Sınıfı
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // public string ContactInfo { get; set; } // Eğer ComboBox'ta göstermeyeceksen veya başka yerde kullanmayacaksan bu satır gereksiz olabilir. AddSupplier metodunda kullanılıyor.

        public override string ToString()
        {
            return Name; // ComboBox'ta sadece adı göster
        }
    }
}