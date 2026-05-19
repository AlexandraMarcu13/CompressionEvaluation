using CompressionEvaluation.Models;
using CompressionEvaluation.Utils;
using CompressionEvaluation.Evaluator;
namespace CompressionEvaluation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("                                                            ");
            Console.WriteLine("         COMPRESSION ALGORITHM EVALUATOR                    ");
            Console.WriteLine("                                                            ");

            // Setup directories
            string inputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles", "Input");
            string outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles", "Output");

            Console.WriteLine($"\n Input directory: {inputDir}");
            Console.WriteLine($" Output directory: {outputDir}");

            // Create directories if they don't exist
            Directory.CreateDirectory(inputDir);
            Directory.CreateDirectory(outputDir);

            // Ask if user wants to create sample files
            Console.WriteLine("\n Do you want to create sample test files? (Y/N):");
            string response = Console.ReadLine();

            if (response?.Trim().ToUpper() == "Y")
            {
                FileHelper.CreateSampleTestFiles(inputDir, forceOverwrite: false);
            }
            else
            {
                Console.WriteLine("\n Using existing files in input directory...");
                Console.WriteLine(" Tip: You can manually add any files to the input folder!");
            }

            FileHelper.DisplayInputFiles(inputDir);

            // Initialize evaluator
            var evaluator = new CompressionEvaluator(inputDir, outputDir);

            // Get all files to test
            string[] testFiles = FileHelper.GetTestFiles(inputDir);

            if (testFiles.Length == 0)
            {
                Console.WriteLine("\n No test files found in input directory!");
                Console.WriteLine("\n TIP: You can:");
                Console.WriteLine("   1. Answer 'Y' to create sample files, or");
                Console.WriteLine("   2. Manually copy files to: " + inputDir);
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nStarting compression evaluation on {testFiles.Length} file(s)...");
            Console.WriteLine(new string('=', 80));

            // Results collection
            var allResults = new List<Dictionary<string, CompressionResult>>();

            // Evaluate each file
            int fileNumber = 1;
            foreach (string file in testFiles)
            {
                Console.WriteLine($"\n[{fileNumber}/{testFiles.Length}] Processing...");
                try
                {
                    var results = evaluator.EvaluateAll(file);
                    allResults.Add(results);

                    Console.WriteLine("\n" + new string('─', 100));
                    Console.WriteLine($" RESULTS FOR {Path.GetFileName(file)}:");
                    Console.WriteLine(new string('─', 100));

                    foreach (var result in results.Values)
                    {
                        Console.WriteLine(result.ToString());
                    }

                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n Error processing {Path.GetFileName(file)}: {ex.Message}");
                }
                fileNumber++;
            }

            // Generate reports
            if (allResults.Count > 0)
            {
                Console.WriteLine(new string('=', 80));
                Console.WriteLine("\n Generating Reports...");

                evaluator.GenerateSummaryReport(allResults);

                evaluator.GenerateHtmlReport(allResults);

            }

            Console.WriteLine("\n Evaluation complete!");
            Console.WriteLine($" Check output directory for all reports and compressed files:");
            Console.WriteLine($"   {outputDir}");
            Console.WriteLine("\n Reports generated:");
            Console.WriteLine($"    compression_report.txt - Text summary");
            Console.WriteLine($"    CompressionReport.html - Visual report (opens in browser)");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}