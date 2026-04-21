using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MealPlanApp.Models;

namespace MealPlanApp.Services
{
    public class MealPlanService
    {
        /// <summary>
        /// Cerinta 5: Aplicatie pentru listarea datelor in functie de specificitatea aplicatiei
        /// </summary>
        public List<FoodItem> GetSuggestionsForUser(User user)
        {
            Console.WriteLine($"\n=== SUGESTII PENTRU UTILIZATORUL: {user.Name} ===");
            Console.WriteLine($"Preferinta: {user.DietaryPreference}");
            Console.WriteLine($"Calorii tintite: {user.TargetCalories} kcal/zi\n");
            
            var suggestions = new List<FoodItem>();
            
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                
                // Selectare alimente compatibile cu preferintele utilizatorului
                string query = @"
                    SELECT * FROM FoodItems 
                    WHERE Category IN (
                        SELECT PreferenceValue FROM UserPreferences 
                        WHERE UserId = @UserId AND PreferenceKey = 'AllowedCategories'
                    )
                    ORDER BY Calories ASC
                    LIMIT 20";
                
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.Id);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            suggestions.Add(new FoodItem
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Category = reader["Category"].ToString(),
                                Calories = Convert.ToDecimal(reader["Calories"]),
                                Protein = Convert.ToDecimal(reader["Protein"]),
                                Carbs = Convert.ToDecimal(reader["Carbs"]),
                                Fat = Convert.ToDecimal(reader["Fat"]),
                                ImageUrl = reader["ImageUrl"]?.ToString(),
                                ImageHash = reader["ImageHash"]?.ToString(),
                                ProviderId = Convert.ToInt32(reader["ProviderId"])
                            });
                        }
                    }
                }
            }
            
            // Daca nu sunt preferinte setate, returnam toate alimentele
            if (suggestions.Count == 0)
            {
                var foodService = new FoodItemService();
                suggestions = foodService.GetAllFoodItems().Take(20).ToList();
            }
            
            DisplaySuggestions(suggestions, user.TargetCalories);
            
            return suggestions;
        }

        public MealPlan CreateMealPlan(int userId, string name, int days, string dietaryPreference)
        {
            Console.WriteLine($"\n=== CREARE PLAN ALIMENTAR: {name} ===");
            
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                
                // Inserare plan alimentar
                string insertPlanQuery = @"
                    INSERT INTO MealPlans (Name, UserId, StartDate, EndDate, DietaryPreference, TargetCalories, IsActive)
                    VALUES (@Name, @UserId, @StartDate, @EndDate, @DietaryPreference, @TargetCalories, @IsActive)";
                
                using (var command = new SQLiteCommand(insertPlanQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@StartDate", DateTime.Now);
                    command.Parameters.AddWithValue("@EndDate", DateTime.Now.AddDays(days));
                    command.Parameters.AddWithValue("@DietaryPreference", dietaryPreference);
                    command.Parameters.AddWithValue("@TargetCalories", 2000); // default
                    command.Parameters.AddWithValue("@IsActive", 1);
                    command.ExecuteNonQuery();
                }
                
                // Obtinere ID plan creat
                command.CommandText = "SELECT last_insert_rowid()";
                int mealPlanId = Convert.ToInt32(command.ExecuteScalar());
                
                Console.WriteLine($"Plan alimentar creat cu ID: {mealPlanId}");
                
                // Adaugare iteme la plan (simplificat)
                PopulateMealPlan(mealPlanId, days, dietaryPreference);
                
                return new MealPlan
                {
                    Id = mealPlanId,
                    Name = name,
                    UserId = userId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(days),
                    DietaryPreference = dietaryPreference,
                    IsActive = true
                };
            }
        }

        private void PopulateMealPlan(int mealPlanId, int days, string dietaryPreference)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                
                string[] mealTypes = { "Mic Dejun", "Pranz", "Cina", "Gustare" };
                var random = new Random();
                
                foreach (var day in Enumerable.Range(1, days))
                {
                    foreach (var mealType in mealTypes)
                    {
                        // Selectare aleatorie de alimente
                        string query = @"
                            SELECT Id, Calories FROM FoodItems 
                            WHERE Category NOT IN ('Dulciuri')
                            ORDER BY RANDOM() 
                            LIMIT 1";
                        
                        using (var command = new SQLiteCommand(query, connection))
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int foodItemId = Convert.ToInt32(reader["Id"]);
                                
                                string insertItemQuery = @"
                                    INSERT INTO MealPlanItems (MealPlanId, FoodItemId, MealType, DayNumber, Quantity, Unit)
                                    VALUES (@MealPlanId, @FoodItemId, @MealType, @DayNumber, @Quantity, @Unit)";
                                
                                using (var itemCommand = new SQLiteCommand(insertItemQuery, connection))
                                {
                                    itemCommand.Parameters.AddWithValue("@MealPlanId", mealPlanId);
                                    itemCommand.Parameters.AddWithValue("@FoodItemId", foodItemId);
                                    itemCommand.Parameters.AddWithValue("@MealType", mealType);
                                    itemCommand.Parameters.AddWithValue("@DayNumber", day);
                                    itemCommand.Parameters.AddWithValue("@Quantity", random.Next(100, 300));
                                    itemCommand.Parameters.AddWithValue("@Unit", "g");
                                    itemCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                
                Console.WriteLine($"Planul alimentar a fost populat cu mese pentru {days} zile.");
            }
        }

        public void DisplayMealPlan(int mealPlanId)
        {
            Console.WriteLine($"\n=== PLAN ALIMENTAR ID: {mealPlanId} ===");
            
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                
                string query = @"
                    SELECT mpi.DayNumber, mpi.MealType, fi.Name, fi.Calories, mpi.Quantity, mpi.Unit
                    FROM MealPlanItems mpi
                    JOIN FoodItems fi ON mpi.FoodItemId = fi.Id
                    WHERE mpi.MealPlanId = @MealPlanId
                    ORDER BY mpi.DayNumber, 
                        CASE mpi.MealType 
                            WHEN 'Mic Dejun' THEN 1 
                            WHEN 'Pranz' THEN 2 
                            WHEN 'Cina' THEN 3 
                            WHEN 'Gustare' THEN 4 
                        END";
                
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MealPlanId", mealPlanId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        int currentDay = -1;
                        
                        while (reader.Read())
                        {
                            int day = Convert.ToInt32(reader["DayNumber"]);
                            
                            if (day != currentDay)
                            {
                                Console.WriteLine($"\n--- Ziua {day} ---");
                                currentDay = day;
                            }
                            
                            Console.WriteLine($"  {reader["MealType"]}: {reader["Name"]} ({reader["Quantity"]}{reader["Unit"]}) - {reader["Calories"]} kcal");
                        }
                    }
                }
            }
        }

        private void DisplaySuggestions(List<FoodItem> suggestions, int targetCalories)
        {
            Console.WriteLine("Alimente sugerate (sortate dupa calorii):");
            Console.WriteLine(new string('-', 80));
            Console.WriteLine($"{"Nume",-30} {"Categorie",-15} {"Calorii",10} {"Proteine",10}");
            Console.WriteLine(new string('-', 80));
            
            foreach (var item in suggestions.Take(10))
            {
                Console.WriteLine($"{item.Name,-30} {item.Category,-15} {item.Calories,10} {item.Protein,10}");
            }
            
            Console.WriteLine(new string('-', 80));
            Console.WriteLine($"\nTotal sugestii: {suggestions.Count} alimente");
        }

        /// <summary>
        /// Cerinta 7: Notificare aparitie anunt care corespunde cerintelor utilizatorului - fire de executie paralele
        /// </summary>
        public async Task CheckAndNotifyUsersAsync(FoodItem newItem)
        {
            Console.WriteLine($"\n=== VERIFICARE NOTIFICARI PENTRU: {newItem.Name} ===");
            
            var users = GetAllUsersWithNotificationsEnabled();
            
            var tasks = users.Select(async user =>
            {
                await Task.Run(() =>
                {
                    // Verificare daca alimentul se potriveste cu preferintele utilizatorului
                    bool matches = CheckIfMatchesPreferences(newItem, user);
                    
                    if (matches)
                    {
                        SendNotification(user, $"Aliment nou disponibil: {newItem.Name} ({newItem.Category}) - {newItem.Calories} kcal");
                        Console.WriteLine($"  [NOTIFICAT] Utilizator {user.Name} despre {newItem.Name}");
                    }
                });
            });
            
            await Task.WhenAll(tasks);
            
            Console.WriteLine("=== VERIFICARE NOTIFICARI COMPLETATA ===\n");
        }

        private List<User> GetAllUsersWithNotificationsEnabled()
        {
            var users = new List<User>();
            
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM Users WHERE NotificationsEnabled = 1";
                
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
                            DietaryPreference = reader["DietaryPreference"]?.ToString(),
                            TargetCalories = Convert.ToInt32(reader["TargetCalories"]),
                            NotificationsEnabled = true
                        });
                    }
                }
            }
            
            return users;
        }

        private bool CheckIfMatchesPreferences(FoodItem item, User user)
        {
            // Logica simpla de verificare
            if (string.IsNullOrEmpty(user.DietaryPreference))
                return true;
            
            // Exemplu: vegetarian nu vrea carne
            if (user.DietaryPreference == "Vegetarian" && item.Category == "Carne")
                return false;
            
            if (user.DietaryPreference == "Vegan" && 
                (item.Category == "Carne" || item.Category == "Peste" || item.Category == "Lactate"))
                return false;
            
            return true;
        }

        private void SendNotification(User user, string message)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"
                    INSERT INTO Notifications (UserId, Message, IsRead, CreatedAt)
                    VALUES (@UserId, @Message, 0, @CreatedAt)";
                
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.Id);
                    command.Parameters.AddWithValue("@Message", message);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
