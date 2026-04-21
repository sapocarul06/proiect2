using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using MealPlanApp.Models;

namespace MealPlanApp.Data
{
    public class DatabaseHelper
    {
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mealplan.db");
        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";

        public static void Initialize()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                var foodTable = @"
                    CREATE TABLE IF NOT EXISTS Foods (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Description TEXT,
                        Calories REAL,
                        Protein REAL,
                        Carbs REAL,
                        Fat REAL,
                        ImageUrl TEXT,
                        ImageHash TEXT,
                        Provider TEXT,
                        CreatedAt TEXT,
                        UpdatedAt TEXT
                    )";

                var userTable = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Email TEXT,
                        CreatedAt TEXT
                    )";

                var preferenceTable = @"
                    CREATE TABLE IF NOT EXISTS UserPreferences (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER,
                        DietaryType TEXT,
                        MinCalories REAL,
                        MaxCalories REAL,
                        MinProtein REAL,
                        MaxCarbs REAL,
                        MaxFat REAL,
                        ExcludedIngredients TEXT,
                        CreatedAt TEXT,
                        UpdatedAt TEXT,
                        FOREIGN KEY(UserId) REFERENCES Users(Id)
                    )";

                using (var cmd = new SQLiteCommand(foodTable, connection))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SQLiteCommand(userTable, connection))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SQLiteCommand(preferenceTable, connection))
                    cmd.ExecuteNonQuery();
            }
        }

        public static void InsertFood(Food food)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var sql = @"INSERT INTO Foods (Name, Description, Calories, Protein, Carbs, Fat, ImageUrl, ImageHash, Provider, CreatedAt, UpdatedAt)
                           VALUES (@Name, @Description, @Calories, @Protein, @Carbs, @Fat, @ImageUrl, @ImageHash, @Provider, @CreatedAt, @UpdatedAt)";
                
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", food.Name);
                    cmd.Parameters.AddWithValue("@Description", food.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Calories", food.Calories);
                    cmd.Parameters.AddWithValue("@Protein", food.Protein);
                    cmd.Parameters.AddWithValue("@Carbs", food.Carbs);
                    cmd.Parameters.AddWithValue("@Fat", food.Fat);
                    cmd.Parameters.AddWithValue("@ImageUrl", food.ImageUrl ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImageHash", food.ImageHash ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Provider", food.Provider);
                    cmd.Parameters.AddWithValue("@CreatedAt", food.CreatedAt.ToString("o"));
                    cmd.Parameters.AddWithValue("@UpdatedAt", food.UpdatedAt.ToString("o"));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<Food> GetAllFoods()
        {
            var foods = new List<Food>();
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var sql = "SELECT * FROM Foods";
                using (var cmd = new SQLiteCommand(sql, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        foods.Add(new Food
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"]?.ToString(),
                            Calories = Convert.ToDecimal(reader["Calories"]),
                            Protein = Convert.ToDecimal(reader["Protein"]),
                            Carbs = Convert.ToDecimal(reader["Carbs"]),
                            Fat = Convert.ToDecimal(reader["Fat"]),
                            ImageUrl = reader["ImageUrl"]?.ToString(),
                            ImageHash = reader["ImageHash"]?.ToString(),
                            Provider = reader["Provider"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                            UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                        });
                    }
                }
            }
            return foods;
        }

        public static List<Food> GetFoodsByProvider(string provider)
        {
            var foods = new List<Food>();
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var sql = "SELECT * FROM Foods WHERE Provider = @Provider";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Provider", provider);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foods.Add(new Food
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"]?.ToString(),
                                Calories = Convert.ToDecimal(reader["Calories"]),
                                Protein = Convert.ToDecimal(reader["Protein"]),
                                Carbs = Convert.ToDecimal(reader["Carbs"]),
                                Fat = Convert.ToDecimal(reader["Fat"]),
                                ImageUrl = reader["ImageUrl"]?.ToString(),
                                ImageHash = reader["ImageHash"]?.ToString(),
                                Provider = reader["Provider"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                            });
                        }
                    }
                }
            }
            return foods;
        }

        public static bool FoodExistsByHash(string imageHash)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var sql = "SELECT COUNT(*) FROM Foods WHERE ImageHash = @ImageHash";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@ImageHash", imageHash);
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public static void UpdateFood(Food food)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var sql = @"UPDATE Foods SET Name = @Name, Description = @Description, Calories = @Calories, 
                           Protein = @Protein, Carbs = @Carbs, Fat = @Fat, UpdatedAt = @UpdatedAt 
                           WHERE Id = @Id";
                
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", food.Id);
                    cmd.Parameters.AddWithValue("@Name", food.Name);
                    cmd.Parameters.AddWithValue("@Description", food.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Calories", food.Calories);
                    cmd.Parameters.AddWithValue("@Protein", food.Protein);
                    cmd.Parameters.AddWithValue("@Carbs", food.Carbs);
                    cmd.Parameters.AddWithValue("@Fat", food.Fat);
                    cmd.Parameters.AddWithValue("@UpdatedAt", food.UpdatedAt.ToString("o"));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int InsertUser(User user)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var sql = @"INSERT INTO Users (Name, Email, CreatedAt) VALUES (@Name, @Email, @CreatedAt);
                           SELECT last_insert_rowid();";
                
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedAt", user.CreatedAt.ToString("o"));
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static void InsertUserPreference(UserPreference pref)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var sql = @"INSERT INTO UserPreferences (UserId, DietaryType, MinCalories, MaxCalories, MinProtein, MaxCarbs, MaxFat, ExcludedIngredients, CreatedAt, UpdatedAt)
                           VALUES (@UserId, @DietaryType, @MinCalories, @MaxCalories, @MinProtein, @MaxCarbs, @MaxFat, @ExcludedIngredients, @CreatedAt, @UpdatedAt)";
                
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", pref.UserId);
                    cmd.Parameters.AddWithValue("@DietaryType", pref.DietaryType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MinCalories", pref.MinCalories);
                    cmd.Parameters.AddWithValue("@MaxCalories", pref.MaxCalories);
                    cmd.Parameters.AddWithValue("@MinProtein", pref.MinProtein);
                    cmd.Parameters.AddWithValue("@MaxCarbs", pref.MaxCarbs);
                    cmd.Parameters.AddWithValue("@MaxFat", pref.MaxFat);
                    cmd.Parameters.AddWithValue("@ExcludedIngredients", pref.ExcludedIngredients ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedAt", pref.CreatedAt.ToString("o"));
                    cmd.Parameters.AddWithValue("@UpdatedAt", pref.UpdatedAt.ToString("o"));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static UserPreference GetUserPreference(int userId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                var sql = "SELECT * FROM UserPreferences WHERE UserId = @UserId LIMIT 1";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserPreference
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                UserId = Convert.ToInt32(reader["UserId"]),
                                DietaryType = reader["DietaryType"]?.ToString(),
                                MinCalories = Convert.ToDecimal(reader["MinCalories"]),
                                MaxCalories = Convert.ToDecimal(reader["MaxCalories"]),
                                MinProtein = Convert.ToDecimal(reader["MinProtein"]),
                                MaxCarbs = Convert.ToDecimal(reader["MaxCarbs"]),
                                MaxFat = Convert.ToDecimal(reader["MaxFat"]),
                                ExcludedIngredients = reader["ExcludedIngredients"]?.ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
