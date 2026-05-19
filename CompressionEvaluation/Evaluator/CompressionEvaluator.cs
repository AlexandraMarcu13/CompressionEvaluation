using CompressionEvaluation.CompressionAlgorithms;
using CompressionEvaluation.Models;
using CompressionEvaluation.Utils;
using System.Diagnostics;

namespace CompressionEvaluation.Evaluator
{
    public class CompressionEvaluator
    {
        private readonly string _inputDirectory;
        private readonly string _outputDirectory;

        public CompressionEvaluator(string inputDirectory, string outputDirectory)
        {
            _inputDirectory = inputDirectory;
            _outputDirectory = outputDirectory;
            Directory.CreateDirectory(_outputDirectory);
        }

        public CompressionResult EvaluateGZip(string inputFile)
        {
            var result = new CompressionResult
            {
                AlgorithmName = "GZip",
                InputFileName = Path.GetFileName(inputFile)
            };

            byte[] originalData = File.ReadAllBytes(inputFile);
            result.OriginalSize = originalData.Length;

            var stopwatch = Stopwatch.StartNew();
            byte[] compressedData = GZipCompression.Compress(originalData);
            stopwatch.Stop();
            result.CompressionTimeMs = stopwatch.Elapsed.TotalMilliseconds;
            result.CompressedSize = compressedData.Length;
            result.CompressionRatio = (double)result.OriginalSize / result.CompressedSize;

            string outputFile = Path.Combine(_outputDirectory,
                Path.GetFileNameWithoutExtension(inputFile) + "_gzip.bin");
            File.WriteAllBytes(outputFile, compressedData);

            stopwatch.Restart();
            byte[] decompressedData = GZipCompression.Decompress(compressedData);
            stopwatch.Stop();
            result.DecompressionTimeMs = stopwatch.Elapsed.TotalMilliseconds;

            if (!originalData.SequenceEqual(decompressedData))
                throw new Exception("GZip decompression verification failed!");

            return result;
        }

        public CompressionResult EvaluateBrotli(string inputFile)
        {
            var result = new CompressionResult
            {
                AlgorithmName = "Brotli",
                InputFileName = Path.GetFileName(inputFile)
            };

            byte[] originalData = File.ReadAllBytes(inputFile);
            result.OriginalSize = originalData.Length;

            var stopwatch = Stopwatch.StartNew();
            byte[] compressedData = BrotliCompression.Compress(originalData);
            stopwatch.Stop();
            result.CompressionTimeMs = stopwatch.Elapsed.TotalMilliseconds;
            result.CompressedSize = compressedData.Length;
            result.CompressionRatio = (double)result.OriginalSize / result.CompressedSize;

            string outputFile = Path.Combine(_outputDirectory,
                Path.GetFileNameWithoutExtension(inputFile) + "_brotli.br");
            File.WriteAllBytes(outputFile, compressedData);

            stopwatch.Restart();
            byte[] decompressedData = BrotliCompression.Decompress(compressedData);
            stopwatch.Stop();
            result.DecompressionTimeMs = stopwatch.Elapsed.TotalMilliseconds;

            if (!originalData.SequenceEqual(decompressedData))
                throw new Exception("Brotli decompression verification failed!");

            return result;
        }

        public CompressionResult EvaluateRLE(string inputFile)
        {
            var result = new CompressionResult
            {
                AlgorithmName = "RLE",
                InputFileName = Path.GetFileName(inputFile)
            };

            byte[] originalData = File.ReadAllBytes(inputFile);
            result.OriginalSize = originalData.Length;

            var stopwatch = Stopwatch.StartNew();
            byte[] compressedData = RLECompression.Compress(originalData);
            stopwatch.Stop();
            result.CompressionTimeMs = stopwatch.Elapsed.TotalMilliseconds;
            result.CompressedSize = compressedData.Length;
            result.CompressionRatio = (double)result.OriginalSize / result.CompressedSize;

            string outputFile = Path.Combine(_outputDirectory,
                Path.GetFileNameWithoutExtension(inputFile) + "_rle.rle");
            File.WriteAllBytes(outputFile, compressedData);

            stopwatch.Restart();
            byte[] decompressedData = RLECompression.Decompress(compressedData);
            stopwatch.Stop();
            result.DecompressionTimeMs = stopwatch.Elapsed.TotalMilliseconds;

            if (!originalData.SequenceEqual(decompressedData))
                throw new Exception("RLE decompression verification failed!");

            return result;
        }

