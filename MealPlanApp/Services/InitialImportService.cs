using System;
using System.Collections.Generic;
using MealPlanApp.Models;
using MealPlanApp.Data;

namespace MealPlanApp.Services
{
    /// <summary>
    /// Cerința 1: Script pentru importul datelor de la mai mulți provideri - rulează o singură dată
    /// </summary>
    public class InitialImportService
    {
        private readonly List<string> _providers = new List<string> { "ProviderA", "ProviderB", "ProviderC" };

        public void RunInitialImport()
        {
            Console.WriteLine("=== Import Inițial de la Mai Mulți Provideri ===");
            
            foreach (var provider in _providers)
            {
                Console.WriteLine($"Import date de la {provider}...");
                var foods = GetFoodsFromProvider(provider);
                
                foreach (var food in foods)
                {
                    DatabaseHelper.InsertFood(food);
                    Console.WriteLine($"  - Adăugat: {food.Name} ({food.Calories} kcal)");
                }
            }
            
            Console.WriteLine($"Import completat. Total provideri: {_providers.Count}");
        }

        private List<Food> GetFoodsFromProvider(string provider)
        {
            // Simulare date de la provideri diferiți
            var foods = new List<Food>();
            var now = DateTime.Now;
            
            if (provider == "ProviderA")
            {
                foods.Add(new Food { Name = "Salată Caesar", Calories = 350, Protein = 25, Carbs = 15, Fat = 18, Provider = provider, CreatedAt = now, UpdatedAt = now });
                foods.Add(new Food { Name = "Paste Carbonara", Calories = 650, Protein = 30, Carbs = 70, Fat = 25, Provider = provider, CreatedAt = now, UpdatedAt = now });
            }
            else if (provider == "ProviderB")
            {
                foods.Add(new Food { Name = "Grătar Mixt", Calories = 550, Protein = 45, Carbs = 10, Fat = 30, Provider = provider, CreatedAt = now, UpdatedAt = now });
                foods.Add(new Food { Name = "Supă de Legume", Calories = 180, Protein = 8, Carbs = 25, Fat = 5, Provider = provider, CreatedAt = now, UpdatedAt = now });
            }
            else if (provider == "ProviderC")
            {
                foods.Add(new Food { Name = "Smoothie Verde", Calories = 220, Protein = 12, Carbs = 35, Fat = 6, Provider = provider, CreatedAt = now, UpdatedAt = now });
                foods.Add(new Food { Name = "Bowl Quinoa", Calories = 420, Protein = 18, Carbs = 55, Fat = 14, Provider = provider, CreatedAt = now, UpdatedAt = now });
            }
            
            return foods;
        }
    }
}
