using System;
using System.Data.SQLite;
using System.IO;

namespace MealPlanApp.Data
{
    public class DatabaseHelper
    {
        private static string _connectionString;
        
        public static void Initialize(string dbPath)
        {
            _connectionString = $"Data Source={dbPath};Version=3;";
            
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }
            
            CreateTables();
        }
        
        public static string GetConnectionString()
        {
            return _connectionString;
        }
        
        private static void CreateTables()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                
                // Tabelul Providers
                string createProvidersTable = @"
                    CREATE TABLE IF NOT EXISTS Providers (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        ApiUrl TEXT,
                        ApiKey TEXT,
                        IsActive INTEGER DEFAULT 1
                    )";
                
                // Tabelul FoodItems
                string createFoodItemsTable = @"
                    CREATE TABLE IF NOT EXISTS FoodItems (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Category TEXT,
                        Calories DECIMAL(10,2),
                        Protein DECIMAL(10,2),
                        Carbs DECIMAL(10,2),
                        Fat DECIMAL(10,2),
                        ImageUrl TEXT,
                        ImageHash TEXT,
                        ProviderId INTEGER,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                        UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (ProviderId) REFERENCES Providers(Id)
                    )";
                
                // Tabelul Users
                string createUsersTable = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Email TEXT UNIQUE NOT NULL,
                        DietaryPreference TEXT,
                        TargetCalories INTEGER,
                        NotificationsEnabled INTEGER DEFAULT 1,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                    )";
                
                // Tabelul MealPlans
                string createMealPlansTable = @"
                    CREATE TABLE IF NOT EXISTS MealPlans (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        UserId INTEGER,
                        StartDate DATETIME,
                        EndDate DATETIME,
                        DietaryPreference TEXT,
                        TargetCalories INTEGER,
                        IsActive INTEGER DEFAULT 1,
                        FOREIGN KEY (UserId) REFERENCES Users(Id)
                    )";
                
                // Tabelul MealPlanItems
                string createMealPlanItemsTable = @"
                    CREATE TABLE IF NOT EXISTS MealPlanItems (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        MealPlanId INTEGER,
                        FoodItemId INTEGER,
                        MealType TEXT,
                        DayNumber INTEGER,
                        Quantity DECIMAL(10,2),
                        Unit TEXT,
                        FOREIGN KEY (MealPlanId) REFERENCES MealPlans(Id),
                        FOREIGN KEY (FoodItemId) REFERENCES FoodItems(Id)
                    )";
                
                // Tabelul UserPreferences
                string createUserPreferencesTable = @"
                    CREATE TABLE IF NOT EXISTS UserPreferences (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER,
                        PreferenceKey TEXT,
                        PreferenceValue TEXT,
                        FOREIGN KEY (UserId) REFERENCES Users(Id)
                    )";
                
                // Tabelul Notifications
                string createNotificationsTable = @"
                    CREATE TABLE IF NOT EXISTS Notifications (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER,
                        Message TEXT,
                        IsRead INTEGER DEFAULT 0,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (UserId) REFERENCES Users(Id)
                    )";
                
                // Tabelul ImportLogs
                string createImportLogsTable = @"
                    CREATE TABLE IF NOT EXISTS ImportLogs (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ProviderId INTEGER,
                        ImportDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                        RecordsImported INTEGER,
                        RecordsUpdated INTEGER,
                        Errors INTEGER,
                        Status TEXT,
                        FOREIGN KEY (ProviderId) REFERENCES Providers(Id)
                    )";
                
                // Index pentru ImageHash (identificare duplicate)
                string createImageHashIndex = @"
                    CREATE INDEX IF NOT EXISTS IX_FoodItems_ImageHash ON FoodItems(ImageHash)";
                
                // Index pentru cautari dupa preferinte
                string createDietaryIndex = @"
                    CREATE INDEX IF NOT EXISTS IX_FoodItems_Category ON FoodItems(Category)";
                
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = createProvidersTable;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createFoodItemsTable;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createUsersTable;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createMealPlansTable;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createMealPlanItemsTable;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createUserPreferencesTable;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createNotificationsTable;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createImportLogsTable;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createImageHashIndex;
                    command.ExecuteNonQuery();
                    
                    command.CommandText = createDietaryIndex;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