        public CompressionResult EvaluateLZ4(string inputFile)
        {
            var result = new CompressionResult
            {
                AlgorithmName = "LZ4",
                InputFileName = Path.GetFileName(inputFile)
            };

            byte[] originalData = File.ReadAllBytes(inputFile);
            result.OriginalSize = originalData.Length;

            var stopwatch = Stopwatch.StartNew();
            byte[] compressedData = LZ4Compression.Compress(originalData);
            stopwatch.Stop();
            result.CompressionTimeMs = stopwatch.Elapsed.TotalMilliseconds;
            result.CompressedSize = compressedData.Length;
            result.CompressionRatio = (double)result.OriginalSize / result.CompressedSize;

            string outputFile = Path.Combine(_outputDirectory,
                Path.GetFileNameWithoutExtension(inputFile) + "_lz4.lz4");
            File.WriteAllBytes(outputFile, compressedData);

            stopwatch.Restart();
            byte[] decompressedData = LZ4Compression.Decompress(compressedData, originalData.Length);
            stopwatch.Stop();
            result.DecompressionTimeMs = stopwatch.Elapsed.TotalMilliseconds;

            if (!originalData.SequenceEqual(decompressedData))
                throw new Exception("LZ4 decompression verification failed!");

            return result;
        }

        public CompressionResult EvaluateZstd(string inputFile)
        {
            var result = new CompressionResult
            {
                AlgorithmName = "Zstd",
                InputFileName = Path.GetFileName(inputFile)
            };

            byte[] originalData = File.ReadAllBytes(inputFile);
            result.OriginalSize = originalData.Length;

            var stopwatch = Stopwatch.StartNew();
            byte[] compressedData = ZstdCompression.Compress(originalData);
            stopwatch.Stop();
            result.CompressionTimeMs = stopwatch.Elapsed.TotalMilliseconds;
            result.CompressedSize = compressedData.Length;
            result.CompressionRatio = (double)result.OriginalSize / result.CompressedSize;

            string outputFile = Path.Combine(_outputDirectory,
                Path.GetFileNameWithoutExtension(inputFile) + "_zstd.zst");
            File.WriteAllBytes(outputFile, compressedData);

            stopwatch.Restart();
            byte[] decompressedData = ZstdCompression.Decompress(compressedData);
            stopwatch.Stop();
            result.DecompressionTimeMs = stopwatch.Elapsed.TotalMilliseconds;

            if (!originalData.SequenceEqual(decompressedData))
                throw new Exception("Zstd decompression verification failed!");

            return result;
        }

        public Dictionary<string, CompressionResult> EvaluateAll(string inputFile)
        {
            var results = new Dictionary<string, CompressionResult>();

            Console.WriteLine($"\nEvaluating: {Path.GetFileName(inputFile)}");
            Console.WriteLine($"Original size: {FileHelper.FormatBytes(new FileInfo(inputFile).Length)}\n");

            Console.Write("Testing GZip... ");
            results["GZip"] = EvaluateGZip(inputFile);
            Console.WriteLine($" Ratio: {results["GZip"].CompressionRatio:F2}:1");

            Console.Write("Testing Brotli... ");
            results["Brotli"] = EvaluateBrotli(inputFile);
            Console.WriteLine($" Ratio: {results["Brotli"].CompressionRatio:F2}:1");

            Console.Write("Testing RLE... ");
            results["RLE"] = EvaluateRLE(inputFile);
            Console.WriteLine($" Ratio: {results["RLE"].CompressionRatio:F2}:1");

            Console.Write("Testing LZ4... ");
            results["LZ4"] = EvaluateLZ4(inputFile);
            Console.WriteLine($" Ratio: {results["LZ4"].CompressionRatio:F2}:1");

            Console.Write("Testing Zstd... ");
            results["Zstd"] = EvaluateZstd(inputFile);
            Console.WriteLine($" Ratio: {results["Zstd"].CompressionRatio:F2}:1");

            return results;
        }

