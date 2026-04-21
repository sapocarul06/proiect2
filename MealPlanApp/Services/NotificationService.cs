using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MealPlanApp.Models;
using MealPlanApp.Data;

namespace MealPlanApp.Services
{
    /// <summary>
    /// Cerința 5: Aplicație pentru listarea datelor în funcție de specificitatea aplicației
    /// Cerința 7: Notificare apariție anunț care corespunde cerințelor utilizatorului - fire de execuție paralele
    /// </summary>
    public class NotificationService
    {
        /// <summary>
        /// Listează alimentele recomandate în funcție de preferințele utilizatorului
        /// </summary>
        public List<Food> GetRecommendedFoods(int userId)
        {
            Console.WriteLine("=== Sugestii Plan Alimentar Personalizat ===");
            
            var preference = DatabaseHelper.GetUserPreference(userId);
            if (preference == null)
            {
                Console.WriteLine("Nu există preferințe configurate pentru acest utilizator.");
                return new List<Food>();
            }
            
            Console.WriteLine($"Preferințe utilizator {userId}:");
            Console.WriteLine($"  - Tip dietă: {preference.DietaryType}");
            Console.WriteLine($"  - Calorii: {preference.MinCalories} - {preference.MaxCalories} kcal");
            Console.WriteLine($"  - Proteine min: {preference.MinProtein}g");
            
            var allFoods = DatabaseHelper.GetAllFoods();
            var recommended = allFoods.Where(f => 
                f.Calories >= preference.MinCalories && 
                f.Calories <= preference.MaxCalories &&
                f.Protein >= preference.MinProtein
            ).ToList();
            
            Console.WriteLine($"\nAlimente recomandate: {recommended.Count}");
            foreach (var food in recommended.Take(10))
            {
                Console.WriteLine($"  ✓ {food.Name} - {food.Calories} kcal, {food.Protein}g proteine");
            }
            
            return recommended;
        }

        /// <summary>
        /// Cerința 7: Verifică și notifică utilizatorii când apar alimente noi care corespund preferințelor lor
        /// Folosește fire de execuție paralele (async/await + Task.WhenAll)
        /// </summary>
        public async Task CheckAndNotifyUsersAsync(List<Food> newFoods)
        {
            Console.WriteLine("\n=== Verificare Notificări Paralele ===");
            Console.WriteLine($"Se procesează {newFoods.Count} alimente noi...");
            
            // Obținem toți utilizatorii cu preferințe
            var users = GetAllUsersWithPreferences();
            
            var notificationTasks = new List<Task>();
            
            // Creăm task-uri paralele pentru fiecare utilizator
            foreach (var user in users)
            {
                notificationTasks.Add(Task.Run(async () =>
                {
                    await ProcessUserNotificationsAsync(user.Key, user.Value, newFoods);
                }));
            }
            
            // Așteptăm completarea tuturor task-urilor în paralel
            await Task.WhenAll(notificationTasks);
            
            Console.WriteLine("\n✓ Toate notificările au fost procesate!");
        }

        private async Task ProcessUserNotificationsAsync(int userId, UserPreference preference, List<Food> newFoods)
        {
            // Simulăm o operație I/O (ex: verificare DB, trimitere email)
            await Task.Delay(100); // Simulare latență
            
            var matchingFoods = newFoods.Where(f =>
                f.Calories >= preference.MinCalories &&
                f.Calories <= preference.MaxCalories &&
                f.Protein >= preference.MinProtein
            ).ToList();
            
            if (matchingFoods.Any())
            {
                Console.WriteLine($"  🔔 Utilizator {userId}: {matchingFoods.Count} alimente noi compatibile!");
                foreach (var food in matchingFoods)
                {
                    Console.WriteLine($"      → {food.Name} ({food.Calories} kcal)");
                }
                
                // Aici s-ar trimite email/notification reală
                // await SendEmailNotificationAsync(userId, matchingFoods);
            }
        }

        private Dictionary<int, UserPreference> GetAllUsersWithPreferences()
        {
            // Simulare: în producție s-ar citi din DB toți utilizatorii
            return new Dictionary<int, UserPreference>
            {
                { 1, new UserPreference { 
                    UserId = 1, 
                    DietaryType = "balanced",
                    MinCalories = 200, 
                    MaxCalories = 500, 
                    MinProtein = 15 
                }},
                { 2, new UserPreference { 
                    UserId = 2, 
                    DietaryType = "high-protein",
                    MinCalories = 300, 
                    MaxCalories = 600, 
                    MinProtein = 25 
                }},
                { 3, new UserPreference { 
                    UserId = 3, 
                    DietaryType = "low-calorie",
                    MinCalories = 100, 
                    MaxCalories = 300, 
                    MinProtein = 10 
                }}
            };
        }

        /// <summary>
        /// Trimite notificări către un singur utilizator (metodă asincronă)
        /// </summary>
        private async Task SendEmailNotificationAsync(int userId, List<Food> foods)
        {
            await Task.Run(() =>
            {
                // Simulare trimitere email
                Console.WriteLine($"    [EMAIL] Se trimite către utilizatorul {userId} cu {foods.Count} sugestii");
            });
        }
    }
}
