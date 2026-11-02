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

            // ---- Sequential ----
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

            // ---- Parallel ----
            stopwatch.Restart();
            byte[] compressedParallel = GZipCompression.CompressParallel(originalData);
            stopwatch.Stop();
            result.CompressionTimeMsParallel = stopwatch.Elapsed.TotalMilliseconds;
            result.CompressedSizeParallel = compressedParallel.Length;
            result.CompressionRatioParallel = (double)result.OriginalSize / result.CompressedSizeParallel;

            string outputFilePar = Path.Combine(_outputDirectory,
                Path.GetFileNameWithoutExtension(inputFile) + "_gzip_parallel.bin");
            File.WriteAllBytes(outputFilePar, compressedParallel);

            stopwatch.Restart();
            byte[] decompressedParallel = GZipCompression.DecompressParallel(compressedParallel);
            stopwatch.Stop();
            result.DecompressionTimeMsParallel = stopwatch.Elapsed.TotalMilliseconds;

            if (!originalData.SequenceEqual(decompressedParallel))
                throw new Exception("GZip parallel decompression verification failed!");

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

            // ---- Sequential ----
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

            // ---- Parallel ----
            stopwatch.Restart();
            byte[] compressedParallel = BrotliCompression.CompressParallel(originalData);
            stopwatch.Stop();
            result.CompressionTimeMsParallel = stopwatch.Elapsed.TotalMilliseconds;
            result.CompressedSizeParallel = compressedParallel.Length;
            result.CompressionRatioParallel = (double)result.OriginalSize / result.CompressedSizeParallel;

            string outputFilePar = Path.Combine(_outputDirectory,
                Path.GetFileNameWithoutExtension(inputFile) + "_brotli_parallel.br");
            File.WriteAllBytes(outputFilePar, compressedParallel);

            stopwatch.Restart();
            byte[] decompressedParallel = BrotliCompression.DecompressParallel(compressedParallel);
            stopwatch.Stop();
            result.DecompressionTimeMsParallel = stopwatch.Elapsed.TotalMilliseconds;

            if (!originalData.SequenceEqual(decompressedParallel))
                throw new Exception("Brotli parallel decompression verification failed!");

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

            // ---- Sequential ----
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

            // ---- Parallel ----
            stopwatch.Restart();
            byte[] compressedParallel = RLECompression.CompressParallel(originalData);
            stopwatch.Stop();
            result.CompressionTimeMsParallel = stopwatch.Elapsed.TotalMilliseconds;
            result.CompressedSizeParallel = compressedParallel.Length;
            result.CompressionRatioParallel = (double)result.OriginalSize / result.CompressedSizeParallel;

            string outputFilePar = Path.Combine(_outputDirectory,
                Path.GetFileNameWithoutExtension(inputFile) + "_rle_parallel.rle");
            File.WriteAllBytes(outputFilePar, compressedParallel);

            stopwatch.Restart();
            byte[] decompressedParallel = RLECompression.DecompressParallel(compressedParallel);
            stopwatch.Stop();
            result.DecompressionTimeMsParallel = stopwatch.Elapsed.TotalMilliseconds;

            if (!originalData.SequenceEqual(decompressedParallel))
                throw new Exception("RLE parallel decompression verification failed!");

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

            // ---- Sequential ----
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

            // ---- Parallel ----
            stopwatch.Restart();
            byte[] compressedParallel = LZ4Compression.CompressParallel(originalData);
            stopwatch.Stop();
            result.CompressionTimeMsParallel = stopwatch.Elapsed.TotalMilliseconds;
            result.CompressedSizeParallel = compressedParallel.Length;
            result.CompressionRatioParallel = (double)result.OriginalSize / result.CompressedSizeParallel;

            string outputFilePar = Path.Combine(_outputDirectory,
                Path.GetFileNameWithoutExtension(inputFile) + "_lz4_parallel.lz4");
            File.WriteAllBytes(outputFilePar, compressedParallel);

            stopwatch.Restart();
            byte[] decompressedParallel = LZ4Compression.DecompressParallel(compressedParallel);
            stopwatch.Stop();
            result.DecompressionTimeMsParallel = stopwatch.Elapsed.TotalMilliseconds;

            if (!originalData.SequenceEqual(decompressedParallel))
                throw new Exception("LZ4 parallel decompression verification failed!");

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

            // ---- Sequential ----
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

            // ---- Parallel ----
            stopwatch.Restart();
            byte[] compressedParallel = ZstdCompression.CompressParallel(originalData);
            stopwatch.Stop();
            result.CompressionTimeMsParallel = stopwatch.Elapsed.TotalMilliseconds;
            result.CompressedSizeParallel = compressedParallel.Length;
            result.CompressionRatioParallel = (double)result.OriginalSize / result.CompressedSizeParallel;

            string outputFilePar = Path.Combine(_outputDirectory,
                Path.GetFileNameWithoutExtension(inputFile) + "_zstd_parallel.zst");
            File.WriteAllBytes(outputFilePar, compressedParallel);

            stopwatch.Restart();
            byte[] decompressedParallel = ZstdCompression.DecompressParallel(compressedParallel);
            stopwatch.Stop();
            result.DecompressionTimeMsParallel = stopwatch.Elapsed.TotalMilliseconds;

            if (!originalData.SequenceEqual(decompressedParallel))
                throw new Exception("Zstd parallel decompression verification failed!");

            return result;
        }

        public Dictionary<string, CompressionResult> EvaluateAll(string inputFile)
        {
            var results = new Dictionary<string, CompressionResult>();

            Console.WriteLine($"\nEvaluating: {Path.GetFileName(inputFile)}");
            Console.WriteLine($"Original size: {FileHelper.FormatBytes(new FileInfo(inputFile).Length)}\n");

            Console.Write("Testing GZip (seq + parallel)... ");
            results["GZip"] = EvaluateGZip(inputFile);
            Console.WriteLine($" Ratio seq: {results["GZip"].CompressionRatio:F2}:1 | par: {results["GZip"].CompressionRatioParallel:F2}:1");

            Console.Write("Testing Brotli (seq + parallel)... ");
            results["Brotli"] = EvaluateBrotli(inputFile);
            Console.WriteLine($" Ratio seq: {results["Brotli"].CompressionRatio:F2}:1 | par: {results["Brotli"].CompressionRatioParallel:F2}:1");

            Console.Write("Testing RLE (seq + parallel)... ");
            results["RLE"] = EvaluateRLE(inputFile);
            Console.WriteLine($" Ratio seq: {results["RLE"].CompressionRatio:F2}:1 | par: {results["RLE"].CompressionRatioParallel:F2}:1");

            Console.Write("Testing LZ4 (seq + parallel)... ");
            results["LZ4"] = EvaluateLZ4(inputFile);
            Console.WriteLine($" Ratio seq: {results["LZ4"].CompressionRatio:F2}:1 | par: {results["LZ4"].CompressionRatioParallel:F2}:1");

            Console.Write("Testing Zstd (seq + parallel)... ");
            results["Zstd"] = EvaluateZstd(inputFile);
            Console.WriteLine($" Ratio seq: {results["Zstd"].CompressionRatio:F2}:1 | par: {results["Zstd"].CompressionRatioParallel:F2}:1");

            return results;
        }

        public void GenerateSummaryReport(List<Dictionary<string, CompressionResult>> allResults)
        {
            string reportPath = Path.Combine(_outputDirectory, "compression_report.txt");

            using var writer = new StreamWriter(reportPath);
            writer.WriteLine("                COMPRESSION ALGORITHM EVALUATION REPORT (SEQ vs PARALLEL)         ");
            writer.WriteLine($"Generated: {DateTime.Now}");
            writer.WriteLine($"Total files analyzed: {allResults.Count}");
            writer.WriteLine(new string('=', 110));

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

            writer.WriteLine("\nPERFORMANCE SUMMARY (Sequential vs Parallel):");
            writer.WriteLine(new string('-', 110));

            foreach (var algo in algorithmStats)
            {
                var avgRatio = algo.Value.Average(r => r.CompressionRatio);
                var avgRatioPar = algo.Value.Average(r => r.CompressionRatioParallel);
                var avgCompTime = algo.Value.Average(r => r.CompressionTimeMs);
                var avgCompTimePar = algo.Value.Average(r => r.CompressionTimeMsParallel);
                var avgDecompTime = algo.Value.Average(r => r.DecompressionTimeMs);
                var avgDecompTimePar = algo.Value.Average(r => r.DecompressionTimeMsParallel);

                writer.WriteLine($"\n {algo.Key}:");
                writer.WriteLine($"    Avg Compression Ratio:   seq {avgRatio:F2}:1   |  par {avgRatioPar:F2}:1");
                writer.WriteLine($"    Avg Compression Time:    seq {avgCompTime:F2}ms |  par {avgCompTimePar:F2}ms");
                writer.WriteLine($"    Avg Decompression Time:  seq {avgDecompTime:F2}ms |  par {avgDecompTimePar:F2}ms");
            }

            writer.WriteLine("\nDETAILED RESULTS:");
            writer.WriteLine(new string('-', 110));

            foreach (var results in allResults)
            {
                var firstResult = results.First().Value;
                writer.WriteLine($"\nFile: {firstResult.InputFileName}");
                writer.WriteLine($"   Original Size: {firstResult.GetFormattedOriginalSize()}");

                foreach (var result in results.Values)
                {
                    writer.WriteLine($"   {result.AlgorithmName,-8} seq: {result.GetFormattedCompressedSize(),-10} " +
                                   $"({result.CompressionTimeMs:F2}ms) | par: {result.GetFormattedCompressedSizeParallel(),-10} " +
                                   $"({result.CompressionTimeMsParallel:F2}ms)");
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