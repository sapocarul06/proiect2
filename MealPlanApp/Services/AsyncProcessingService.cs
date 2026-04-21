using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MealPlanApp.Models;
using MealPlanApp.Data;

namespace MealPlanApp.Services
{
    /// <summary>
    /// Cerința 9: Integrare de serviciu asincron specific temei
    /// Cerința 11: Statistici consumatoare de timp
    /// </summary>
    public class AsyncProcessingService
    {
        /// <summary>
        /// Cerința 9: Procesare asincronă în background pentru analiză nutrițională
        /// </summary>
        public async Task ProcessNutritionalAnalysisAsync(List<Food> foods)
        {
            Console.WriteLine("=== Procesare Asincronă - Analiză Nutrițională ===");
            
            var tasks = new List<Task<FoodAnalysisResult>>();
            
            foreach (var food in foods)
            {
                tasks.Add(AnalyzeFoodAsync(food));
            }
            
            var results = await Task.WhenAll(tasks);
            
            Console.WriteLine($"\n✓ Analiză completată pentru {results.Length} alimente");
            
            // Afișăm rezultatele
            foreach (var result in results.Take(5))
            {
                Console.WriteLine($"  • {result.FoodName}: Scor nutrițional {result.NutritionalScore}/100");
            }
        }

        private async Task<FoodAnalysisResult> AnalyzeFoodAsync(Food food)
        {
            // Simulăm o operație intensivă de calcul (ex: API extern, ML model)
            await Task.Delay(200); // Simulare latență
            
            // Calculăm un scor nutrițional simplificat
            var score = CalculateNutritionalScore(food);
            
            return new FoodAnalysisResult
            {
                FoodName = food.Name,
                NutritionalScore = score,
                ProcessingTimeMs = 200
            };
        }

        private int CalculateNutritionalScore(Food food)
        {
            // Algoritm simplificat de scoring
            var proteinScore = Math.Min(food.Protein * 2, 40);
            var caloriePenalty = Math.Max(0, (food.Calories - 500) / 10);
            var fatPenalty = Math.Max(0, (food.Fat - 20) / 2);
            
            var score = 100 + proteinScore - caloriePenalty - fatPenalty;
            return Math.Max(0, Math.Min(100, (int)score));
        }

        /// <summary>
        /// Cerința 11: Statistici consumatoare de timp - măsoară performanța operațiilor
        /// </summary>
        public void RunPerformanceStatistics()
        {
            Console.WriteLine("=== Statistici Performanță - Operații Consumatoare de Timp ===\n");
            
            var stopwatch = new Stopwatch();
            var results = new Dictionary<string, TimeSpan>();
            
            // Test 1: Timp de citire din bază de date
            stopwatch.Restart();
            var foods = DatabaseHelper.GetAllFoods();
            stopwatch.Stop();
            results["Citire DB (toate alimentele)"] = stopwatch.Elapsed;
            Console.WriteLine($"⏱️  Citire DB ({foods.Count} înregistrări): {stopwatch.ElapsedMilliseconds}ms");
            
            // Test 2: Timp de procesare pentru filtrare complexă
            stopwatch.Restart();
            var filtered = foods.Where(f => 
                f.Calories > 200 && 
                f.Calories < 600 && 
                f.Protein > 15 &&
                f.Carbs < 50
            ).ToList();
            stopwatch.Stop();
            results["Filtrare complexă"] = stopwatch.Elapsed;
            Console.WriteLine($"⏱️  Filtrare complexă ({filtered.Count} rezultate): {stopwatch.ElapsedMilliseconds}ms");
            
            // Test 3: Timp de sortare și grupare
            stopwatch.Restart();
            var grouped = foods.GroupBy(f => f.Provider)
                               .Select(g => new { Provider = g.Key, Count = g.Count(), AvgCalories = g.Average(x => x.Calories) })
                               .OrderByDescending(x => x.AvgCalories)
                               .ToList();
            stopwatch.Stop();
            results["Grupare și sortare"] = stopwatch.Elapsed;
            Console.WriteLine($"⏱️  Grupare și sortare ({grouped.Count} grupuri): {stopwatch.ElapsedMilliseconds}ms");
            
            // Test 4: Timp de procesare asincronă
            stopwatch.Restart();
            ProcessNutritionalAnalysisAsync(foods.Take(10).ToList()).Wait();
            stopwatch.Stop();
            results["Procesare asincronă (10 alimente)"] = stopwatch.Elapsed;
            Console.WriteLine($"⏱️  Procesare asincronă: {stopwatch.ElapsedMilliseconds}ms");
            
            // Rezumat
            Console.WriteLine("\n📊 REZUMAT PERFORMANȚĂ:");
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            foreach (var result in results)
            {
                var percentage = (result.Value.TotalMilliseconds / results.Values.Sum(v => v.TotalMilliseconds)) * 100;
                Console.WriteLine($"║  {result.Key,-35} {result.Value.TotalMilliseconds,6:F0}ms ({percentage,5:F1}%) ║");
            }
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            
            // Recomandări
            Console.WriteLine("\n💡 RECOMANDĂRI OPTIMIZARE:");
            var slowestOp = results.OrderByDescending(r => r.Value.TotalMilliseconds).First();
            if (slowestOp.Value.TotalMilliseconds > 1000)
            {
                Console.WriteLine($"   ⚠️  Operația '{slowestOp.Key}' depășește 1 secundă!");
                Console.WriteLine($"      → Considerați adăugarea de indici în baza de date");
                Console.WriteLine($"      → Implementați caching pentru rezultatele frecvente");
                Console.WriteLine($"      → Folosiți procesare asincronă în background");
            }
            else
            {
                Console.WriteLine("   ✓ Toate operațiile se execută în parametri normali");
            }
        }

