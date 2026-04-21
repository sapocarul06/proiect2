using System;
using System.Collections.Generic;
using MealPlanApp.Models;
using MealPlanApp.Data;

namespace MealPlanApp.Services
{
    /// <summary>
    /// Cerința 2: Script de import pentru actualizare zilnică - rulează în fiecare zi (cron)
    /// </summary>
    public class DailyUpdateService
    {
        private readonly List<string> _providers = new List<string> { "ProviderA", "ProviderB", "ProviderC" };

        public void RunDailyUpdate()
        {
            Console.WriteLine("=== Actualizare Zilnică a Datelor ===");
            Console.WriteLine($"Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            
            int addedCount = 0;
            int updatedCount = 0;
            
            foreach (var provider in _providers)
            {
                Console.WriteLine($"\nVerificare actualizări de la {provider}...");
                var foods = GetUpdatedFoodsFromProvider(provider);
                
                foreach (var food in foods)
                {
                    // Verificăm dacă există deja pe baza hash-ului imaginii
                    if (!string.IsNullOrEmpty(food.ImageHash) && DatabaseHelper.FoodExistsByHash(food.ImageHash))
                    {
                        // Actualizăm datele existente
                        var existingFood = FindFoodByHash(food.ImageHash);
                        if (existingFood != null)
                        {
                            existingFood.Name = food.Name;
                            existingFood.Calories = food.Calories;
                            existingFood.UpdatedAt = DateTime.Now;
                            DatabaseHelper.UpdateFood(existingFood);
                            updatedCount++;
                            Console.WriteLine($"  - Actualizat: {food.Name}");
                        }
                    }
                    else
                    {
                        // Adăugăm aliment nou
                        DatabaseHelper.InsertFood(food);
                        addedCount++;
                        Console.WriteLine($"  - Adăugat: {food.Name}");
                    }
                }
            }
            
            Console.WriteLine($"\nActualizare completată: {addedCount} adăugate, {updatedCount} actualizate.");
        }

        private Food FindFoodByHash(string imageHash)
        {
            // Implementare simplificată - în producție ar fi o metodă dedicated în DatabaseHelper
            var allFoods = DatabaseHelper.GetAllFoods();
            return allFoods.Find(f => f.ImageHash == imageHash);
        }

        private List<Food> GetUpdatedFoodsFromProvider(string provider)
        {
            // Simulare date actualizate zilnic
            var foods = new List<Food>();
            var now = DateTime.Now;
            var random = new Random();
            
            // Generăm hash unic pentru imagine
            string GenerateHash() => $"hash_{Guid.NewGuid().ToString().Substring(0, 8)}";
            
            if (provider == "ProviderA")
            {
                foods.Add(new Food 
                { 
                    Name = "Salată Caesar cu Pui", 
                    Calories = 380 + random.Next(-10, 10), 
                    Protein = 28, 
                    Carbs = 15, 
                    Fat = 19, 
                    Provider = provider, 
                    ImageHash = GenerateHash(),
                    CreatedAt = now, 
                    UpdatedAt = now 
                });
            }
            else if (provider == "ProviderB")
            {
                foods.Add(new Food 
                { 
                    Name = "Pește la Grătar", 
                    Calories = 320, 
                    Protein = 40, 
                    Carbs = 5, 
                    Fat = 12, 
                    Provider = provider, 
                    ImageHash = GenerateHash(),
                    CreatedAt = now, 
                    UpdatedAt = now 
                });
            }
            
            return foods;
        }

        /// <summary>
        /// Configurare cron pentru Linux (se adaugă în crontab -e)
        /// Exemplu: 0 2 * * * /usr/bin/mono /path/to/MealPlanApp.exe --daily-update
        /// </summary>
        public static string GetCronConfiguration()
        {
            return @"
# Configurare cron pentru actualizare zilnică la ora 02:00
# Deschideți terminalul și rulați: crontab -e
# Adăugați linia de mai jos:

0 2 * * * /usr/bin/mono /opt/mealplan/MealPlanApp.exe daily-update >> /var/log/mealplan-daily.log 2>&1

# Pentru Windows Task Scheduler, creați un task care rulează:
# Program: C:\Program Files\MealPlanApp\MealPlanApp.exe
# Argumente: daily-update
# Trigger: Daily at 2:00 AM
";
        }
    }
}
