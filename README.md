# ConfigSystem

Bu proje, merkezi bir **Configuration Management System** örneğidir.  
Farklı servislerin (ServiceA, ServiceB) ortak bir **ConfigurationReader kütüphanesi** üzerinden veritabanındaki konfigürasyonlarını okumasını sağlar.  
Ayrıca bir **Admin Panel (MVC)** üzerinden bu ayarların **CRUD** işlemleri yapılabilir.


##  Proje Yapısı

├─ Config.Abstractions/ → Ortak interface’ler (IConfigStore, IConfigRepository vb.)

├─ Config.Data/ → Entity Framework (EF) ile DB erişim katmanı

│ └─ Ef/ → DbContext, EfConfigStore, Repository implementasyonları

├─ ConfigurationReader/ → Servislerin konfigürasyonu okumasını sağlayan kütüphane

├─ ServiceA/ → Örnek servis (konfigürasyonu DB’den okur)

├─ ServiceB/ → Örnek servis (konfigürasyonu DB’den okur)

├─ ConfigAdminPanel/ → Admin panel (UI, MVC)

└─Init.sql → Veritabanı oluşturma scripti

##  Gereksinimler

- .NET 8 SDK
- SQL Server

##  Veritabanı Kurulumu

Proje `ConfigDb` isimli bir veritabanı kullanır.  
`Init.sql` scriptini çalıştırarak tabloyu oluşturun.

## Çalıştırma

Proje klasöründe şu komutları çalıştırın:

### Windows(PowerShell)


dotnet build
Start-Process powershell -ArgumentList 'dotnet run --project ServiceA --urls http://localhost:5000'

Start-Process powershell -ArgumentList 'dotnet run --project ServiceB --urls http://localhost:5001'

Start-Process powershell -ArgumentList 'dotnet run --project ConfigAdminPanel --urls http://localhost:5002'

### Visual Studio


Solution’a sağ tık → Set Startup Projects…

Multiple startup projects seçin

ServiceA, ServiceB, ConfigAdminPanel için Start işaretleyin

F5 ile hepsi aynı anda çalıştırın.

## Test

http://localhost:5000/ → ServiceA

http://localhost:5001/ → ServiceB

http://localhost:5002/ → Config Admin Panel

Config değerlerini almak için:

http://localhost:5000/config/{ConfigName}

http://localhost:5001/config/{ConfigName}

