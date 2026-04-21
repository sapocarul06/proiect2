using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using MealPlanApp.Models;

namespace MealPlanApp.Services
{
    /// <summary>
    /// Cerinta 10: Generare de rapoarte PDF / Excel
    /// </summary>
    public class ReportService
    {
        /// <summary>
        /// Generare raport Excel (format CSV pentru simplitate)
        /// In productie s-ar folosi o librarie precum EPPlus sau ClosedXML
        /// </summary>
        public void GenerateExcelReport(string outputPath)
        {
            Console.WriteLine($"\n=== GENERARE RAPORT EXCEL ===");
            Console.WriteLine($"Output: {outputPath}\n");
            
            var foodItems = GetAllFoodItemsWithDetails();
            
            // Generare CSV (simulare Excel)
            var csv = new StringBuilder();
            
            // Header
            csv.AppendLine("ID;Nume;Categorie;Calorii;Proteine;Carbohidrati;Grasimi;Provider;Data Creare");
            
            // Date
            foreach (var item in foodItems)
            {
                csv.AppendLine($"{item.Id};{item.Name};{item.Category};{item.Calories};{item.Protein};{item.Carbs};{item.Fat};{item.ProviderName};{item.CreatedAt:yyyy-MM-dd}");
            }
            
            File.WriteAllText(outputPath, csv.ToString(), Encoding.UTF8);
            
            Console.WriteLine($"Raport Excel (CSV) generat cu succes!");
            Console.WriteLine($"Total randuri: {foodItems.Count}");
            Console.WriteLine($"Fisier salvat la: {outputPath}\n");
        }

