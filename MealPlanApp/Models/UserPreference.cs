using System;

namespace MealPlanApp.Models
{
    public class UserPreference
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string DietaryType { get; set; } // e.g., "vegetarian", "vegan", "keto"
        public decimal MinCalories { get; set; }
        public decimal MaxCalories { get; set; }
        public decimal MinProtein { get; set; }
        public decimal MaxCarbs { get; set; }
        public decimal MaxFat { get; set; }
        public string ExcludedIngredients { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
