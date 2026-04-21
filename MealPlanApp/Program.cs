using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealPlanApp.Models;
using MealPlanApp.Data;
using MealPlanApp.Services;

namespace MealPlanApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║     APLICAȚIE GESTIONARE PLAN ALIMENTAR                        ║");
            Console.WriteLine("║     .NET Framework 4.7.2                                       ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            // Inițializare bază de date
            DatabaseHelper.Initialize();
            Console.WriteLine("✓ Baza de date inițializată cu succes.\n");

            bool exit = false;
            while (!exit)
            {
                DisplayMenu();
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RunInitialImport();
                        break;
                    case "2":
                        RunDailyUpdate();
                        break;
                    case "3":
                        DetectDuplicates();
                        break;
                    case "4":
                        DisplayImageStorageStudy();
                        break;
                    case "5":
                        ShowRecommendedFoods();
                        break;
                    case "6":
                        DisplayEmailCronConfig();
                        break;
                    case "7":
                        RunParallelNotifications().Wait();
                        break;
                    case "8":
                        DisplayHostingStudy();
                        break;
                    case "9":
                        RunAsyncProcessing().Wait();
                        break;
                    case "10":
                        GenerateReports();
                        break;
                    case "11":
                        RunPerformanceStats();
                        break;
                    case "0":
                        exit = true;
                        Console.WriteLine("La revedere!");
                        break;
                    default:
                        Console.WriteLine("Opțiune invalidă. Încercați din nou.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nApăsați ENTER pentru a continua...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        static void DisplayMenu()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      MENIU PRINCIPAL                           ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  1. Import inițial de la provideri (Cerința 1)                 ║");
            Console.WriteLine("║  2. Actualizare zilnică date (Cerința 2 + Cron)                ║");
            Console.WriteLine("║  3. Detectare duplicate pe baza imaginilor (Cerința 3)         ║");
            Console.WriteLine("║  4. Studiu stocare imagini (Cerința 4)                         ║");
            Console.WriteLine("║  5. Listare sugestii personalizate (Cerința 5)                 ║");
            Console.WriteLine("║  6. Configurare cron emailuri promo (Cerința 6)                ║");
            Console.WriteLine("║  7. Notificări paralele utilizatori (Cerința 7)                ║");
            Console.WriteLine("║  8. Studiu găzduire aplicație (Cerința 8)                      ║");
            Console.WriteLine("║  9. Procesare asincronă (Cerința 9)                            ║");
            Console.WriteLine("║ 10. Generare rapoarte PDF/Excel (Cerința 10)                   ║");
            Console.WriteLine("║ 11. Statistici performanță (Cerința 11)                        ║");
            Console.WriteLine("║                                                                ║");
            Console.WriteLine("║  0. Ieșire                                                     ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.Write("\nSelectați o opțiune: ");
        }

        static void RunInitialImport()
        {
            var service = new InitialImportService();
            service.RunInitialImport();
        }

        static void RunDailyUpdate()
        {
            var service = new DailyUpdateService();
            service.RunDailyUpdate();
        }

        static void DetectDuplicates()
        {
            var detector = new ImageDuplicateDetector();
            detector.FindDuplicates();
        }

        static void DisplayImageStorageStudy()
        {
            ImageDuplicateDetector.DisplayImageStorageStudy();
        }

        static void ShowRecommendedFoods()
        {
            // Creăm un utilizator demo dacă nu există
            var user = new User { Name = "Utilizator Demo", Email = "demo@example.com", CreatedAt = DateTime.Now };
            int userId = DatabaseHelper.InsertUser(user);
            
            var preference = new UserPreference
            {
                UserId = userId,
                DietaryType = "balanced",
                MinCalories = 200,
                MaxCalories = 500,
                MinProtein = 15,
                MaxCarbs = 60,
                MaxFat = 25,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            DatabaseHelper.InsertUserPreference(preference);
            
            Console.WriteLine($"Utilizator creat cu ID: {userId}\n");
            
            var notificationService = new NotificationService();
            notificationService.GetRecommendedFoods(userId);
        }

        static void DisplayEmailCronConfig()
        {
            Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════════════════════╗
║     CERINȚA 6: Cron pentru trimiterea de emailuri cu materiale promiționale  ║
╠══════════════════════════════════════════════════════════════════════════════╣
║                                                                              ║
║  CONFIGURARE LINUX CRONTAB:                                                  ║
║  ────────────────────────────                                                ║
║  Deschideți terminalul și rulați: crontab -e                                 ║
║  Adăugați una dintre liniile de mai jos:                                     ║
║                                                                              ║
║  # Zilnic la ora 09:00                                                       ║
║  0 9 * * * /usr/bin/mono /opt/mealplan/MealPlanApp.exe send-promo >> /var/log/mealplan-promo.log 2>&1
║                                                                              ║
║  # Luni dimineața la 08:00                                                   ║
║  0 8 * * 1 /usr/bin/mono /opt/mealplan/MealPlanApp.exe send-promo >> /var/log/mealplan-promo.log 2>&1
║                                                                              ║
║  # În fiecare zi la prânz (12:00) și seara (18:00)                           ║
║  0 12,18 * * * /usr/bin/mono /opt/mealplan/MealPlanApp.exe send-promo >> /var/log/mealplan-promo.log 2>&1
║                                                                              ║
║  CONFIGURARE WINDOWS TASK SCHEDULER:                                         ║
║  ───────────────────────────────                                             ║
║  1. Deschideți Task Scheduler                                                ║
║  2. Click dreapta → Create Basic Task                                        ║
║  3. Nume: ""MealPlan Promo Emails""                                          ║
║  4. Trigger: Daily / Weekly                                                  ║
║  5. Action: Start a program                                                  ║
║  6. Program: C:\Program Files\MealPlanApp\MealPlanApp.exe                    ║
║  7. Arguments: send-promo                                                    ║
║  8. Finish                                                                   ║
║                                                                              ║
║  COD PENTRU TRIMITEREA EMAILURILOR (de implementat):                         ║
║  ─────────────────────────────────────────                                   ║
║  - Folosiți System.Net.Mail.SmtpClient                                       ║
║  - Sau un serviciu terț: SendGrid, Mailgun, Azure Communication Services     ║
║  - Template-uri HTML pentru emailuri atractive                               ║
║                                                                              ║
╚══════════════════════════════════════════════════════════════════════════════╝
");
        }

        static async Task RunParallelNotifications()
        {
            // Simulăm alimente noi adăugate
            var newFoods = new List<Food>
            {
                new Food { Name = "Salată Proteinată", Calories = 350, Protein = 30, Carbs = 20, Fat = 12 },
                new Food { Name = "Smoothie Energizant", Calories = 250, Protein = 18, Carbs = 35, Fat = 5 },
                new Food { Name = "Bowl Mediteranean", Calories = 420, Protein = 22, Carbs = 45, Fat = 18 },
                new Food { Name = "Supă Detox", Calories = 180, Protein = 12, Carbs = 25, Fat = 4 }
            };

            var service = new NotificationService();
            await service.CheckAndNotifyUsersAsync(newFoods);
        }

        static void DisplayHostingStudy()
        {
            AsyncProcessingService.DisplayHostingStudy();
        }

        static async Task RunAsyncProcessing()
        {
            var foods = DatabaseHelper.GetAllFoods();
            if (foods.Count == 0)
            {
                Console.WriteLine("Nu există alimente în baza de date. Rulați mai întâi importul inițial.");
                return;
            }

            var service = new AsyncProcessingService();
            await service.ProcessNutritionalAnalysisAsync(foods);
        }

        static void GenerateReports()
        {
            var reportService = new ReportService();
            
            Console.WriteLine("\nSe generează rapoartele...\n");
            
            var excelPath = reportService.GenerateExcelReport();
            Console.WriteLine($"\n📄 Raport Excel (CSV): {excelPath}");
            
            var pdfPath = reportService.GeneratePdfReport();
            Console.WriteLine($"\n📄 Raport PDF (text): {pdfPath}");
            
            Console.WriteLine("\n✓ Toate rapoartele au fost generate în folderul 'Reports'");
        }

        static void RunPerformanceStats()
        {
            var foods = DatabaseHelper.GetAllFoods();
            if (foods.Count == 0)
            {
                Console.WriteLine("Nu există alimente în baza de date. Rulați mai întâi importul inițial.");
                return;
            }

            var service = new AsyncProcessingService();
            service.RunPerformanceStatistics();
        }
    }
}
