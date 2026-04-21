using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using MealPlanApp.Models;

namespace MealPlanApp.Services
{
    /// <summary>
    /// Cerinta 6: Setarea de cron pentru trimiterea de emailuri cu materiale promotionale
    /// Cerinta 9: Integrare de serviciu asincron specific temei
    /// </summary>
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderPassword;

        public EmailService(string smtpServer = "smtp.example.com", int smtpPort = 587, 
                           string senderEmail = "noreply@mealplan.com", string senderPassword = "")
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _senderEmail = senderEmail;
            _senderPassword = senderPassword;
        }

        /// <summary>
        /// Trimitere email promotional catre toti utilizatorii (pentru cron zilnic/saptamanal)
        /// </summary>
        public void SendPromotionalEmails()
        {
            Console.WriteLine("\n=== TRIMITERE EMAILURI PROMOTIONALE ===");
            Console.WriteLine($"Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            
            var users = GetAllUsers();
            var promotionalMeals = GetPromotionalMeals();
            
            int sentCount = 0;
            int failedCount = 0;
            
            foreach (var user in users)
            {
                try
                {
                    // Simulare trimitere email (in productie s-ar folosi SmtpClient sau serviciu extern)
                    string subject = "Oferte Speciale - Planul Tau Alimentar";
                    string body = GeneratePromotionalEmailBody(user, promotionalMeals);
                    
                    // In productie:
                    // using (var client = new SmtpClient(_smtpServer, _smtpPort))
                    // {
                    //     client.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
                    //     client.EnableSsl = true;
                    //     var mailMessage = new MailMessage(_senderEmail, user.Email, subject, body);
                    //     client.Send(mailMessage);
                    // }
                    
                    // Simulare
                    Console.WriteLine($"  [EMAIL] Catre: {user.Email} - Subiect: {subject}");
                    
                    // Log email trimis
                    LogEmailSent(user.Id, subject, true);
                    
                    sentCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  [ERROR] Email catre {user.Email}: {ex.Message}");
                    LogEmailSent(user.Id, "Error", false);
                    failedCount++;
                }
            }
            
            Console.WriteLine($"\nTotal: {sentCount} trimise, {failedCount} esuate");
            Console.WriteLine("=== TRIMITERE EMAILURI COMPLETATA ===\n");
        }

        /// <summary>
        /// Serviciu asincron pentru trimiterea de notificari
        /// Cerinta 9: Integrare serviciu asincron
        /// </summary>
        public async System.Threading.Tasks.Task SendNotificationAsync(User user, string subject, string message)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                Console.WriteLine($"  [ASYNC EMAIL] Catre: {user.Email}");
                Console.WriteLine($"                Subiect: {subject}");
                
                // Simulare procesare asincrona
                System.Threading.Thread.Sleep(100);
                
                LogEmailSent(user.Id, subject, true);
            });
        }

        /// <summary>
        /// Procesare in background pentru coada de emailuri
        /// </summary>
        public async System.Threading.Tasks.Task ProcessEmailQueueAsync()
        {
            Console.WriteLine("\n=== PROCESARE COADA EMAILURI (Background Service) ===");
            
            var pendingEmails = GetPendingEmailsFromQueue();
            
            var tasks = pendingEmails.Select(async email =>
            {
                await SendNotificationAsync(email.User, email.Subject, email.Body);
            });
            
            await System.Threading.Tasks.Task.WhenAll(tasks);
            
            Console.WriteLine("=== PROCESARE COADA COMPLETATA ===\n");
        }

        private List<User> GetAllUsers()
        {
            var users = new List<User>();
            
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Users";
                
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            DietaryPreference = reader["DietaryPreference"]?.ToString()
                        });
                    }
                }
            }
            
            return users;
        }

        private List<FoodItem> GetPromotionalMeals()
        {
            var meals = new List<FoodItem>();
            
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"
                    SELECT * FROM FoodItems 
                    WHERE Calories < 300 
                    ORDER BY RANDOM() 
                    LIMIT 5";
                
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        meals.Add(new FoodItem
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Category = reader["Category"].ToString(),
                            Calories = Convert.ToDecimal(reader["Calories"])
                        });
                    }
                }
            }
            
            return meals;
        }

        private string GeneratePromotionalEmailBody(User user, List<FoodItem> meals)
        {
            string body = $@"
Buna {user.Name},

Iti prezentam ofertele noastre speciale pentru aceasta saptamana!

Alimente recomandate pentru tine:
";
            foreach (var meal in meals)
            {
                body += $"- {meal.Name} ({meal.Calories} kcal)\n";
            }

            body += @"
Profită de reducerile noastre și creează-ți un plan alimentar personalizat!

Cu drag,
Echipa MealPlanApp
";
            return body;
        }

        private void LogEmailSent(int userId, string subject, bool success)
        {
            // Log simplu in consola - in productie s-ar salva in baza de date
            Console.WriteLine($"  [LOG] Email {(success ? "trimis" : "esuat")}: {subject} pentru User {userId}");
        }

        private List<(User User, string Subject, string Body)> GetPendingEmailsFromQueue()
        {
            // Simulare coada de emailuri
            var queue = new List<(User, string, string)>();
            var users = GetAllUsers().Take(3).ToList();
            
            foreach (var user in users)
            {
                queue.Add((user, "Notificare Plan Alimentar", $"Buna {user.Name}, ai un plan nou!"));
            }
            
            return queue;
        }

        /// <summary>
        /// Configurare CRON pentru trimiterea automata de emailuri
        /// Pentru Linux:
        /// 0 9 * * 1 /usr/bin/dotnet /path/to/MealPlanApp.dll --send-promotional-emails
        /// 
        /// Pentru Windows Task Scheduler:
        /// Actiune: dotnet.exe
        /// Argumente: C:\path\to\MealPlanApp.dll --send-promotional-emails
        /// Trigger: Saptamanal, Luni, ora 09:00
        /// </summary>
        public static void DisplayCronConfiguration()
        {
            Console.WriteLine("\n=== CONFIGURARE CRON PENTRU EMAILURI ===\n");
            
            Console.WriteLine("Pentru LINUX (crontab):");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("# Trimitere emailuri promotionale in fiecare Luni la 09:00");
            Console.WriteLine("0 9 * * 1 /usr/bin/dotnet /opt/mealplan/MealPlanApp.dll --action=email");
            Console.WriteLine("");
            Console.WriteLine("# Actualizare zilnica de date la 02:00");
            Console.WriteLine("0 2 * * * /usr/bin/dotnet /opt/mealplan/MealPlanApp.dll --action=daily-update");
            Console.WriteLine("-------------------------------------------\n");
            
            Console.WriteLine("Pentru WINDOWS (Task Scheduler):");
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("1. Deschideti 'Task Scheduler'");
            Console.WriteLine("2. Creati o sarcina de baza");
            Console.WriteLine("3. Configuratii:");
            Console.WriteLine("   - Nume: MealPlan - Emailuri Promotionale");
            Console.WriteLine("   - Trigger: Saptamanal, Luni, 09:00");
            Console.WriteLine("   - Actiune: Start a program");
            Console.WriteLine("   - Program: dotnet.exe");
            Console.WriteLine("   - Argumente: C:\\MealPlanApp\\MealPlanApp.dll --action=email");
            Console.WriteLine("-------------------------------------------\n");
        }
    }
}