        public void GenerateSummaryReport(List<Dictionary<string, CompressionResult>> allResults)
        {
            string reportPath = Path.Combine(_outputDirectory, "compression_report.txt");

            using var writer = new StreamWriter(reportPath);
            writer.WriteLine("                    COMPRESSION ALGORITHM EVALUATION REPORT                 ");
            writer.WriteLine($"Generated: {DateTime.Now}");
            writer.WriteLine($"Total files analyzed: {allResults.Count}");
            writer.WriteLine(new string('=', 90));

            var algorithmStats = new Dictionary<string, List<CompressionResult>>();

            foreach (var results in allResults)
            {
                foreach (var kvp in results)
                {
                    if (!algorithmStats.ContainsKey(kvp.Key))
                        algorithmStats[kvp.Key] = new List<CompressionResult>();
                    algorithmStats[kvp.Key].Add(kvp.Value);
                }
            }

            writer.WriteLine("\nPERFORMANCE SUMMARY:");
            writer.WriteLine(new string('-', 90));

            foreach (var algo in algorithmStats)
            {
                var avgRatio = algo.Value.Average(r => r.CompressionRatio);
                var avgSpaceSaving = algo.Value.Average(r => r.GetSpaceSavingPercentage());
                var avgCompTime = algo.Value.Average(r => r.CompressionTimeMs);
                var avgDecompTime = algo.Value.Average(r => r.DecompressionTimeMs);

                writer.WriteLine($"\n {algo.Key}:");
                writer.WriteLine($"    Average Compression Ratio: {avgRatio:F2}:1");
                writer.WriteLine($"    Average Space Saving: {avgSpaceSaving:F1}%");
                writer.WriteLine($"    Average Compression Time: {avgCompTime:F2}ms");
                writer.WriteLine($"    Average Decompression Time: {avgDecompTime:F2}ms");
            }

            writer.WriteLine("\nDETAILED RESULTS:");
            writer.WriteLine(new string('-', 90));

            foreach (var results in allResults)
            {
                var firstResult = results.First().Value;
                writer.WriteLine($"\nFile: {firstResult.InputFileName}");
                writer.WriteLine($"   Original Size: {firstResult.GetFormattedOriginalSize()}");

                foreach (var result in results.Values)
                {
                    writer.WriteLine($"   {result.AlgorithmName,-8} → {result.GetFormattedCompressedSize(),-10} " +
                                   $"(Ratio: {result.CompressionRatio:F2}:1, Saving: {result.GetSpaceSavingPercentage():F1}%)");
                }
            }

            Console.WriteLine($"\nText report saved to: {reportPath}");
        }

        public void GenerateHtmlReport(List<Dictionary<string, CompressionResult>> allResults)
        {
            string reportPath = Path.Combine(_outputDirectory, "CompressionReport.html");
            ReportGenerator.GenerateHtmlReport(allResults, reportPath);
            Console.WriteLine($"\nHTML Report generated: {reportPath}");

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = reportPath,
                    UseShellExecute = true
                });
                Console.WriteLine("Report opened in your default browser!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not auto-open browser: {ex.Message}");
                Console.WriteLine($"Please open manually: {reportPath}");
            }
        }
    }
}