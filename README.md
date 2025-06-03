# Giyim Mağazası Yönetim Sistemi

Bu proje, C# Windows Forms ve SQL Server kullanılarak geliştirilmiş bir masaüstü giyim mağazası ürün yönetim sistemidir. Temel CRUD (Oluşturma, Okuma, Güncelleme, Silme) işlemlerini destekler.

## Açıklama

Uygulama, bir giyim mağazasında bulunan ürünlerin (tişörtler, pantolonlar, elbiseler, ayakkabılar vb.) kayıt altına alınmasını, mevcut ürünlerin listelenmesini, seçilen ürünlerin bilgilerinin güncellenmesini ve sistemden kaldırılmasını amaçlar. Ürünlerin fiyat, kategori, tedarikçi, beden ve renk gibi temel detayları bu sistem üzerinden yönetilebilir.

## Temel Özellikler

* **Ürün Ekleme:** Yeni ürün bilgilerini (ad, fiyat, kategori, tedarikçi, beden, renk) sisteme kaydetme.
* **Ürün Listeleme:** Sistemde kayıtlı tüm ürünleri detaylarıyla (stok miktarı dahil) görüntüleme.
* **Ürün Güncelleme:** Mevcut bir ürünün bilgilerini düzenleme ve kaydetme.
* **Ürün Silme:** Seçilen bir ürünü (ilgili stok bilgisiyle birlikte) sistemden kaldırma.
* **Dinamik Veri Yükleme:** Kategori ve tedarikçi bilgilerini veritabanından çekerek ComboBox'larda gösterme.
* **Basit Stok Yönetimi:** Eklenen her yeni ürüne varsayılan bir stok miktarı atama.

## Kullanılan Teknolojiler

* **Programlama Dili:** C#
* **Arayüz Teknolojisi:** Windows Forms (.NET Framework veya .NET Core/5+)
* **Veritabanı:** Microsoft SQL Server (örneğin, SQL Server Express)
* **Veritabanı Bağlantı Kütüphanesi:** `Microsoft.Data.SqlClient` (NuGet paketi)
* **Geliştirme Ortamı (IDE):** Visual Studio

## Kurulum ve Çalıştırma

Projeyi kendi bilgisayarınızda çalıştırmak için aşağıdaki adımları izleyebilirsiniz:

1.  **Ön Gereksinimler:**
    * .NET Runtime (projenizin kullandığı sürüm)
    * Microsoft SQL Server (Express sürümü yeterlidir)
    * Visual Studio (Projeyi açmak ve derlemek için)

2.  **Veritabanı Ayarları:**
    * SQL Server Management Studio (SSMS) veya benzeri bir araç ile SQL Server'ınıza bağlanın.
    * `ProductManagementDB` adında yeni bir veritabanı oluşturun.
    * Daha önce paylaşılan SQL script'ini (tablo oluşturma ve giyim kategorileri için örnek veri ekleme script'i) bu veritabanı üzerinde çalıştırın. Bu script `Categories`, `Suppliers`, `Products` ve `Stock` tablolarını oluşturacaktır.
    * Projedeki `Form1.cs` dosyasında bulunan `connectionString` değişkenini kendi SQL Server bağlantı ayarlarınıza göre güncelleyin:
        ```csharp
        private string connectionString = "Server=SUNUCU_ADINIZ\\SQLEXPRESS_INSTANCE_ADINIZ;Database=ProductManagementDB;Integrated Security=True;TrustServerCertificate=True";
        ```
        (Örneğin: `Server=VEYSELPC\\SQLEXPRESS;Database=ProductManagementDB;Integrated Security=True;TrustServerCertificate=True`)

3.  **Uygulamayı Başlatma:**
    * Projeyi (`final projem.sln` dosyasını) Visual Studio ile açın.
    * Gerekli NuGet paketlerinin (özellikle `Microsoft.Data.SqlClient`) geri yüklendiğinden/yüklü olduğundan emin olun. (Solution Explorer'da projeye sağ tıklayıp "Manage NuGet Packages..." ile kontrol edebilirsiniz.)
    * Projeyi derleyin (Build) ve ardından çalıştırın (Start).



## Gelecekte Eklenebilecekler (İsteğe Bağlı)

* Kullanıcı girişi ve yetkilendirme.
* Daha gelişmiş arama ve filtreleme seçenekleri.
* Satış ve sipariş yönetimi modülü.
* Raporlama özellikleri (örn: en çok satılan ürünler, stok durumu raporları).

---

Bu README dosyasını projenin ana klasörüne (`final projem.sln` ve `.gitignore` dosyalarının olduğu yere) `README.md` olarak kaydet. GitHub bu dosyayı otomatik olarak tanıyacak ve proje sayfanın altında gösterecektir.

İçeriği kendine göre daha da özelleştirebilirsin!
