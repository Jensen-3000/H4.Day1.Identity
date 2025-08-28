# Serversideprogrammering opgave

---

## Indhold

- [Certifikater](#certifikater)
  - [Hurtig kopi med dotnet dev-certs](#1-hurtig-kopi-med-dotnet-dev-certs)
  - [Custom certifikat med PowerShell](#2-custom-certifikat-med-powershell)
  - [Opsætning af secrets.json](#opsætning-af-secretsjson-til-custom-certifikat)
- [Entity Framework Migrationer](#entity-framework-migrationer)
- [Easy-To-Copy-Paste brugere](#easy-to-copy-paste-brugere)
- [Keys-mappe](#keys-mappe)

---

## Certifikater

Der er to måder at oprette certifikater på:

### 1. Hurtig kopi med dotnet dev-certs
**Bemærk:** Mapperne `%USERPROFILE%\.aspnet\https` skal være oprettet før du kører kommandoen.    
Certifikatet skal efterfølgende flyttes til projektets `Cert`-mappe.

Standard certifikat uden tilpasning.

```sh
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\Jensen.pfx -p Jensen@1
```
- `-ep`: Export profile
- `-p`: Password

Flyt den oprettede `.pfx` fil til projektets `Cert`-mappe.

---

### 2. Custom certifikat med PowerShell
**Bemærk:** Mapperne `%USERPROFILE%\.aspnet\https` skal være oprettet før du kører kommandoerne.    
Certifikatet skal efterfølgende flyttes til projektets `Cert`-mappe.

Tilpas certifikatets egenskaber (Owner, FriendlyName, udløbsdato osv.).

Kør disse PowerShell kommandoer:

```powershell
$cert = New-SelfSignedCertificate -DnsName "localhost" -CertStoreLocation "cert:\CurrentUser\My" -FriendlyName "My Custom Blazor Cert" -NotAfter (Get-Date).AddMonths(2) -Subject "CN=.CustomBoys, O=Jensen, OU=CertBoys"
```
```powershell
$pwd = ConvertTo-SecureString -String "Jensen@1" -Force -AsPlainText
```
```powershell
Export-PfxCertificate -Cert $cert -FilePath "$env:USERPROFILE\.aspnet\https\MyBlazorApp.pfx" -Password $pwd
```
- Installer certifikatet i både Personal og Trusted stores (CurrentUser).
- Det kan gøres med `certmgr.msc` kommandoen. Og så ellers copy-paste fra personal ind i trusted root.

Flyt den oprettede `.pfx` fil til projektets `Cert`-mappe.

---

### Opsætning af secrets.json til custom certifikat
Hvis du bruger et custom certifikat, tilføj følgende til din `secrets.json`:

```json
{
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://localhost:7294",
        "Certificate": {
          "Path": "Cert/MyBlazorApp.pfx",
          "Password": "Jensen@1"
        }
      }
    }
  }
}
```
Dette sikrer at Kestrel bruger dit custom certifikat.

---

## Entity Framework Migrationer

### Tilføj migrationer
```sh
Add-Migration "Init AppDb" -Context ApplicationDbContext -OutputDir Data/Migrations/AppDb
```
```sh
Add-Migration "Init TodoDb" -Context TodoDbContext -OutputDir Data/Migrations/TodoDb
```

### Opdater database
```sh
Update-Database -Context ApplicationDbContext
```
```sh
Update-Database -Context TodoDbContext
```

### Slet database
```sh
Drop-Database -Context ApplicationDbContext
```
```sh
Drop-Database -Context TodoDbContext
```

---

## Easy-To-Copy-Paste brugere

```txt
Admin@test.dk
```
```txt
Passw0rd!
```
```txt
User@test.dk
```
```txt
Passw0rd!
```

---

## Keys-mappe

Projektets `Keys`-mappe bruges til at gemme private og public keys til asymmetrisk kryptering.
