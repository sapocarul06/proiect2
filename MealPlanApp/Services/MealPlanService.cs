using System;
using System.Collections.Generic;
using System.Linq;
using MealPlanApp.Models;
using MealPlanApp.Data;

namespace MealPlanApp.Services
{
    public class MealPlanService
    {
        /// <summary>
        /// Calculează planul alimentar personalizat bazat pe datele utilizatorului
        /// </summary>
        public PersonalizedPlanResult CalculatePersonalizedPlan(int heightCm, double weightKg, int ageYears, string gender, string activityLevel)
        {
            // 1. Calcul Metabolism Bazal (BMR) - Formula Mifflin-St Jeor
            double bmr;
            if (gender.ToUpper() == "M")
            {
                bmr = 10 * weightKg + 6.25 * heightCm - 5 * ageYears + 5;
            }
            else
            {
                bmr = 10 * weightKg + 6.25 * heightCm - 5 * ageYears - 161;
            }

            // 2. Calcul Calorii Întreținere (TDEE) în funcție de activitate
            double activityMultiplier = activityLevel.ToLower() switch
            {
                "sedentar" => 1.2,
                "moderat" => 1.55,
                "intens" => 1.725,
                _ => 1.2
            };

            double tdee = bmr * activityMultiplier;

            // 3. Calcul deficit caloric (-500 kcal pentru pierdere în greutate)
            double targetCalories = tdee - 500;
            if (targetCalories < 1200) targetCalories = 1200; // Minim sigur

            // 4. Distribuție macronutrienți (40% carbohidrați, 30% proteine, 30% grăsimi)
            double proteinPercentage = 0.30;
            double carbsPercentage = 0.40;
            double fatPercentage = 0.30;

            double proteinCalories = targetCalories * proteinPercentage;
            double carbsCalories = targetCalories * carbsPercentage;
            double fatCalories = targetCalories * fatPercentage;

            // 1g proteine = 4 kcal, 1g carbohidrați = 4 kcal, 1g grăsimi = 9 kcal
            int proteinGrams = (int)(proteinCalories / 4);
            int carbsGrams = (int)(carbsCalories / 4);
            int fatGrams = (int)(fatCalories / 9);

            // 5. Generare sugestii de mese din baza de date
            List<MealSuggestion> mealPlan = GenerateMealSuggestions(targetCalories, proteinGrams, carbsGrams, fatGrams);

            return new PersonalizedPlanResult
            {
                BMR = (int)bmr,
                TDEE = (int)tdee,
                TargetCalories = (int)targetCalories,
                ProteinGrams = proteinGrams,
                CarbsGrams = carbsGrams,
                FatGrams = fatGrams,
                ProteinCalories = (int)proteinCalories,
                CarbsCalories = (int)carbsCalories,
                FatCalories = (int)fatCalories,
                ProteinPercentage = (int)(proteinPercentage * 100),
                CarbsPercentage = (int)(carbsPercentage * 100),
                FatPercentage = (int)(fatPercentage * 100),
                MealPlan = mealPlan
            };
        }

