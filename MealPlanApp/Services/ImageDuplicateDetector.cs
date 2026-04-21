using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MealPlanApp.Models;
using MealPlanApp.Data;

namespace MealPlanApp.Services
{
    /// <summary>
    /// Cerința 3: Script care identifică anunțurile identice pe baza imaginilor
    /// Folosește hash SHA256 pentru a detecta duplicatele
    /// </summary>
    public class ImageDuplicateDetector
    {
        /// <summary>
        /// Calculează hash-ul unei imagini (simulat prin hash pe URL sau conținut)
        /// În producție, s-ar citi fișierul imagine și s-ar calcula hash-ul real
        /// </summary>
        public string CalculateImageHash(string imageUrl, byte[] imageBytes = null)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes;
                
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    // Hash din conținutul real al imaginii
                    hashBytes = sha256.ComputeHash(imageBytes);
                }
                else
                {
                    // Hash din URL (pentru demonstrație)
                    hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(imageUrl ?? string.Empty));
                }
                
                // Convertim în hex string
                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Detectează duplicate în baza de date pe baza hash-ului imaginii
        /// </summary>
        public List<(Food Original, Food Duplicate)> FindDuplicates()
        {
            Console.WriteLine("=== Detectare Duplicate pe Baza Imaginilor ===");
            
            var allFoods = DatabaseHelper.GetAllFoods();
            var duplicates = new List<(Food, Food)>();
            var hashDictionary = new Dictionary<string, Food>();
            
            foreach (var food in allFoods)
            {
                // Calculăm hash dacă nu există deja
                if (string.IsNullOrEmpty(food.ImageHash))
                {
                    food.ImageHash = CalculateImageHash(food.ImageUrl);
                }
                
                if (hashDictionary.ContainsKey(food.ImageHash))
                {
                    var original = hashDictionary[food.ImageHash];
                    duplicates.Add((original, food));
                    Console.WriteLine($"  ⚠️  DUPLICAT DETECTAT:");
                    Console.WriteLine($"      Original: {original.Name} (ID: {original.Id})");
                    Console.WriteLine($"      Duplicat: {food.Name} (ID: {food.Id})");
                    Console.WriteLine($"      Hash: {food.ImageHash.Substring(0, 16)}...");
                }
                else
                {
                    hashDictionary[food.ImageHash] = food;
                }
            }
            
            Console.WriteLine($"\nTotal duplicate găsite: {duplicates.Count}");
            return duplicates;
        }

        /// <summary>
        /// Elimină duplicatele din baza de date (păstrează primul)
        /// </summary>
        public int RemoveDuplicates()
        {
            var duplicates = FindDuplicates();
            int removedCount = 0;
            
            Console.WriteLine("\n=== Eliminare Duplicate ===");
            
            // În implementarea reală, am șterge din DB
            // Aici doar simulăm procesul
            foreach (var (original, duplicate) in duplicates)
            {
                Console.WriteLine($"  - Se marchează spre ștergere: {duplicate.Name} (ID: {duplicate.Id})");
                removedCount++;
            }
            
            Console.WriteLine($"\nTotal elemente marcate pentru ștergere: {removedCount}");
            return removedCount;
        }

        /// <summary>
        /// Studiu de caz - Cerința 4: Modalități de stocare a imaginilor
        /// </summary>
        public static void DisplayImageStorageStudy()
        {
            Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════════════════════╗
║     STUDIU DE CAZ: Stocarea Eficientă a Imaginilor                           ║
╠══════════════════════════════════════════════════════════════════════════════╣
║                                                                              ║
║  1. STOCARE ÎN BAZA DE DATE (BLOB)                                           ║
║     ├── Avantaje:                                                            ║
║     │   • Backup unitar (DB + imagini împreună)                              ║
║     │   • Tranzacționalitate garantată                                       ║
║     │   • Control centralizat al accesului                                   ║
║     ├── Dezavantaje:                                                         ║
║     │   • Crește semnificativ dimensiunea DB                                 ║
║     │   • Performanță scăzută la citire/scriere                              ║
║     │   • Dificil de scalat                                                  ║
║     └── Recomandare: Doar pentru imagini mici (< 100KB)                      ║
║                                                                              ║
║  2. STOCARE PE SERVER (File System)                                          ║
║     ├── Avantaje:                                                            ║
║     │   • Acces rapid la fișiere                                             ║
║     │   • Ușor de implementat                                                ║
║     │   • Cost redus                                                         ║
║     ├── Dezavantaje:                                                         ║
║     │   • Backup separat necesar                                             ║
║     │   • Dificil de scalat orizontal                                        ║
║     │   • Dependență de serverul fizic                                       ║
║     └── Recomandare: Pentru aplicații mici/medii on-premise                  ║
║                                                                              ║
║  3. STOCARE CLOUD (Azure Blob Storage / AWS S3) - RECOMANDAT                 ║
║     ├── Avantaje:                                                            ║
║     │   • Scalabilitate infinită                                             ║
║     │   • Cost optimizat (pay-per-use)                                       ║
║     │   • CDN integrat pentru livrare rapidă                                 ║
║     │   • Redundanță și backup automat                                       ║
║     │   • Separarea preocupărilor (app vs storage)                           ║
║     ├── Dezavantaje:                                                         ║
║     │   • Costuri la transfer de date                                        ║
║     │   • Dependență de provider cloud                                       ║
║     └── Recomandare: SOLUȚIA OPTIMĂ pentru majoritatea cazurilor             ║
║                                                                              ║
║  4. ARHITECTURA RECOMANDATĂ PENTRU ACEST PROIECT:                            ║
║     • În DB: Se stochează DOAR metadata (URL, hash, dimensiune, tip)         ║
║     • În Cloud: Se stochează fișierele immagine reale                        ║
║     • Se folosește CDN pentru cache și livrare rapidă                        ║
║     • Hash-ul imaginii se calculează la upload pentru deduplicare            ║
║                                                                              ║
╚══════════════════════════════════════════════════════════════════════════════╝
");
        }
    }
}
