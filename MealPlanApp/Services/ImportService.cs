using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MealPlanApp.Models;

namespace MealPlanApp.Services
{
    public class ImportService
    {
        private readonly FoodItemService _foodItemService;

        public ImportService()
        {
            _foodItemService = new FoodItemService();
        }

        /// <summary>
        /// Cerinta 1: Script pentru importul datelor de la mai multi provideri - ruleaza o singura data
        /// </summary>
        public void InitialImport(List<Provider> providers)
        {
            Console.WriteLine("=== IMPORT INITIAL DE DATE ===");
            
            foreach (var provider in providers)
            {
                Console.WriteLine($"Import de la provider: {provider.Name}");
                
                // Simulare date de la provider
                var mockData = GetMockDataFromProvider(provider);
                
                int imported = 0;
                int duplicates = 0;
                
                foreach (var item in mockData)
                {
                    // Verificare duplicat pe baza de ImageHash
                    if (!string.IsNullOrEmpty(item.ImageHash) && _foodItemService.ExistsByImageHash(item.ImageHash))
                    {
                        Console.WriteLine($"  [SKIP] Duplicat detectat: {item.Name}");
                        duplicates++;
                        continue;
                    }
                    
                    _foodItemService.AddFoodItem(item);
                    imported++;
                    Console.WriteLine($"  [OK] Adaugat: {item.Name}");
                }
                
                // Log import
                LogImport(provider.Id, imported, 0, duplicates, "Completed");
                
                Console.WriteLine($"Provider {provider.Name}: {imported} importate, {duplicates} duplicate");
            }
            
            Console.WriteLine("=== IMPORT INITIAL COMPLETAT ===\n");
        }

        /// <summary>
        /// Cerinta 2: Script de import pentru actualizare zilnica - se seteaza ca cron
        /// </summary>
        public void DailyUpdateImport(List<Provider> providers)
        {
            Console.WriteLine("=== ACTUALIZARE ZILNICA DE DATE ===");
            Console.WriteLine($"Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            
            foreach (var provider in providers.Where(p => p.IsActive))
            {
                Console.WriteLine($"\nActualizare de la provider: {provider.Name}");
                
                var mockData = GetMockDataFromProvider(provider);
                
                int updated = 0;
                int inserted = 0;
                int errors = 0;
                
                foreach (var item in mockData)
                {
                    try
                    {
                        // Verificare daca exista deja
                        var existing = _foodItemService.GetFoodItemById(item.Id);
                        
                        if (existing != null)
                        {
                            // Actualizare
                            item.CreatedAt = existing.CreatedAt;
                            _foodItemService.UpdateFoodItem(item);
                            updated++;
                            Console.WriteLine($"  [UPDATE] {item.Name}");
                        }
                        else
                        {
                            // Inserare noua
                            if (!string.IsNullOrEmpty(item.ImageHash) && _foodItemService.ExistsByImageHash(item.ImageHash))
                            {
                                Console.WriteLine($"  [SKIP] Duplicat: {item.Name}");
                                continue;
                            }
                            
                            _foodItemService.AddFoodItem(item);
                            inserted++;
                            Console.WriteLine($"  [INSERT] {item.Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors++;
                        Console.WriteLine($"  [ERROR] {item.Name}: {ex.Message}");
                    }
                }
                
                LogImport(provider.Id, inserted, updated, errors, "Completed");
                Console.WriteLine($"Provider {provider.Name}: {inserted} inserate, {updated} actualizate, {errors} erori");
            }
            
            Console.WriteLine("\n=== ACTUALIZARE ZILNICA COMPLETATA ===\n");
        }

        /// <summary>
        /// Cerinta 3: Identificare anunturi identice pe baza imaginilor
        /// </summary>
        public List<(FoodItem Original, List<FoodItem> Duplicates)> FindDuplicateImages()
        {
            Console.WriteLine("=== CAUTARE DUPLICATE PE BAZA DE IMAGINI ===");
            
            var allItems = _foodItemService.GetAllFoodItems();
            var duplicates = new List<(FoodItem, List<FoodItem>)>();
            
            // Grupare dupa ImageHash
            var grouped = allItems
                .Where(i => !string.IsNullOrEmpty(i.ImageHash))
                .GroupBy(i => i.ImageHash)
                .Where(g => g.Count() > 1)
                .ToList();
            
            foreach (var group in grouped)
            {
                var original = group.First();
                var dupList = group.Skip(1).ToList();
                
                Console.WriteLine($"\n[DUPLICAT] Hash: {group.Key}");
                Console.WriteLine($"  Original: {original.Name} (ID: {original.Id})");
                foreach (var dup in dupList)
                {
                    Console.WriteLine($"  -> Duplicat: {dup.Name} (ID: {dup.Id}, Provider: {dup.ProviderId})");
                }
                
                duplicates.Add((original, dupList));
            }
            
            Console.WriteLine($"\nTotal grupuri de duplicate gasite: {duplicates.Count}");
            return duplicates;
        }

        /// <summary>
        /// Generare hash pentru imagine (simulare - in productie s-ar folosi MD5/SHA al imaginii reale)
        /// </summary>
        public string GenerateImageHash(string imageUrl)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(imageUrl));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private List<FoodItem> GetMockDataFromProvider(Provider provider)
        {
            // Simulare date primite de la API-ul providerului
            var items = new List<FoodItem>();
            var random = new Random();
            
            string[] categories = { "Fructe", "Legume", "Carne", "Peste", "Lactate", "Cereale", "Dulciuri" };
            string[] names = { 
                "Mar", "Para", "Portocala", "Banana", "Struguri",
                "Morcov", "Broccoli", "Spanac", "Rosii", "Castravete",
                "Pui", "Vita", "Porc", "Curcan",
                "Somon", "Ton", "Cod", "Pastrav",
                "Lapte", "Iaurt", "Branza", "Unt",
                "Orez", "Gris", "Quinoa", "Ovaz",
                "Ciocolata", "Ingheata", "Prajitura"
            };
            
            for (int i = 0; i < 5; i++)
            {
                var name = names[random.Next(names.Length)];
                var category = categories[random.Next(categories.Length)];
                var imageUrl = $"https://example.com/images/{name.ToLower()}_{random.Next(1000)}.jpg";
                
                items.Add(new FoodItem
                {
                    Id = provider.Id * 1000 + i,
                    Name = $"{name} ({provider.Name})",
                    Category = category,
                    Calories = random.Next(50, 500),
                    Protein = random.Next(1, 50),
                    Carbs = random.Next(1, 100),
                    Fat = random.Next(1, 30),
                    ImageUrl = imageUrl,
                    ImageHash = GenerateImageHash(imageUrl),
                    ProviderId = provider.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }
            
            return items;
        }

        private void LogImport(int providerId, int imported, int updated, int errors, string status)
        {
            using (var connection = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
            {
                connection.Open();
                string query = @"
                    INSERT INTO ImportLogs (ProviderId, ImportDate, RecordsImported, RecordsUpdated, Errors, Status)
                    VALUES (@ProviderId, @ImportDate, @RecordsImported, @RecordsUpdated, @Errors, @Status)";
                
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProviderId", providerId);
                    command.Parameters.AddWithValue("@ImportDate", DateTime.Now);
                    command.Parameters.AddWithValue("@RecordsImported", imported);
                    command.Parameters.AddWithValue("@RecordsUpdated", updated);
                    command.Parameters.AddWithValue("@Errors", errors);
                    command.Parameters.AddWithValue("@Status", status);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
