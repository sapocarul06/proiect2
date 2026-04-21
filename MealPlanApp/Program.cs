using System;
using System.Collections.Generic;
using System.IO;
using MealPlanApp.Data;
using MealPlanApp.Models;
using MealPlanApp.Services;

namespace MealPlanApp
{
    /// <summary>
    /// Aplicatie de gestionare a unui plan alimentar cu includerea sugestiilor 
    /// in baza unor setari prestabilite
    /// 
    /// Autor: Asistent AI
    /// Framework: .NET Framework 4.7.2
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("   MEAL PLAN APP - Gestionare Plan Alimentar");
            Console.WriteLine("   .NET Framework 4.7.2");
            Console.WriteLine("=================================================\n");

            // Initializare baza de date
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MealPlanDB.sqlite");
            DatabaseHelper.Initialize(dbPath);
            Console.WriteLine($"Baza de date initializata: {dbPath}\n");

            bool exit = false;
            
            while (!exit)
            {
                DisplayMainMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RunInitialImport();
                        break;
                    case "2":
                        RunDailyUpdate();
                        break;
                    case "3":
                        FindDuplicateImages();
                        break;
                    case "4":
                        ShowImageStorageStudy();
                        break;
                    case "5":
                        ListDataBySpecificity();
                        break;
                    case "6":
                        ShowCronConfiguration();
                        break;
                    case "7":
                        RunParallelNotifications();
                        break;
                    case "8":
                        ShowHostingStudy();
                        break;
                    case "9":
                        RunAsyncService();
                        break;
                    case "10":
                        GenerateReports();
                        break;
                    case "11":
                        RunTimeConsumingStatistics();
                        break;
                    case "0":
                        exit = true;
                        Console.WriteLine("\nLa revedere!");
                        break;
                    default:
                        Console.WriteLine("\nOptiune invalida. Incercati din nou.\n");
                        break;
                }
            }
        }

        static void DisplayMainMenu()
        {
            Console.WriteLine("\n=== MENIU PRINCIPAL ===");
            Console.WriteLine("1. Import initial date de la provideri (Cerinta 1)");
            Console.WriteLine("2. Actualizare zilnica date - Cron (Cerinta 2)");
            Console.WriteLine("3. Identificare anunturi identice pe baza imaginilor (Cerinta 3)");
            Console.WriteLine("4. Studiu stocare imagini (Cerinta 4)");
            Console.WriteLine("5. Listare date dupa specificitate (Cerinta 5)");
            Console.WriteLine("6. Configurare Cron emailuri promotionale (Cerinta 6)");
            Console.WriteLine("7. Notificari paralele pentru utilizatori (Cerinta 7)");
            Console.WriteLine("8. Studiu hosting: Server vs Serverless (Cerinta 8)");
            Console.WriteLine("9. Serviciu asincron (Cerinta 9)");
            Console.WriteLine("10. Generare rapoarte PDF/Excel (Cerinta 10)");
            Console.WriteLine("11. Statistici consumatoare de timp (Cerinta 11)");
            Console.WriteLine("0. Iesire");
            Console.Write("\nAlegeti o optiune: ");
        }

        static void RunInitialImport()
        {
            // Cerinta 1: Import initial de la mai multi provideri
            var importService = new ImportService();
            
            var providers = new List<Provider>
            {
                new Provider { Id = 1, Name = "NutriData", ApiUrl = "https://api.nutridata.com", IsActive = true },
                new Provider { Id = 2, Name = "FoodBase", ApiUrl = "https://api.foodbase.com", IsActive = true },
                new Provider { Id = 3, Name = "MealInfo", ApiUrl = "https://api.mealinfo.com", IsActive = true }
            };
            
            // Inserare provideri in BD
            InsertProviders(providers);
            
            importService.InitialImport(providers);
        }

        static void RunDailyUpdate()
        {
            // Cerinta 2: Actualizare zilnica
            var importService = new ImportService();
            
            var providers = GetActiveProviders();
            importService.DailyUpdateImport(providers);
            
            Console.WriteLine("\nNOTA: Pentru automatizare, configurati un cron job:");
            Console.WriteLine("Linux: 0 2 * * * /usr/bin/dotnet MealPlanApp.dll --action=daily-update");
            Console.WriteLine("Windows: Task Scheduler -> Luni-Vineri 02:00");
        }

        static void FindDuplicateImages()
        {
            // Cerinta 3: Identificare duplicate pe baza de imagini
            var importService = new ImportService();
            importService.FindDuplicateImages();
        }

        static void ShowImageStorageStudy()
        {
            // Cerinta 4: Studiu de caz stocare imagini
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("STUDIU DE CAZ: STOCAREA IMAGINILOR");
            Console.WriteLine(new string('=', 60));
            
            Console.WriteLine(@"
OPTIUNEA 1: STOCARE IN BAZA DE DATE (BLOB)
-------------------------------------------
Avantaje:
  + Backup simplificat (totul intr-un singur loc)
  + Tranzactii consistente
  + Control centralizat al accesului
  + Fisierele nu se pierd niciodata

Dezavantaje:
  - Performanta scazuta la citire/scriere
  - Baza de date devine foarte mare
  - Consum mare de memorie
  - Dificil de scalat

Recomandare: Doar pentru imagini mici (< 100KB)


OPTIUNEA 2: STOCARE PE SERVER DE FISIERE
-------------------------------------------
Avantaje:
  + Performanta ridicata la citire
  + Scalabilitate orizontala posibila
  + CDN integration usoara
  + Cost redus de stocare

Dezavantaje:
  - Backup separat necesar
  - Posibile probleme de consistenta
  - Management suplimentar al fisierelor

Recomandare: Ideal pentru aplicatii medii/mari


OPTIUNEA 3: STOCARE IN CLOUD (S3, Azure Blob, etc.)
-------------------------------------------
Avantaje:
  + Scalabilitate infinita
  + Redundanta incorporata
  + CDN integration nativa
  + Cost pay-as-you-go
  + Fara management de infrastructura

Dezavantaje:
  - Dependenta de provider extern
  - Costuri pot creste la volum mare
  - Latenta network

Recomandare: Cel mai bun pentru productia moderna


SOLUTIA RECOMANDATA PENTRU ACEST PROIECT:
-------------------------------------------
Hybrid Approach:
1. Stocare fisiere pe server/cloud (path in BD)
2. Hash MD5/SHA256 al imaginii in baza de date
3. Cache Redis/Memcached pentru imagini populare
4. CDN pentru delivery global

Exemplu structura:
- DB: ImageHash, ImageUrl (path relativ)
- File System: /uploads/{year}/{month}/{hash}.jpg
");
            Console.WriteLine("\nApasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        static void ListDataBySpecificity()
        {
            // Cerinta 5: Listare date dupa specificitate
            var mealPlanService = new MealPlanService();
            
            // Creare user demo daca nu exista
            int userId = CreateDemoUser();
            
            var user = new User 
            { 
                Id = userId, 
                Name = "User Demo", 
                DietaryPreference = "Balanced",
                TargetCalories = 2000 
            };
            
            mealPlanService.GetSuggestionsForUser(user);
            
            // Creare plan alimentar demo
            var plan = mealPlanService.CreateMealPlan(userId, "Plan Saptamanal", 7, "Balanced");
            mealPlanService.DisplayMealPlan(plan.Id);
        }

        static void ShowCronConfiguration()
        {
            // Cerinta 6: Configurare Cron
            EmailService.DisplayCronConfiguration();
            
            Console.WriteLine("\nDoriti sa simulati trimiterea de emailuri? (y/n)");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                var emailService = new EmailService();
                emailService.SendPromotionalEmails();
            }
        }

        static async void RunParallelNotifications()
        {
            // Cerinta 7: Notificari paralele
            var mealPlanService = new MealPlanService();
            
            // Creare useri demo
            CreateDemoUsers();
            
            // Simulare aliment nou adaugat
            var newItem = new FoodItem
            {
                Name = "Avocado Organic",
                Category = "Legume",
                Calories = 160,
                Protein = 2,
                Carbs = 9,
                Fat = 15
            };
            
            await mealPlanService.CheckAndNotifyUsersAsync(newItem);
        }

        static void ShowHostingStudy()
        {
            // Cerinta 8: Studiu hosting Server vs Serverless
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("STUDIU COMPARATIV: SERVER VS SERVERLESS");
            Console.WriteLine(new string('=', 60));
            
            Console.WriteLine(@"
ARHITECTURA TRADITIONALA (SERVER/VPS)
-------------------------------------------
Exemple: AWS EC2, DigitalOcean Droplet, Azure VM

Avantaje:
  + Control complet asupra mediului
  + Cost predictibil pentru trafic constant
  + Fara cold starts
  + Compatibilitate maxima (.NET Framework 4.7.2)
  + Debugging usor

Dezavantaje:
  - Management de infrastructura necesar
  - Scalare manuala sau complexa
  - Plata si pentru timp idle
  - Backup si security manual

Cost estimat: $10-50/luna (VPS mediu)


ARHITECTURA SERVERLESS
-------------------------------------------
Exemple: AWS Lambda, Azure Functions, Google Cloud Functions

Avantaje:
  + Fara management de infrastructura
  + Scalare automata
  + Pay-per-execution (cost mic la trafic variabil)
  + High availability incorporat
  + Deployment simplificat

Dezavantaje:
  - Cold start latency (1-5 secunde)
  - Limitari de runtime (max 15 min AWS)
  - .NET Framework 4.7.2 NU este suportat (doar .NET Core/.NET 5+)
  - Debugging mai dificil
  - Cost poate fi mare la trafic constant ridicat

Cost estimat: $0-100+/luna (variabil)


CONTAINERE (DOCKER + KUBERNETES)
-------------------------------------------
Exemple: AWS ECS/EKS, Azure AKS, Google GKE

Avantaje:
  + Portabilitate maxima
  + Scalare automata
  + Utilizare eficienta a resurselor
  + CI/CD integration

Dezavantaje:
  - Complexitate ridicata
  - Curba de invatare abrupta
  - Overhead pentru aplicatii mici

Cost estimat: $20-200+/luna


RECOMANDARE PENTRU ACEST PROIECT:
-------------------------------------------
Deoarece aplicatia foloseste .NET Framework 4.7.2:

OPTEAZA PENTRU SERVER TRADITIONAL:
- Windows VPS sau Linux cu Mono
- IIS sau Kestrel pentru hosting
- SQLite pentru dezvoltare, SQL Server/PostgreSQL pentru productie

Pentru migrare la serverless in viitor:
- Migrati la .NET 6/8 (LTS)
- Refactorizati pentru stateless execution
- Folositi Azure Functions (cel mai bun support .NET)
");
            Console.WriteLine("\nApasati orice tasta pentru a continua...");
            Console.ReadKey();
        }

        static async void RunAsyncService()
        {
            // Cerinta 9: Serviciu asincron
            var emailService = new EmailService();
            
            Console.WriteLine("\n=== DEMONSTRATIE SERVICIU ASINCRON ===");
            Console.WriteLine("Se proceseaza coada de emailuri in background...\n");
            
            await emailService.ProcessEmailQueueAsync();
            
            Console.WriteLine("\nServiciul asincron s-a incheiat.");
        }

        static void GenerateReports()
        {
            // Cerinta 10: Generare rapoarte
            var reportService = new ReportService();
            
            string reportsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
            Directory.CreateDirectory(reportsDir);
            
            // Generare raport Excel (CSV)
            string excelPath = Path.Combine(reportsDir, $"FoodItems_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            reportService.GenerateExcelReport(excelPath);
            
            // Generare raport PDF (text)
            string pdfPath = Path.Combine(reportsDir, $"SummaryReport_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            reportService.GeneratePdfReport(pdfPath);
            
            // Generare raport nutritional (daca exista planuri)
            var plans = GetAllMealPlans();
            if (plans.Count > 0)
            {
                string nutritionPath = Path.Combine(reportsDir, $"NutritionReport_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                reportService.GenerateNutritionReport(plans[0].Id, nutritionPath);
            }
            
            Console.WriteLine($"\nRapoartele au fost salvate in: {reportsDir}");
        }

        static void RunTimeConsumingStatistics()
        {
            // Cerinta 11: Statistici consumatoare de timp
            Console.WriteLine("\n=== STATISTICI CONSUMATOARE DE TIMP ===\n");
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            Console.WriteLine("Se calculeaza statistici complexe...\n");
            
            // Simulare operatiuni intensive
            var stats = CalculateComplexStatistics();
            
            stopwatch.Stop();
            
            Console.WriteLine($"\n--- REZULTATE ---");
            Console.WriteLine($"Total alimente procesate: {stats.TotalItems}");
            Console.WriteLine($"Media calorii per categorie:");
            foreach (var cat in stats.AverageCaloriesByCategory)
            {
                Console.WriteLine($"  {cat.Key}: {cat.Value:F2} kcal");
            }
            Console.WriteLine($"\nTimp de executie: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"\nAceasta operatiune este 'consumatoare de timp' pentru ca:");
            Console.WriteLine("  - Proceseaza toate inregistrarile din baza de date");
            Console.WriteLine("  - Calculeaza agregari complexe");
            Console.WriteLine("  - Poate fi optimizata prin caching sau pre-calculare");
        }

        #region Helper Methods

        static void InsertProviders(List<Provider> providers)
        {
            using (var connection = new System.Data.SQLite.SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                foreach (var provider in providers)
                {
                    var cmd = new System.Data.SQLite.SQLiteCommand(@"
                        INSERT OR IGNORE INTO Providers (Id, Name, ApiUrl, ApiKey, IsActive)
                        VALUES (@Id, @Name, @ApiUrl, @ApiKey, @IsActive)", connection);
                    
                    cmd.Parameters.AddWithValue("@Id", provider.Id);
                    cmd.Parameters.AddWithValue("@Name", provider.Name);
                    cmd.Parameters.AddWithValue("@ApiUrl", provider.ApiUrl);
                    cmd.Parameters.AddWithValue("@ApiKey", "");
                    cmd.Parameters.AddWithValue("@IsActive", provider.IsActive ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        static List<Provider> GetActiveProviders()
        {
            var providers = new List<Provider>();
            using (var connection = new System.Data.SQLite.SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                var cmd = new System.Data.SQLite.SQLiteCommand("SELECT * FROM Providers WHERE IsActive = 1", connection);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        providers.Add(new Provider
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            ApiUrl = reader["ApiUrl"]?.ToString(),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        });
                    }
                }
            }
            return providers;
        }

        static int CreateDemoUser()
        {
            using (var connection = new System.Data.SQLite.SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                
                // Verificare daca exista deja
                var cmd = new System.Data.SQLite.SQLiteCommand("SELECT Id FROM Users LIMIT 1", connection);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                
                // Creare user demo
                cmd = new System.Data.SQLite.SQLiteCommand(@"
                    INSERT INTO Users (Name, Email, DietaryPreference, TargetCalories, NotificationsEnabled, CreatedAt)
                    VALUES ('User Demo', 'demo@mealplan.com', 'Balanced', 2000, 1, @CreatedAt)", connection);
                
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                cmd.ExecuteNonQuery();
                
                cmd = new System.Data.SQLite.SQLiteCommand("SELECT last_insert_rowid()", connection);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        static void CreateDemoUsers()
        {
            using (var connection = new System.Data.SQLite.SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                
                var users = new[]
                {
                    new { Name = "Ana Popescu", Email = "ana@example.com", Pref = "Vegetarian" },
                    new { Name = "Ion Ionescu", Email = "ion@example.com", Pref = "Balanced" },
                    new { Name = "Maria Radu", Email = "maria@example.com", Pref = "Vegan" }
                };
                
                foreach (var user in users)
                {
                    var cmd = new System.Data.SQLite.SQLiteCommand(@"
                        SELECT COUNT(*) FROM Users WHERE Email = @Email", connection);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    
                    if (count == 0)
                    {
                        cmd = new System.Data.SQLite.SQLiteCommand(@"
                            INSERT INTO Users (Name, Email, DietaryPreference, TargetCalories, NotificationsEnabled, CreatedAt)
                            VALUES (@Name, @Email, @Pref, 2000, 1, @CreatedAt)", connection);
                        
                        cmd.Parameters.AddWithValue("@Name", user.Name);
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        cmd.Parameters.AddWithValue("@Pref", user.Pref);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        static List<MealPlan> GetAllMealPlans()
        {
            var plans = new List<MealPlan>();
            using (var connection = new System.Data.SQLite.SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                var cmd = new System.Data.SQLite.SQLiteCommand("SELECT * FROM MealPlans LIMIT 5", connection);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        plans.Add(new MealPlan
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            UserId = Convert.ToInt32(reader["UserId"])
                        });
                    }
                }
            }
            return plans;
        }

        static (int TotalItems, Dictionary<string, decimal> AverageCaloriesByCategory) CalculateComplexStatistics()
        {
            var result = (0, new Dictionary<string, decimal>());
            
            using (var connection = new System.Data.SQLite.SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                
                // Total items
                var cmd = new System.Data.SQLite.SQLiteCommand("SELECT COUNT(*) FROM FoodItems", connection);
                result.TotalItems = Convert.ToInt32(cmd.ExecuteScalar());
                
                // Media calorii per categorie (operatiune mai complexa)
                cmd = new System.Data.SQLite.SQLiteCommand(@"
                    SELECT Category, AVG(Calories) as AvgCalories 
                    FROM FoodItems 
                    GROUP BY Category 
                    ORDER BY AvgCalories DESC", connection);
                
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.AverageCaloriesByCategory[reader["Category"].ToString()] = 
                            Convert.ToDecimal(reader["AvgCalories"]);
                    }
                }
            }
            
            // Simulare procesare suplimentara
            System.Threading.Thread.Sleep(500);
            
            return result;
        }

        #endregion
    }
}
