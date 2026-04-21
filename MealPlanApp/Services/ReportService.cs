using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MealPlanApp.Models;
using MealPlanApp.Data;

namespace MealPlanApp.Services
{
    /// <summary>
    /// Cerința 10: Generare de rapoarte PDF / Excel
    /// </summary>
    public class ReportService
    {
        private readonly string _reportsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");

        public ReportService()
        {
            if (!Directory.Exists(_reportsFolder))
            {
                Directory.CreateDirectory(_reportsFolder);
            }
        }

        /// <summary>
        /// Generează raport în format Excel (CSV)
        /// </summary>
        public string GenerateExcelReport()
        {
            Console.WriteLine("=== Generare Raport Excel (CSV) ===");
            
            var foods = DatabaseHelper.GetAllFoods();
            var fileName = $"MealPlan_Report_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            var filePath = Path.Combine(_reportsFolder, fileName);
            
            var sb = new StringBuilder();
            
            // Header
            sb.AppendLine("ID,Nume,Descriere,Calorii,Proteine,Carbohidrati,Grasimi,Provider,CreatLa");
            
            // Date
            foreach (var food in foods)
            {
                sb.AppendLine($"{food.Id},\"{food.Name}\",\"{food.Description}\",{food.Calories},{food.Protein},{food.Carbs},{food.Fat},\"{food.Provider}\",{food.CreatedAt:yyyy-MM-dd}");
            }
            
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            
            Console.WriteLine($"✓ Raport CSV generat: {filePath}");
            Console.WriteLine($"  Total înregistrări: {foods.Count}");
            
            return filePath;
        }

        /// <summary>
        /// Generează raport în format PDF (simulat prin text formatat)
        /// În producție s-ar folosi o librărie iTextSharp sau QuestPDF
        /// </summary>
        public string GeneratePdfReport()
        {
            Console.WriteLine("=== Generare Raport PDF ===");
            
            var foods = DatabaseHelper.GetAllFoods();
            var users = GetTotalUserCount();
            var fileName = $"MealPlan_Summary_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            var filePath = Path.Combine(_reportsFolder, fileName);
            
            var sb = new StringBuilder();
            
            sb.AppendLine("╔════════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║           RAPORT PLAN ALIMENTAR - SUMAR                        ║");
            sb.AppendLine($"║           Data: {DateTime.Now:dd.MM.yyyy HH:mm:ss}                            ║");
            sb.AppendLine("╚════════════════════════════════════════════════════════════════╝");
            sb.AppendLine();
            
            // Statistici generale
            sb.AppendLine("📊 STATISTICI GENERALE:");
            sb.AppendLine($"   • Total alimente în baza de date: {foods.Count}");
            sb.AppendLine($"   • Total utilizatori înregistrați: {users}");
            sb.AppendLine($"   • Calorii medii per aliment: {CalculateAverageCalories(foods):F0} kcal");
            sb.AppendLine($"   • Proteine medii per aliment: {CalculateAverageProtein(foods):F1}g");
            sb.AppendLine();
            
            // Alimente pe categorii
            sb.AppendLine("📋 ALIMENTE PE CATEGORII DE CALORII:");
            var lowCal = foods.FindAll(f => f.Calories < 300);
            var mediumCal = foods.FindAll(f => f.Calories >= 300 && f.Calories < 500);
            var highCal = foods.FindAll(f => f.Calories >= 500);
            
            sb.AppendLine($"   • Scăzut (<300 kcal): {lowCal.Count} alimente");
            sb.AppendLine($"   • Mediu (300-500 kcal): {mediumCal.Count} alimente");
            sb.AppendLine($"   • Ridicat (>500 kcal): {highCal.Count} alimente");
            sb.AppendLine();
            
            // Top 5 alimente cu cele mai multe proteine
            sb.AppendLine("🏆 TOP 5 ALIMENTE CU CELE MAI MULTE PROTEINE:");
            var topProtein = foods.OrderByDescending(f => f.Protein).Take(5);
            int rank = 1;
            foreach (var food in topProtein)
            {
                sb.AppendLine($"   {rank}. {food.Name,-25} {food.Protein,5:F1}g proteine");
                rank++;
            }
            sb.AppendLine();
            
            // Provideri
            sb.AppendLine("📦 DISTRIBUȚIE PE PROVIDERI:");
            var providers = foods.GroupBy(f => f.Provider);
            foreach (var provider in providers)
            {
                sb.AppendLine($"   • {provider.Key}: {provider.Count()} alimente");
            }
            sb.AppendLine();
            
            sb.AppendLine("╔════════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║                    SFÂRȘIT RAPORT                              ║");
            sb.AppendLine("╚════════════════════════════════════════════════════════════════╝");
            
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            
            Console.WriteLine($"✓ Raport PDF (text) generat: {filePath}");
            
            return filePath;
        }

        private int CalculateAverageCalories(List<Food> foods)
        {
            if (foods.Count == 0) return 0;
            return (int)(foods.Average(f => f.Calories));
        }

        private decimal CalculateAverageProtein(List<Food> foods)
        {
            if (foods.Count == 0) return 0;
            return (decimal)foods.Average(f => f.Protein);
        }

        private int GetTotalUserCount()
        {
            // Simulare: în producție s-ar număra din DB
            return 3;
        }
    }
}
