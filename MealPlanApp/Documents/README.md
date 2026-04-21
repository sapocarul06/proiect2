# MealPlanApp - Aplicație Gestionare Plan Alimentar

## Descriere Proiect
Aplicație console în C# (.NET Framework 4.7.2) pentru gestionarea planurilor alimentare cu sugestii personalizate bazate pe preferințele utilizatorilor.

## Cerințe Implementate

### ✅ Cerința 1: Import inițial de la mai mulți provideri
- **Fișier:** `Services/InitialImportService.cs`
- Importă date de la 3 provideri diferiți (ProviderA, ProviderB, ProviderC)
- Rulează o singură dată la început

### ✅ Cerința 2: Actualizare zilnică cu cron
- **Fișier:** `Services/DailyUpdateService.cs`
- Script care rulează zilnic pentru actualizarea datelor
- Include configurație cron pentru Linux și Windows Task Scheduler

### ✅ Cerința 3: Identificare duplicate pe baza imaginilor
- **Fișier:** `Services/ImageDuplicateDetector.cs`
- Folosește hash SHA256 pentru a detecta imagini identice
- Previne duplicarea anunțurilor/alimentelor

### ✅ Cerința 4: Studiu stocare imagini
- **Fișier:** `Services/ImageDuplicateDetector.cs` (metoda `DisplayImageStorageStudy`)
- Compară 3 metode: Bază de date (BLOB), File System, Cloud Storage
- Recomandare: Cloud Storage (Azure Blob/AWS S3) + metadata în DB

### ✅ Cerința 5: Listare sugestii personalizate
- **Fișier:** `Services/NotificationService.cs`
- Filtrează alimentele în funcție de preferințele utilizatorului
- Calorii, proteine, tip dietă, ingrediente excluse

### ✅ Cerința 6: Cron pentru emailuri promoționale
- **Fișier:** `Program.cs` (metoda `DisplayEmailCronConfig`)
- Configurare crontab pentru Linux
- Configurare Windows Task Scheduler
- Exemple de programare: zilnic, săptămânal, de mai multe ori pe zi

### ✅ Cerința 7: Notificări paralele (fire de execuție)
- **Fișier:** `Services/NotificationService.cs`
- Folosește `async/await` și `Task.WhenAll`
- Procesează notificări pentru mai mulți utilizatori în paralel

### ✅ Cerința 8: Studiu găzduire aplicație
- **Fișier:** `Services/AsyncProcessingService.cs` (metoda `DisplayHostingStudy`)
- Compară: Local, VPS, Cloud Tradițional, Serverless
- Arhitectură recomandată cu Azure Functions + Azure SQL + Blob Storage

### ✅ Cerința 9: Serviciu asincron
- **Fișier:** `Services/AsyncProcessingService.cs`
- Procesare background pentru analiză nutrițională
- Calcul scor nutrițional în mod asincron

### ✅ Cerința 10: Generare rapoarte PDF/Excel
- **Fișier:** `Services/ReportService.cs`
- Excel: Format CSV cu toate alimentele
- PDF: Raport text formatat cu statistici (în producție se poate folosi iTextSharp sau QuestPDF)

### ✅ Cerința 11: Statistici consumatoare de timp
- **Fișier:** `Services/AsyncProcessingService.cs` (metoda `RunPerformanceStatistics`)
- Măsoară timpul de execuție pentru operații DB, filtrare, sortare
- Oferă recomandări de optimizare

## Structura Proiectului

```
MealPlanApp/
├── Models/
│   ├── Food.cs              # Model pentru alimente
│   ├── User.cs              # Model pentru utilizatori
│   └── UserPreference.cs    # Preferințe utilizator
├── Data/
│   └── DatabaseHelper.cs    # Acces la baza de date SQLite
├── Services/
│   ├── InitialImportService.cs      # Cerința 1
│   ├── DailyUpdateService.cs        # Cerința 2
│   ├── ImageDuplicateDetector.cs    # Cerința 3, 4
│   ├── NotificationService.cs       # Cerința 5, 7
│   ├── ReportService.cs             # Cerința 10
│   └── AsyncProcessingService.cs    # Cerința 8, 9, 11
├── Program.cs               # Meniu principal
├── MealPlanApp.csproj       # Fișier proiect
├── packages.config          # Dependințe NuGet
└── Documents/
    └── README.md            # Această documentație
```

## Instalare și Rulare

### Pasul 1: Deschideți proiectul în Visual Studio
1. Deschideți Visual Studio 2019 sau 2022
2. File → Open → Project/Solution
3. Selectați `MealPlanApp/MealPlanApp.csproj`

### Pasul 2: Instalați pachetul NuGet
1. Click dreapta pe proiect → Manage NuGet Packages
2. Căutați `System.Data.SQLite.Core`
3. Instalați versiunea 1.0.118.0

**SAU** din Package Manager Console:
```
Install-Package System.Data.SQLite.Core -Version 1.0.118.0
```

### Pasul 3: Build și Run
1. Build → Build Solution (sau Ctrl+Shift+B)
2. Debug → Start Debugging (sau F5)

### Pasul 4: Utilizare
1. Selectați opțiunea **1** pentru import inițial
2. Explorați celelalte funcționalități din meniu

## Configurație Cron (Actualizare Zilnică)

### Linux
```bash
crontab -e
# Adăugați linia:
0 2 * * * /usr/bin/mono /opt/mealplan/MealPlanApp.exe daily-update >> /var/log/mealplan-daily.log 2>&1
```

### Windows Task Scheduler
1. Deschideți Task Scheduler
2. Create Basic Task → "MealPlan Daily Update"
3. Trigger: Daily at 2:00 AM
4. Action: Start a program
5. Program: `C:\Program Files\MealPlanApp\MealPlanApp.exe`
6. Arguments: `daily-update`

## Tehnologii Folosite
- **Limbaj:** C#
- **Framework:** .NET Framework 4.7.2
- **Bază de date:** SQLite (System.Data.SQLite)
- **Pattern-uri:** Repository, Service Layer, Async/Await
- **Criptare:** SHA256 pentru hash imagini

## Autor
Proiect realizat pentru tema: "Proiectarea și implementarea unei aplicații de gestionare a unui plan alimentar cu includerea sugestiilor în baza unor setări prestabilite"
