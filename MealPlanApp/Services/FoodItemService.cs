using System;
using System.Collections.Generic;
using System.Data.SQLite;
using MealPlanApp.Models;

namespace MealPlanApp.Services
{
    public class FoodItemService
    {
        public void AddFoodItem(FoodItem item)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"
                    INSERT INTO FoodItems (Name, Category, Calories, Protein, Carbs, Fat, ImageUrl, ImageHash, ProviderId, CreatedAt, UpdatedAt)
                    VALUES (@Name, @Category, @Calories, @Protein, @Carbs, @Fat, @ImageUrl, @ImageHash, @ProviderId, @CreatedAt, @UpdatedAt)";
                
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", item.Name);
                    command.Parameters.AddWithValue("@Category", item.Category);
                    command.Parameters.AddWithValue("@Calories", item.Calories);
                    command.Parameters.AddWithValue("@Protein", item.Protein);
                    command.Parameters.AddWithValue("@Carbs", item.Carbs);
                    command.Parameters.AddWithValue("@Fat", item.Fat);
                    command.Parameters.AddWithValue("@ImageUrl", item.ImageUrl ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ImageHash", item.ImageHash ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ProviderId", item.ProviderId);
                    command.Parameters.AddWithValue("@CreatedAt", item.CreatedAt);
                    command.Parameters.AddWithValue("@UpdatedAt", item.UpdatedAt);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateFoodItem(FoodItem item)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"
                    UPDATE FoodItems 
                    SET Name = @Name, Category = @Category, Calories = @Calories, Protein = @Protein, 
                        Carbs = @Carbs, Fat = @Fat, ImageUrl = @ImageUrl, ImageHash = @ImageHash, 
                        ProviderId = @ProviderId, UpdatedAt = @UpdatedAt
                    WHERE Id = @Id";
                
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", item.Id);
                    command.Parameters.AddWithValue("@Name", item.Name);
                    command.Parameters.AddWithValue("@Category", item.Category);
                    command.Parameters.AddWithValue("@Calories", item.Calories);
                    command.Parameters.AddWithValue("@Protein", item.Protein);
                    command.Parameters.AddWithValue("@Carbs", item.Carbs);
                    command.Parameters.AddWithValue("@Fat", item.Fat);
                    command.Parameters.AddWithValue("@ImageUrl", item.ImageUrl ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ImageHash", item.ImageHash ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ProviderId", item.ProviderId);
                    command.Parameters.AddWithValue("@UpdatedAt", item.UpdatedAt);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<FoodItem> GetFoodItemsByCategory(string category)
        {
            var items = new List<FoodItem>();
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM FoodItems WHERE Category = @Category";
                
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Category", category);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new FoodItem
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
                                ProviderId = Convert.ToInt32(reader["ProviderId"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<FoodItem> GetAllFoodItems()
        {
            var items = new List<FoodItem>();
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM FoodItems ORDER BY Name";
                
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new FoodItem
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
                            ProviderId = Convert.ToInt32(reader["ProviderId"]),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                            UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                        });
                    }
                }
            }
            return items;
        }

        public FoodItem GetFoodItemById(int id)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT * FROM FoodItems WHERE Id = @Id";
                
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new FoodItem
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
                                ProviderId = Convert.ToInt32(reader["ProviderId"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool ExistsByImageHash(string imageHash)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM FoodItems WHERE ImageHash = @ImageHash";
                
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ImageHash", imageHash);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }
    }
}
