using CompressionEvaluation.Models;
using System.Text;

namespace CompressionEvaluation.Utils
{
    public static class ReportGenerator
    {
        public static void GenerateHtmlReport(List<Dictionary<string, CompressionResult>> allResults, string outputPath)
        {
            string html = BuildHtmlReport(allResults);
            File.WriteAllText(outputPath, html);
        }

        private static string BuildHtmlReport(List<Dictionary<string, CompressionResult>> allResults)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='en'>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset='UTF-8'>");
            sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            sb.AppendLine("    <title>Compression Algorithm Report (Sequential vs Parallel)</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        * { margin: 0; padding: 0; box-sizing: border-box; }");
            sb.AppendLine("        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 20px; }");
            sb.AppendLine("        .container { max-width: 1500px; margin: 0 auto; background: white; border-radius: 15px; box-shadow: 0 20px 60px rgba(0,0,0,0.3); overflow: hidden; }");
            sb.AppendLine("        .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; }");
            sb.AppendLine("        .header h1 { font-size: 2.5em; margin-bottom: 10px; }");
            sb.AppendLine("        .header p { font-size: 1.1em; opacity: 0.9; }");
            sb.AppendLine("        .content { padding: 30px; }");
            sb.AppendLine("        .summary-cards { display: grid; grid-template-columns: repeat(auto-fit, minmax(280px, 1fr)); gap: 20px; margin-bottom: 40px; }");
            sb.AppendLine("        .card { background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%); padding: 20px; border-radius: 10px; text-align: center; transition: transform 0.3s; }");
            sb.AppendLine("        .card:hover { transform: translateY(-5px); }");
            sb.AppendLine("        .card h3 { color: #333; margin-bottom: 10px; }");
            sb.AppendLine("        .card .value { font-size: 1.6em; font-weight: bold; color: #667eea; }");
            sb.AppendLine("        .card .label { color: #666; margin-top: 5px; font-size: 0.9em; }");
            sb.AppendLine("        .card .speedup-pos { color: #2e7d32; font-weight: bold; }");
            sb.AppendLine("        .card .speedup-neg { color: #c62828; font-weight: bold; }");
            sb.AppendLine("        .table-wrapper { max-height: 700px; overflow-y: auto; border: 1px solid #ddd; border-radius: 10px; margin-bottom: 30px; }");
            sb.AppendLine("        table { width: 100%; border-collapse: collapse; font-size: 13px; }");
            sb.AppendLine("        th { background: #667eea; color: white; padding: 10px; text-align: left; position: sticky; top: 0; z-index: 10; }");
            sb.AppendLine("        td { padding: 8px; border-bottom: 1px solid #ddd; }");
            sb.AppendLine("        tr:hover { background: #f5f5f5; }");
            sb.AppendLine("        .best { background: #d4edda; font-weight: bold; }");
            sb.AppendLine("        .best-par { background: #cce5ff; font-weight: bold; }");
            sb.AppendLine("        .progress-bar { background: #e0e0e0; border-radius: 10px; overflow: hidden; height: 20px; }");
            sb.AppendLine("        .progress-fill { height: 100%; transition: width 0.3s; }");
            sb.AppendLine("        .chart-container { margin: 30px 0; padding: 20px; background: #f9f9f9; border-radius: 10px; }");
            sb.AppendLine("        .chart-title { font-size: 1.3em; margin-bottom: 15px; color: #333; }");
            sb.AppendLine("        .bar-item { margin: 8px 0; }");
            sb.AppendLine("        .bar-label { display: inline-block; width: 180px; font-weight: bold; font-size: 0.9em; }");
            sb.AppendLine("        .footer { background: #333; color: white; padding: 20px; text-align: center; }");
            sb.AppendLine("        @media (max-width: 768px) { .summary-cards { grid-template-columns: 1fr; } table { font-size: 11px; } .bar-label { width: 110px; } }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class='container'>");
            sb.AppendLine("        <div class='header'>");
            sb.AppendLine("            <h1>📊 Compression Evaluation: Sequential vs Parallel</h1>");
            sb.AppendLine($"            <p>Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
            sb.AppendLine($"            <p>Files Tested: {allResults.Count} | Cores: {Environment.ProcessorCount}</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='content'>");

            AddSummaryCards(sb, allResults);
            AddDetailedTable(sb, allResults);
            AddPerformanceCharts(sb, allResults);
            AddRecommendations(sb);

            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='footer'>");
            sb.AppendLine("            <p>Compression Algorithm Evaluator | 5 Algorithms: GZip, Brotli, RLE, LZ4, Zstd (Sequential + Parallel)</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private static void AddSummaryCards(StringBuilder sb, List<Dictionary<string, CompressionResult>> allResults)
        {
            var stats = new Dictionary<string, List<CompressionResult>>();

            foreach (var results in allResults)
            {
                foreach (var kvp in results)
                {
                    if (!stats.ContainsKey(kvp.Key))
                        stats[kvp.Key] = new List<CompressionResult>();
                    stats[kvp.Key].Add(kvp.Value);
                }
            }

            sb.AppendLine("        <div class='summary-cards'>");

            foreach (var algo in stats)
            {
                double avgRatio = algo.Value.Average(r => r.CompressionRatio);
                double avgRatioPar = algo.Value.Average(r => r.CompressionRatioParallel);
                double avgCompTime = algo.Value.Average(r => r.CompressionTimeMs);
                double avgCompTimePar = algo.Value.Average(r => r.CompressionTimeMsParallel);
                double avgDecompTime = algo.Value.Average(r => r.DecompressionTimeMs);
                double avgDecompTimePar = algo.Value.Average(r => r.DecompressionTimeMsParallel);

                double compSpeedup = avgCompTimePar > 0 ? avgCompTime / avgCompTimePar : 0;
                double decompSpeedup = avgDecompTimePar > 0 ? avgDecompTime / avgDecompTimePar : 0;

                sb.AppendLine("            <div class='card'>");
                sb.AppendLine($"                <h3>{algo.Key}</h3>");
                sb.AppendLine($"                <div class='value'>{avgRatio:F1}:1 <span style='font-size:0.6em;color:#999;'>seq</span></div>");
                sb.AppendLine($"                <div class='label'>Compression Ratio</div>");
                sb.AppendLine($"                <div class='value'>{avgRatioPar:F1}:1 <span style='font-size:0.6em;color:#999;'>par</span></div>");
                sb.AppendLine($"                <div class='label'>Compression Ratio</div>");
                sb.AppendLine($"                <div class='value'>{avgCompTime:F1}ms → {avgCompTimePar:F1}ms</div>");
                sb.AppendLine($"                <div class='label'>Comp Time (seq → par) <span class='{(compSpeedup >= 1 ? "speedup-pos" : "speedup-neg")}'>{compSpeedup:F2}x</span></div>");
                sb.AppendLine($"                <div class='value'>{avgDecompTime:F1}ms → {avgDecompTimePar:F1}ms</div>");
                sb.AppendLine($"                <div class='label'>Decomp Time (seq → par) <span class='{(decompSpeedup >= 1 ? "speedup-pos" : "speedup-neg")}'>{decompSpeedup:F2}x</span></div>");
                sb.AppendLine("            </div>");
            }

            sb.AppendLine("        </div>");
        }

        private static void AddDetailedTable(StringBuilder sb, List<Dictionary<string, CompressionResult>> allResults)
        {
            sb.AppendLine("        <h2>📋 Detailed Results (Sequential vs Parallel)</h2>");
            sb.AppendLine("        <div class='table-wrapper'>");
            sb.AppendLine("        <table>");
            sb.AppendLine("            <thead>");
            sb.AppendLine("                <tr>");
            sb.AppendLine("                    <th>File</th>");
            sb.AppendLine("                    <th>Algorithm</th>");
            sb.AppendLine("                    <th>Orig Size</th>");
            sb.AppendLine("                    <th>Comp (seq)</th>");
            sb.AppendLine("                    <th>Comp (par)</th>");
            sb.AppendLine("                    <th>Ratio (seq)</th>");
            sb.AppendLine("                    <th>Ratio (par)</th>");
            sb.AppendLine("                    <th>Comp Time (seq)</th>");
            sb.AppendLine("                    <th>Comp Time (par)</th>");
            sb.AppendLine("                    <th>Comp Speedup</th>");
            sb.AppendLine("                    <th>Decomp Time (seq)</th>");
            sb.AppendLine("                    <th>Decomp Time (par)</th>");
            sb.AppendLine("                    <th>Decomp Speedup</th>");
            sb.AppendLine("                </tr>");
            sb.AppendLine("            </thead>");
            sb.AppendLine("            <tbody>");

            foreach (var results in allResults)
            {
                string fileName = "";
                double bestRatio = 0.0;
                CompressionResult bestResult = null;

                foreach (var result in results.Values)
                {
                    fileName = result.InputFileName;
                    if (result.CompressionRatio > bestRatio)
                    {
                        bestRatio = result.CompressionRatio;
                        bestResult = result;
                    }
                }

                foreach (var result in results.Values)
                {
                    string rowClass = (result == bestResult) ? "class='best'" : "";
                    double compSpeedup = result.GetCompressionSpeedup();
                    double decompSpeedup = result.GetDecompressionSpeedup();

                    sb.AppendLine($"                <tr {rowClass}>");
                    sb.AppendLine($"                    <td>{fileName}</td>");
                    sb.AppendLine($"                    <td><strong>{result.AlgorithmName}</strong></td>");
                    sb.AppendLine($"                    <td>{result.GetFormattedOriginalSize()}</td>");
                    sb.AppendLine($"                    <td>{result.GetFormattedCompressedSize()}</td>");
                    sb.AppendLine($"                    <td>{result.GetFormattedCompressedSizeParallel()}</td>");
                    sb.AppendLine($"                    <td>{result.CompressionRatio:F2}:1</td>");
                    sb.AppendLine($"                    <td>{result.CompressionRatioParallel:F2}:1</td>");
                    sb.AppendLine($"                    <td>{result.CompressionTimeMs:F2}ms</td>");
                    sb.AppendLine($"                    <td>{result.CompressionTimeMsParallel:F2}ms</td>");
                    sb.AppendLine($"                    <td style='color:{(compSpeedup >= 1 ? "#2e7d32" : "#c62828")};font-weight:bold;'>{compSpeedup:F2}x</td>");
                    sb.AppendLine($"                    <td>{result.DecompressionTimeMs:F2}ms</td>");
                    sb.AppendLine($"                    <td>{result.DecompressionTimeMsParallel:F2}ms</td>");
                    sb.AppendLine($"                    <td style='color:{(decompSpeedup >= 1 ? "#2e7d32" : "#c62828")};font-weight:bold;'>{decompSpeedup:F2}x</td>");
                    sb.AppendLine("                </tr>");
                }
            }

            sb.AppendLine("            </tbody>");
            sb.AppendLine("        </table>");
            sb.AppendLine("        </div>");
        }

        private static void AddPerformanceCharts(StringBuilder sb, List<Dictionary<string, CompressionResult>> allResults)
        {
            // Compression ratio comparison
            sb.AppendLine("        <div class='chart-container'>");
            sb.AppendLine("            <div class='chart-title'>📈 Compression Ratio: Sequential vs Parallel</div>");

            foreach (var results in allResults)
            {
                string fileName = results.First().Value.InputFileName;
                sb.AppendLine($"            <div style='margin-bottom: 20px;'>");
                sb.AppendLine($"                <div style='font-weight: bold; margin: 10px 0;'>{fileName}</div>");

                double maxRatio = results.Values.Max(r => Math.Max(r.CompressionRatio, r.CompressionRatioParallel));

                foreach (var result in results.Values)
                {
                    double pctSeq = maxRatio > 0 ? (result.CompressionRatio / maxRatio) * 100 : 0;
                    double pctPar = maxRatio > 0 ? (result.CompressionRatioParallel / maxRatio) * 100 : 0;

                    sb.AppendLine($"                <div class='bar-item'>");
                    sb.AppendLine($"                    <span class='bar-label'>{result.AlgorithmName} (seq)</span>");
                    sb.AppendLine($"                    <div class='progress-bar' style='width: 60%; display: inline-block;'>");
                    sb.AppendLine($"                        <div class='progress-fill' style='width: {pctSeq}%; background: #667eea;'></div>");
                    sb.AppendLine($"                    </div>");
                    sb.AppendLine($"                    <span style='margin-left: 10px;'>{result.CompressionRatio:F1}:1</span>");
                    sb.AppendLine($"                </div>");
                    sb.AppendLine($"                <div class='bar-item'>");
                    sb.AppendLine($"                    <span class='bar-label'>{result.AlgorithmName} (par)</span>");
                    sb.AppendLine($"                    <div class='progress-bar' style='width: 60%; display: inline-block;'>");
                    sb.AppendLine($"                        <div class='progress-fill' style='width: {pctPar}%; background: #43a047;'></div>");
                    sb.AppendLine($"                    </div>");
                    sb.AppendLine($"                    <span style='margin-left: 10px;'>{result.CompressionRatioParallel:F1}:1</span>");
                    sb.AppendLine($"                </div>");
                }

                sb.AppendLine($"            </div>");
            }

            sb.AppendLine("        </div>");

            // Speedup chart
            sb.AppendLine("        <div class='chart-container'>");
            sb.AppendLine("            <div class='chart-title'>⚡ Parallel Speedup (seq time ÷ par time; >1 = parallel faster)</div>");

            var avgCompSpeedup = new Dictionary<string, double>();
            var avgDecompSpeedup = new Dictionary<string, double>();
            var counts = new Dictionary<string, int>();

            foreach (var results in allResults)
            {
                foreach (var result in results.Values)
                {
                    if (!avgCompSpeedup.ContainsKey(result.AlgorithmName))
                    {
                        avgCompSpeedup[result.AlgorithmName] = 0;
                        avgDecompSpeedup[result.AlgorithmName] = 0;
                        counts[result.AlgorithmName] = 0;
                    }
                    avgCompSpeedup[result.AlgorithmName] += result.GetCompressionSpeedup();
                    avgDecompSpeedup[result.AlgorithmName] += result.GetDecompressionSpeedup();
                    counts[result.AlgorithmName]++;
                }
            }

            double maxSpeedup = 1.0;
            foreach (var k in avgCompSpeedup.Keys)
            {
                double c = avgCompSpeedup[k] / counts[k];
                double d = avgDecompSpeedup[k] / counts[k];
                if (c > maxSpeedup) maxSpeedup = c;
                if (d > maxSpeedup) maxSpeedup = d;
            }

            foreach (var algo in avgCompSpeedup.Keys)
            {
                double cSpeed = avgCompSpeedup[algo] / counts[algo];
                double dSpeed = avgDecompSpeedup[algo] / counts[algo];
                double cPct = (cSpeed / maxSpeedup) * 100;
                double dPct = (dSpeed / maxSpeedup) * 100;

                sb.AppendLine($"            <div class='bar-item'>");
                sb.AppendLine($"                <span class='bar-label'>{algo} Comp</span>");
                sb.AppendLine($"                <div class='progress-bar' style='width: 60%; display: inline-block;'>");
                sb.AppendLine($"                    <div class='progress-fill' style='width: {cPct}%; background: #ff5722;'></div>");
                sb.AppendLine($"                </div>");
                sb.AppendLine($"                <span style='margin-left: 10px;'>{cSpeed:F2}x</span>");
                sb.AppendLine($"            </div>");
                sb.AppendLine($"            <div class='bar-item'>");
                sb.AppendLine($"                <span class='bar-label'>{algo} Decomp</span>");
                sb.AppendLine($"                <div class='progress-bar' style='width: 60%; display: inline-block;'>");
                sb.AppendLine($"                    <div class='progress-fill' style='width: {dPct}%; background: #2196f3;'></div>");
                sb.AppendLine($"                </div>");
                sb.AppendLine($"                <span style='margin-left: 10px;'>{dSpeed:F2}x</span>");
                sb.AppendLine($"            </div>");
            }

            sb.AppendLine("        </div>");
        }

        private static void AddRecommendations(StringBuilder sb)
        {
            sb.AppendLine("        <div class='chart-container'>");
            sb.AppendLine("            <div class='chart-title'>💡 Recommendations</div>");
            sb.AppendLine("            <ul style='margin-left: 20px; line-height: 1.8;'>");
            sb.AppendLine("                <li><strong>📄 Text/JSON/HTML/XML:</strong> Brotli or Zstd for best ratio</li>");
            sb.AppendLine("                <li><strong>⚡ Speed-critical:</strong> LZ4 (parallel is often 2-4x faster on multicore)</li>");
            sb.AppendLine("                <li><strong>🎯 General purpose:</strong> GZip for universal compatibility</li>");
            sb.AppendLine("                <li><strong>🌐 Web delivery:</strong> Brotli is the modern standard</li>");
            sb.AppendLine("                <li><strong>🚀 Parallel note:</strong> chunked parallel trades a few % of ratio for speed; use when throughput matters</li>");
            sb.AppendLine("            </ul>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='chart-container'>");
            sb.AppendLine("            <div class='chart-title'>📊 Algorithm Summary</div>");
            sb.AppendLine("            <ul style='margin-left: 20px; line-height: 1.8;'>");
            sb.AppendLine("                <li><strong>LZ4:</strong> 🚀 Fastest, lowest ratio - best for real-time; parallel helps most</li>");
            sb.AppendLine("                <li><strong>GZip:</strong> ⚡ Fast, good ratio - best for compatibility</li>");
            sb.AppendLine("                <li><strong>Zstd:</strong> 🏃 Fast, excellent ratio - best balance</li>");
            sb.AppendLine("                <li><strong>Brotli:</strong> 🐢 Slower, BEST ratio - best for text/web</li>");
            sb.AppendLine("                <li><strong>RLE:</strong> ⚡ Trivial, niche - only for highly repetitive data</li>");
            sb.AppendLine("            </ul>");
            sb.AppendLine("        </div>");
        }
    }
}