        /// <summary>
        /// Generează sugestii de mesepentru o zi
        /// </summary>
        private List<MealSuggestion> GenerateMealSuggestions(double targetCalories, int proteinTarget, int carbsTarget, int fatTarget)
        {
            var meals = new List<MealSuggestion>();
            
            // Distribuție pe mese: Mic dejun 25%, Prânz 35%, Cină 30%, Gustări 10%
            var mealTypes = new Dictionary<string, double>
            {
                { "Mic dejun", 0.25 },
                { "Prânz", 0.35 },
                { "Cină", 0.30 },
                { "Gustare", 0.10 }
            };

            // Obținem alimente din baza de date
            var allFoods = DatabaseHelper.GetAllFoods();
            var random = new Random();

            foreach (var mealType in mealTypes)
            {
                double mealCalories = targetCalories * mealType.Value;
                
                // Selectăm alimente aleatorii care se potrivesc cu calorii
                var suitableFoods = allFoods
                    .Where(f => Math.Abs(f.Calories - mealCalories) < 150)
                    .ToList();

                if (!suitableFoods.Any())
                {
                    // Dacă nu găsim alimente potrivite, luăm cele mai apropiate
                    suitableFoods = allFoods
                        .OrderBy(f => Math.Abs(f.Calories - mealCalories))
                        .Take(2)
                        .ToList();
                }

                foreach (var food in suitableFoods.Take(1))
                {
                    meals.Add(new MealSuggestion
                    {
                        MealType = mealType.Key,
                        FoodName = food.Name,
                        Calories = (int)food.Calories,
                        Protein = (int)food.Protein,
                        Carbs = (int)food.Carbs,
                        Fat = (int)food.Fat
                    });
                }
            }

            // Dacă nu avem suficiente mese, adăugăm unele default
            if (meals.Count < 4)
            {
                var defaultMeals = new List<MealSuggestion>
                {
                    new MealSuggestion { MealType = "Mic dejun", FoodName = "Omletă cu legume", Calories = 350, Protein = 20, Carbs = 15, Fat = 22 },
                    new MealSuggestion { MealType = "Prânz", FoodName = "Pui la grătar cu orez", Calories = 550, Protein = 45, Carbs = 60, Fat = 12 },
                    new MealSuggestion { MealType = "Cină", FoodName = "Pește cu salată", Calories = 400, Protein = 35, Carbs = 20, Fat = 18 },
                    new MealSuggestion { MealType = "Gustare", FoodName = "Iaurt grecesc cu nuci", Calories = 200, Protein = 15, Carbs = 12, Fat = 10 }
                };

                while (meals.Count < 4)
                {
                    var missingType = mealTypes.Keys.ElementAt(meals.Count);
                    var defaultMeal = defaultMeals.FirstOrDefault(m => m.MealType == missingType);
                    if (defaultMeal != null)
                        meals.Add(defaultMeal);
                }
            }

            return meals;
        }

        /// <summary>
        /// Metodă veche pentru compatibilitate
        /// </summary>
        public PersonalizedSuggestions GetPersonalizedSuggestions(int heightCm, double weightKg, int ageYears, string gender, string activityLevel)
        {
            var result = CalculatePersonalizedPlan(heightCm, weightKg, ageYears, gender, activityLevel);
            
            return new PersonalizedSuggestions
            {
                TargetCalories = result.TargetCalories,
                ProteinGrams = result.ProteinGrams,
                CarbsGrams = result.CarbsGrams,
                FatGrams = result.FatGrams,
                MealSuggestions = result.MealPlan.Select(m => new Food 
                { 
                    Name = m.FoodName, 
                    Calories = m.Calories,
                    Protein = m.Protein,
                    Carbs = m.Carbs,
                    Fat = m.Fat
                }).ToList()
            };
        }
    }

    /// <summary>
    /// Rezultatul calculului planului alimentar
    /// </summary>
    public class PersonalizedPlanResult
    {
        public int BMR { get; set; }
        public int TDEE { get; set; }
        public int TargetCalories { get; set; }
        public int ProteinGrams { get; set; }
        public int CarbsGrams { get; set; }
        public int FatGrams { get; set; }
        public int ProteinCalories { get; set; }
        public int CarbsCalories { get; set; }
        public int FatCalories { get; set; }
        public int ProteinPercentage { get; set; }
        public int CarbsPercentage { get; set; }
        public int FatPercentage { get; set; }
        public List<MealSuggestion> MealPlan { get; set; } = new List<MealSuggestion>();
    }

    /// <summary>
    /// Sugestie de masă individuală
    /// </summary>
    public class MealSuggestion
    {
        public string MealType { get; set; }
        public string FoodName { get; set; }
        public int Calories { get; set; }
        public int Protein { get; set; }
        public int Carbs { get; set; }
        public int Fat { get; set; }
    }

    /// <summary>
    /// Clasă veche pentru compatibilitate
    /// </summary>
    public class PersonalizedSuggestions
    {
        public int TargetCalories { get; set; }
        public int ProteinGrams { get; set; }
        public int CarbsGrams { get; set; }
        public int FatGrams { get; set; }
        public List<Food> MealSuggestions { get; set; } = new List<Food>();
    }
}
