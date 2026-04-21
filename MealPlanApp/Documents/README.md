# MealPlanApp - Gestionare Plan Alimentar

## Descriere Proiect
Aplicație console în C# (.NET Framework 4.7.2) pentru gestionarea unui plan alimentar cu includerea sugestiilor în baza unor setări prestabilite.

## Structură Proiect
```
MealPlanApp/
├── Models/
│   └── Models.cs          # Clasele de date (FoodItem, User, MealPlan, etc.)
├── Data/
│   └── DatabaseHelper.cs  # Initializare și creare tabele SQLite
├── Services/
│   ├── FoodItemService.cs # CRUD pentru alimente
│   ├── ImportService.cs   # Import date de la provideri (Cerințele 1, 2, 3)
│   ├── MealPlanService.cs # Generare planuri și sugestii (Cerințele 5, 7)
│   ├── EmailService.cs    # Trimitere emailuri (Cerințele 6, 9)
│   └── ReportService.cs   # Generare rapoarte PDF/Excel (Cerinta 10)
├── Scripts/               # Scripturi SQL opționale
├── Documents/             # Documentație suplimentară
├── Program.cs             # Punctul de intrare al aplicației
├── MealPlanApp.csproj     # Fișier proiect .NET 4.7.2
└── packages.config        # Dependințe NuGet
```

## Cerințe Implementate

### 1. Import initial date de la provideri ✅
- `ImportService.InitialImport()` - Rulează o singură dată
- Importă date de la mai mulți provideri simultan
- Verifică duplicatele pe baza de ImageHash

### 2. Actualizare zilnică (Cron) ✅
- `ImportService.DailyUpdateImport()` - Rulează zilnic
- Actualizează înregistrările existente
- Adaugă înregistrări noi
- Include instrucțiuni pentru configurarea cron

### 3. Identificare anunțuri identice pe baza imaginilor ✅
- `ImportService.FindDuplicateImages()`
- Folosește hash SHA256 pentru identificare
- Grupează și afișează duplicatele

### 4. Studiu stocare imagini ✅
- Documentat în `Program.ShowImageStorageStudy()`
- Compară: Bază de date vs Server fișiere vs Cloud storage
- Recomandă abordarea hibridă

### 5. Listare date după specificitate ✅
- `MealPlanService.GetSuggestionsForUser()`
- Filtrează alimente după preferințele utilizatorului
- Afișează sugestii personalizate

### 6. Cron pentru emailuri promoționale ✅
- `EmailService.SendPromotionalEmails()`
- Include configurații pentru Linux crontab și Windows Task Scheduler

### 7. Notificări paralele ✅
- `MealPlanService.CheckAndNotifyUsersAsync()`
- Folosește `Task.WhenAll()` pentru execuție paralelă
- Verifică preferințele fiecărui utilizator

### 8. Studiu hosting (Server vs Serverless) ✅
- Documentat în `Program.ShowHostingStudy()`
- Compară VPS, Serverless, și containere
- Recomandări pentru .NET Framework 4.7.2

### 9. Serviciu asincron ✅
- `EmailService.ProcessEmailQueueAsync()`
- Procesare background pentru coada de emailuri
- Demonstratie async/await

### 10. Generare rapoarte PDF/Excel ✅
- `ReportService.GenerateExcelReport()` - Format CSV
- `ReportService.GeneratePdfReport()` - Text formatat
- `ReportService.GenerateNutritionReport()` - Raport nutrițional detaliat

### 11. Statistici consumatoare de timp ✅
- `Program.RunTimeConsumingStatistics()`
- Calculează agregări complexe
- Măsoară timpul de execuție

## Cum să rulați proiectul

### Cerințe
- Visual Studio 2019+ sau VS Code
- .NET Framework 4.7.2 SDK
- NuGet package: System.Data.SQLite

### Instalare dependințe
**Metoda 1 - Din Visual Studio (Recomandat):**
1. Deschideți soluția MealPlanApp.sln în Visual Studio
2. Click dreapta pe proiect → "Manage NuGet Packages"
3. Căutați "System.Data.SQLite" și instalați-l
4. Sau din Package Manager Console:
```
Install-Package System.Data.SQLite
```

**Metoda 2 - Restaurare automată:**
Visual Studio va restaura automat pachetele la primul build dacă proiectul este deschis corect.

### Rulare
1. Deschideți soluția în Visual Studio
2. Așteptați restaurarea pachetelor NuGet (se vede în Output window)
3. Build (F6 sau Build → Build Solution)
4. Run (F5 sau Debug → Start Debugging)

### Rezolvare erori comune

**Eroarea: "The name 'DatabaseHelper' does not exist in the current context"**
- Cauză: Fișierul DatabaseHelper.cs nu este inclus în proiect sau namespace-ul este greșit
- Soluție: 
  1. Verificați că `Data/DatabaseHelper.cs` există în proiect
  2. În Visual Studio, click dreapta pe proiect → Add → Existing Item → selectați DatabaseHelper.cs
  3. Asigurați-vă că `using MealPlanApp.Data;` este prezent în Program.cs
  4. Rebuild solution (Build → Clean Solution, apoi Build → Rebuild Solution)

**Eroarea: "Could not load file or assembly 'System.Data.SQLite'"**
- Cauză: Pachetul NuGet nu este instalat
- Soluție: Instalați pachetul System.Data.SQLite din NuGet Package Manager

**Eroarea: "The type or namespace name 'SQLiteConnection' could not be found"**
- Cauză: Lipsa referinței către System.Data.SQLite
- Soluție: 
  1. Install-Package System.Data.SQLite din Package Manager Console
  2. Rebuild solution

### Meniu Interactiv
Aplicația oferă un meniu interactiv cu toate funcționalitățile numerotate conform cerințelor.

## Configurare Cron Jobs

### Linux (crontab)
```bash
# Editare crontab
crontab -e

# Actualizare zilnică la 02:00
0 2 * * * /usr/bin/dotnet /opt/mealplan/MealPlanApp.dll --action=daily-update

# Emailuri promoționale în fiecare Luni la 09:00
0 9 * * 1 /usr/bin/dotnet /opt/mealplan/MealPlanApp.dll --action=email
```

### Windows Task Scheduler
1. Deschideți Task Scheduler
2. Create Basic Task
3. Configurați trigger-ul (zilnic/săptămânal)
4. Acțiune: `dotnet.exe`
5. Argumente: `C:\MealPlanApp\MealPlanApp.dll --action=daily-update`

## Baza de Date
Aplicația folosește SQLite cu următoarele tabele:
- **Providers** - Informații despre furnizorii de date
- **FoodItems** - Alimente cu informații nutriționale
- **Users** - Utilizatori și preferințe
- **MealPlans** - Planuri alimentare
- **MealPlanItems** - Iteme din planurile alimentare
- **UserPreferences** - Preferințe utilizatori
- **Notifications** - Notificări
- **ImportLogs** - Log-uri importuri

## Arhitectură
- **Pattern**: Repository + Service Layer
- **Bază de date**: SQLite (dezvoltare), SQL Server/PostgreSQL (producție)
- **Stocare imagini**: Path în BD + fișiere pe server (recomandat)
- **Async**: async/await pentru operațiuni I/O

## Autor
Generat de Asistent AI pentru proiect universitar/demo
