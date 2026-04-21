using System;

namespace MealPlanApp.Models
{
    public class FoodItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Calories { get; set; }
        public decimal Protein { get; set; }
        public decimal Carbs { get; set; }
        public decimal Fat { get; set; }
        public string ImageUrl { get; set; }
        public string ImageHash { get; set; } // Pentru identificarea duplicatelor
        public int ProviderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class MealPlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DietaryPreference { get; set; } // Vegan, Vegetarian, Keto, etc.
        public int TargetCalories { get; set; }
        public bool IsActive { get; set; }
    }

    public class MealPlanItem
    {
        public int Id { get; set; }
        public int MealPlanId { get; set; }
        public int FoodItemId { get; set; }
        public string MealType { get; set; } // Breakfast, Lunch, Dinner, Snack
        public int DayNumber { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string DietaryPreference { get; set; }
        public int TargetCalories { get; set; }
        public bool NotificationsEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserPreference
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PreferenceKey { get; set; }
        public string PreferenceValue { get; set; }
    }

    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Provider
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ApiUrl { get; set; }
        public string ApiKey { get; set; }
        public bool IsActive { get; set; }
    }

    public class ImportLog
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public DateTime ImportDate { get; set; }
        public int RecordsImported { get; set; }
        public int RecordsUpdated { get; set; }
        public int Errors { get; set; }
        public string Status { get; set; }
    }
}
