namespace ProductManagement41
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // Form1.cs dosyasındaki constructor (yapıcı metot) ve CreateUIElements() metodu
            // formun temel ayarlarını (boyut, başlık vb.) ve tüm UI elemanlarını
            // zaten programatik olarak oluşturduğu için InitializeComponent metodu
            // burada çok az iş yapar veya sadece Visual Studio tasarımcısının
            // ihtiyaç duyduğu temel iskeleti sağlar.

            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            // 
            // Form1
            // 
            // AutoScaleMode ve AutoScaleDimensions gibi özellikler genellikle
            // projenin varsayılan ayarlarına göre otomatik olarak ayarlanır
            // veya Form1.cs constructor'ında elle düzenlenebilir.
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F); // Bu değeri projenizin ayarlarına göre değiştirebilirsiniz.
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            // Formun adı (Name özelliği) genellikle burada ayarlanır.
            this.Name = "Form1";
            // Formun başlığı (Text) ve boyutu (ClientSize/Size) gibi özellikler
            // Form1.cs içerisindeki constructor'da ayarlanmaktadır.
            // this.Text = "Ürün Yönetim Sistemi"; // Form1.cs constructor'ında ayarlanıyor.
            // this.ClientSize = new System.Drawing.Size(1200, 700); // Form1.cs constructor'ında ayarlanıyor.
            this.ResumeLayout(false);

        }

        #endregion

        // Not: UI elemanları (Button, TextBox, DataGridView vb.) için private alan tanımlamaları
        // (örneğin: private System.Windows.Forms.Button btnAddProduct;)
        // Form1.cs dosyasında zaten yapıldığı için bu Designer.cs dosyasında tekrar yer almaz.
        // Eğer Visual Studio'nun form tasarımcısını kullansaydınız, bu tanımlamalar burada olurdu.
    }
}