        /// <summary>
        /// Generare raport PDF (simulare - text formatat)
        /// In productie s-ar folosi o librarie precum iTextSharp sau QuestPDF
        /// </summary>
        public void GeneratePdfReport(string outputPath)
        {
            Console.WriteLine($"\n=== GENERARE RAPORT PDF ===");
            Console.WriteLine($"Output: {outputPath}\n");
            
            var reportData = GetReportData();
            
            var pdfContent = new StringBuilder();
            
            // Simulare continut PDF
            pdfContent.AppendLine("===============================================");
            pdfContent.AppendLine("       RAPORT PLAN ALIMENTAR - MEAL PLAN APP");
            pdfContent.AppendLine("===============================================");
            pdfContent.AppendLine($"Data generare: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            pdfContent.AppendLine("");
            
            // Sectiunea 1: Statistici generale
            pdfContent.AppendLine("--- STATISTICI GENERALE ---");
            pdfContent.AppendLine($"Total alimente in baza de date: {reportData.TotalFoodItems}");
            pdfContent.AppendLine($"Total utilizatori: {reportData.TotalUsers}");
            pdfContent.AppendLine($"Total planuri alimentare: {reportData.TotalMealPlans}");
            pdfContent.AppendLine($"Total importuri: {reportData.TotalImports}");
            pdfContent.AppendLine("");
            
            // Sectiunea 2: Top alimente dupa calorii
            pdfContent.AppendLine("--- TOP 5 ALIMENTE CU CELE MAI MULTE CALORII ---");
            int rank = 1;
            foreach (var item in reportData.TopCaloriesFood)
            {
                pdfContent.AppendLine($"{rank}. {item.Name} - {item.Calories} kcal ({item.Category})");
                rank++;
            }
            pdfContent.AppendLine("");
            
            // Sectiunea 3: Distributie pe categorii
            pdfContent.AppendLine("--- DISTRIBUTIE ALIMENTE PE CATEGORII ---");
            foreach (var cat in reportData.CategoryDistribution)
            {
                pdfContent.AppendLine($"  {cat.Key}: {cat.Value} alimente");
            }
            pdfContent.AppendLine("");
            
            // Sectiunea 4: Activitate importuri
            pdfContent.AppendLine("--- ACTIVITATE IMPORTURI ---");
            foreach (var log in reportData.RecentImportLogs)
            {
                pdfContent.AppendLine($"  [{log.ImportDate:dd.MM.yyyy}] Provider {log.ProviderId}: " +
                    $"{log.RecordsImported} importate, {log.RecordsUpdated} actualizate, {log.Errors} erori");
            }
            pdfContent.AppendLine("");
            
            pdfContent.AppendLine("===============================================");
            pdfContent.AppendLine("                 SFARSIT RAPORT");
            pdfContent.AppendLine("===============================================");
            
            File.WriteAllText(outputPath, pdfContent.ToString(), Encoding.UTF8);
            
            Console.WriteLine($"Raport PDF (text formatat) generat cu succes!");
            Console.WriteLine($"Fisier salvat la: {outputPath}\n");
        }

        /// <summary>
        /// Generare raport nutritional pentru un plan alimentar
        /// </summary>
        public void GenerateNutritionReport(int mealPlanId, string outputPath)
        {
            Console.WriteLine($"\n=== GENERARE RAPORT NUTRITIONAL ===");
            Console.WriteLine($"Plan ID: {mealPlanId}");
            Console.WriteLine($"Output: {outputPath}\n");
            
            var nutritionData = GetNutritionDataForMealPlan(mealPlanId);
            
            var report = new StringBuilder();
            
            report.AppendLine("===============================================");
            report.AppendLine("         RAPORT NUTRITIONAL ZILNIC");
            report.AppendLine("===============================================");
            report.AppendLine("");
            
            foreach (var day in nutritionData.DailyTotals)
            {
                report.AppendLine($"--- ZIUA {day.DayNumber} ---");
                report.AppendLine($"  Calorii totale: {day.TotalCalories:F0} kcal");
                report.AppendLine($"  Proteine: {day.TotalProtein:F1} g");
                report.AppendLine($"  Carbohidrati: {day.TotalCarbs:F1} g");
                report.AppendLine($"  Grasimi: {day.TotalFat:F1} g");
                report.AppendLine("");
                
                report.AppendLine("  Mese:");
                foreach (var meal in day.Meals)
                {
                    report.AppendLine($"    {meal.MealType}: {meal.ItemName} - {meal.Calories} kcal");
                }
                report.AppendLine("");
            }
            
            report.AppendLine("--- RECOMANDARI ---");
            report.AppendLine($"Media zilnica de calorii: {nutritionData.AverageDailyCalories:F0} kcal");
            report.AppendLine($"Tinta recomandata: 2000 kcal");
            
            if (nutritionData.AverageDailyCalories > 2200)
                report.AppendLine("Atentie: Depasiti tinta calorica recomandata!");
            else if (nutritionData.AverageDailyCalories < 1800)
                report.AppendLine("Sfat: Incercati sa cresteti usor aportul caloric.");
            else
                report.AppendLine("Felicitari! Sunteti in limita calorica recomandata.");
            
            report.AppendLine("");
            report.AppendLine("===============================================");
            
            File.WriteAllText(outputPath, report.ToString(), Encoding.UTF8);
            
            Console.WriteLine($"Raport nutritional generat cu succes!");
            Console.WriteLine($"Fisier salvat la: {outputPath}\n");
        }

        private List<FoodItemWithProvider> GetAllFoodItemsWithDetails()
        {
            var items = new List<FoodItemWithProvider>();
            
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"
                    SELECT fi.*, p.Name as ProviderName 
                    FROM FoodItems fi
                    LEFT JOIN Providers p ON fi.ProviderId = p.Id
                    ORDER BY fi.Name";
                
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new FoodItemWithProvider
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Category = reader["Category"].ToString(),
                            Calories = Convert.ToDecimal(reader["Calories"]),
                            Protein = Convert.ToDecimal(reader["Protein"]),
                            Carbs = Convert.ToDecimal(reader["Carbs"]),
                            Fat = Convert.ToDecimal(reader["Fat"]),
                            ProviderName = reader["ProviderName"]?.ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        });
                    }
                }
            }
            
            return items;
        }

        private ReportData GetReportData()
        {
            var data = new ReportData();
            
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                
                // Total food items
                var cmd = new SQLiteCommand("SELECT COUNT(*) FROM FoodItems", connection);
                data.TotalFoodItems = Convert.ToInt32(cmd.ExecuteScalar());
                
                // Total users
                cmd = new SQLiteCommand("SELECT COUNT(*) FROM Users", connection);
                data.TotalUsers = Convert.ToInt32(cmd.ExecuteScalar());
                
                // Total meal plans
                cmd = new SQLiteCommand("SELECT COUNT(*) FROM MealPlans", connection);
                data.TotalMealPlans = Convert.ToInt32(cmd.ExecuteScalar());
                
                // Total imports
                cmd = new SQLiteCommand("SELECT COUNT(*) FROM ImportLogs", connection);
                data.TotalImports = Convert.ToInt32(cmd.ExecuteScalar());
                
                // Top calories food
                cmd = new SQLiteCommand(@"
                    SELECT Name, Category, Calories FROM FoodItems 
                    ORDER BY Calories DESC LIMIT 5", connection);
                
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.TopCaloriesFood.Add(new FoodItem
                        {
                            Name = reader["Name"].ToString(),
                            Category = reader["Category"].ToString(),
                            Calories = Convert.ToDecimal(reader["Calories"])
                        });
                    }
                }
                
                // Category distribution
                cmd = new SQLiteCommand(@"
                    SELECT Category, COUNT(*) as Count FROM FoodItems 
                    GROUP BY Category ORDER BY Count DESC", connection);
                
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.CategoryDistribution[reader["Category"].ToString()] = 
                            Convert.ToInt32(reader["Count"]);
                    }
                }
                
                // Recent import logs
                cmd = new SQLiteCommand(@"
                    SELECT ProviderId, ImportDate, RecordsImported, RecordsUpdated, Errors 
                    FROM ImportLogs ORDER BY ImportDate DESC LIMIT 5", connection);
                
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.RecentImportLogs.Add(new ImportLog
                        {
                            ProviderId = Convert.ToInt32(reader["ProviderId"]),
                            ImportDate = Convert.ToDateTime(reader["ImportDate"]),
                            RecordsImported = Convert.ToInt32(reader["RecordsImported"]),
                            RecordsUpdated = Convert.ToInt32(reader["RecordsUpdated"]),
                            Errors = Convert.ToInt32(reader["Errors"])
                        });
                    }
                }
            }
            
            return data;
        }

        private NutritionReportData GetNutritionDataForMealPlan(int mealPlanId)
        {
            var data = new NutritionReportData();
            
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                
                string query = @"
                    SELECT mpi.DayNumber, mpi.MealType, fi.Name as ItemName, 
                           fi.Calories, fi.Protein, fi.Carbs, fi.Fat
                    FROM MealPlanItems mpi
                    JOIN FoodItems fi ON mpi.FoodItemId = fi.Id
                    WHERE mpi.MealPlanId = @MealPlanId
                    ORDER BY mpi.DayNumber, mpi.MealType";
                
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MealPlanId", mealPlanId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        var dailyGroups = new Dictionary<int, DailyNutrition>();
                        
                        while (reader.Read())
                        {
                            int day = Convert.ToInt32(reader["DayNumber"]);
                            
                            if (!dailyGroups.ContainsKey(day))
                            {
                                dailyGroups[day] = new DailyNutrition { DayNumber = day };
                            }
                            
                            dailyGroups[day].Meals.Add(new MealInfo
                            {
                                MealType = reader["MealType"].ToString(),
                                ItemName = reader["ItemName"].ToString(),
                                Calories = Convert.ToDecimal(reader["Calories"]),
                                Protein = Convert.ToDecimal(reader["Protein"]),
                                Carbs = Convert.ToDecimal(reader["Carbs"]),
                                Fat = Convert.ToDecimal(reader["Fat"])
                            });
                            
                            dailyGroups[day].TotalCalories += Convert.ToDecimal(reader["Calories"]);
                            dailyGroups[day].TotalProtein += Convert.ToDecimal(reader["Protein"]);
                            dailyGroups[day].TotalCarbs += Convert.ToDecimal(reader["Carbs"]);
                            dailyGroups[day].TotalFat += Convert.ToDecimal(reader["Fat"]);
                        }
                        
                        data.DailyTotals = dailyGroups.Values.ToList();
                        
                        if (data.DailyTotals.Count > 0)
                        {
                            data.AverageDailyCalories = 
                                data.DailyTotals.Average(d => d.TotalCalories);
                        }
                    }
                }
            }
            
            return data;
        }

        // Clase auxiliare
        private class FoodItemWithProvider : FoodItem
        {
            public string ProviderName { get; set; }
        }

        private class ReportData
        {
            public int TotalFoodItems { get; set; }
            public int TotalUsers { get; set; }
            public int TotalMealPlans { get; set; }
            public int TotalImports { get; set; }
            public List<FoodItem> TopCaloriesFood { get; set; } = new List<FoodItem>();
            public Dictionary<string, int> CategoryDistribution { get; set; } = new Dictionary<string, int>();
            public List<ImportLog> RecentImportLogs { get; set; } = new List<ImportLog>();
        }

        private class NutritionReportData
        {
            public List<DailyNutrition> DailyTotals { get; set; } = new List<DailyNutrition>();
            public decimal AverageDailyCalories { get; set; }
        }

        private class DailyNutrition
        {
            public int DayNumber { get; set; }
            public decimal TotalCalories { get; set; }
            public decimal TotalProtein { get; set; }
            public decimal TotalCarbs { get; set; }
            public decimal TotalFat { get; set; }
            public List<MealInfo> Meals { get; set; } = new List<MealInfo>();
        }

        private class MealInfo
        {
            public string MealType { get; set; }
            public string ItemName { get; set; }
            public decimal Calories { get; set; }
            public decimal Protein { get; set; }
            public decimal Carbs { get; set; }
            public decimal Fat { get; set; }
        }
    }
}