        /// <summary>
        /// Studiu de caz - Cerința 8: Comparatie stocare local vs server/serverless
        /// </summary>
        public static void DisplayHostingStudy()
        {
            Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════════════════════╗
║     STUDIU DE CAZ: Găzduirea Aplicației - Local vs Server vs Serverless      ║
╠══════════════════════════════════════════════════════════════════════════════╣
║                                                                              ║
║  1. STOCARE/GĂZDUIRE LOCALĂ (On-Premises)                                    ║
║     ├── Avantaje:                                                            ║
║     │   • Control total asupra infrastructurii                               ║
║     │   • Fără costuri recurente de cloud                                    ║
║     │   • Latență minimă în rețeaua locală                                   ║
║     │   • Conformitate cu reglementări stricte de date                       ║
║     ├── Dezavantaje:                                                         ║
║     │   • Costuri inițiale mari (hardware, licențe)                          ║
║     │   • Necesită echipă IT dedicată                                        ║
║     │   • Scalabilitate limitată                                             ║
║     │   • Risc de downtime fără redundanță adecvată                          ║
║     └── Cost estimat: €5,000-20,000 inițial + €500-2000/lună mentenanță      ║
║                                                                              ║
║  2. SERVER VPS/Dedicat (DigitalOcean, Hetzner, OVH)                          ║
║     ├── Avantaje:                                                            ║
║     │   • Cost predictibil lunar                                             ║
║     │   • Control bun asupra configurației                                   ║
║     │   • Scalabilitate verticală posibilă                                   ║
║     │   • Ușor de implementat                                                ║
║     ├── Dezavantaje:                                                         ║
║     │   • Necesită administrare sistem                                       ║
║     │   • Scalabilitate orizontală manuală                                   ║
║     │   • Backup și securitate în responsabilitatea ta                       ║
║     └── Cost estimat: €10-100/lună pentru majoritatea aplicațiilor           ║
║                                                                              ║
║  3. CLOUD TRADIȚIONAL (Azure VM, AWS EC2)                                    ║
║     ├── Avantaje:                                                            ║
║     │   • Scalabilitate elastică                                             ║
║     │   • Servicii managed disponibile (DB, cache, etc.)                     ║
║     │   • Redundanță și backup automat                                       ║
║     │   • Integrare cu alte servicii cloud                                   ║
║     ├── Dezavantaje:                                                         ║
║     │   • Costuri pot crește necontrolat                                     ║
║     │   • Complexitate mai mare                                              ║
║     │   • Vendor lock-in potențial                                           ║
║     └── Cost estimat: €50-500/lună (depinde de utilizare)                    ║
║                                                                              ║
║  4. SERVERLESS (Azure Functions, AWS Lambda) - RECOMANDAT PENTRU STARTUP     ║
║     ├── Avantaje:                                                            ║
║     │   • Pay-per-execution (plătești doar când se folosește)                ║
║     │   • Scalabilitate automată și infinită                                 ║
║     │   • Fără administrare de servere                                       ║
║     │   • Time-to-market rapid                                               ║
║     │   • Ideal pentru workload-uri intermitente                             ║
║     ├── Dezavantaje:                                                         ║
║     │   • Cold start latency (întârziere la prima rulare)                    ║
║     │   • Limitări de timp de execuție (max 5-15 min)                        ║
║     │   • Debugging mai complex                                              ║
║     │   • Costuri mari la volum foarte mare                                  ║
║     └── Cost estimat: €0-100/lună pentru volum mediu                         ║
║                                                                              ║
║  5. RECOMANDARE PENTRU ACEST PROIECT:                                        ║
║     • FAZA 1 (Dezvoltare/Testare): Azure Functions (serverless)              ║
║       - Cost minim, scalabilitate automată, focus pe cod                     ║
║     • FAZA 2 (Producție - volum mic): VPS (DigitalOcean/Hetzner)             ║
║       - Cost predictibil, control bun, ~€20/lună                             ║
║     • FAZA 3 (Scalare): Azure App Service + Azure SQL                        ║
║       - When business justifică costurile suplimentare                       ║
║                                                                              ║
║  ARHITECTURA OPTIMĂ PROPUSĂ:                                                 ║
║  ┌─────────────────┐    ┌──────────────┐    ┌──────────────────┐            ║
║  │  Frontend/API   │───▶│  Azure Func. │───▶│  Azure SQL/Cosmos│            ║
║  │  (Static Web)   │    │  (Logic)     │    │  (Date)          │            ║
║  └─────────────────┘    └──────────────┘    └──────────────────┘            ║
║         │                      │                      │                      ║
║         ▼                      ▼                      ▼                      ║
║  ┌─────────────────┐    ┌──────────────┐    ┌──────────────────┐            ║
║  │  Azure CDN      │    │  Queue Storage│   │  Blob Storage    │            ║
║  │  (Imagini)      │    │  (Async Ops) │   │  (Imagini/Fișiere)│            ║
║  └─────────────────┘    └──────────────┘    └──────────────────┘            ║
║                                                                              ║
╚══════════════════════════════════════════════════════════════════════════════╝
");
        }
    }

    public class FoodAnalysisResult
    {
        public string FoodName { get; set; }
        public int NutritionalScore { get; set; }
        public long ProcessingTimeMs { get; set; }
    }
}
