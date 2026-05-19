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
            sb.AppendLine("    <title>Compression Algorithm Report</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        * { margin: 0; padding: 0; box-sizing: border-box; }");
            sb.AppendLine("        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 20px; }");
            sb.AppendLine("        .container { max-width: 1400px; margin: 0 auto; background: white; border-radius: 15px; box-shadow: 0 20px 60px rgba(0,0,0,0.3); overflow: hidden; }");
            sb.AppendLine("        .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; }");
            sb.AppendLine("        .header h1 { font-size: 2.5em; margin-bottom: 10px; }");
            sb.AppendLine("        .header p { font-size: 1.1em; opacity: 0.9; }");
            sb.AppendLine("        .content { padding: 30px; }");
            sb.AppendLine("        .summary-cards { display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 20px; margin-bottom: 40px; }");
            sb.AppendLine("        .card { background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%); padding: 20px; border-radius: 10px; text-align: center; transition: transform 0.3s; }");
            sb.AppendLine("        .card:hover { transform: translateY(-5px); }");
            sb.AppendLine("        .card h3 { color: #333; margin-bottom: 10px; }");
            sb.AppendLine("        .card .value { font-size: 2em; font-weight: bold; color: #667eea; }");
            sb.AppendLine("        .card .label { color: #666; margin-top: 5px; }");
            sb.AppendLine("        .table-wrapper { max-height: 600px; overflow-y: auto; border: 1px solid #ddd; border-radius: 10px; margin-bottom: 30px; }");
            sb.AppendLine("        table { width: 100%; border-collapse: collapse; }");
            sb.AppendLine("        th { background: #667eea; color: white; padding: 12px; text-align: left; position: sticky; top: 0; z-index: 10; }");
            sb.AppendLine("        td { padding: 10px; border-bottom: 1px solid #ddd; }");
            sb.AppendLine("        tr:hover { background: #f5f5f5; }");
            sb.AppendLine("        .best { background: #d4edda; font-weight: bold; }");
            sb.AppendLine("        .progress-bar { background: #e0e0e0; border-radius: 10px; overflow: hidden; height: 20px; }");
            sb.AppendLine("        .progress-fill { background: linear-gradient(90deg, #4caf50, #8bc34a); height: 100%; transition: width 0.3s; }");
            sb.AppendLine("        .chart-container { margin: 30px 0; padding: 20px; background: #f9f9f9; border-radius: 10px; }");
            sb.AppendLine("        .chart-title { font-size: 1.3em; margin-bottom: 15px; color: #333; }");
            sb.AppendLine("        .bar-chart { margin: 10px 0; }");
            sb.AppendLine("        .bar-item { margin: 10px 0; }");
            sb.AppendLine("        .bar-label { display: inline-block; width: 120px; font-weight: bold; }");
            sb.AppendLine("        .footer { background: #333; color: white; padding: 20px; text-align: center; }");
            sb.AppendLine("        @media (max-width: 768px) { .summary-cards { grid-template-columns: 1fr; } table { font-size: 12px; } .bar-label { width: 80px; } }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class='container'>");
            sb.AppendLine("        <div class='header'>");
            sb.AppendLine("            <h1>📊 Compression Algorithm Evaluation Report</h1>");
            sb.AppendLine($"            <p>Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
            sb.AppendLine($"            <p>Files Tested: {allResults.Count}</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='content'>");

            AddSummaryCards(sb, allResults);
            AddDetailedTable(sb, allResults);
            AddPerformanceCharts(sb, allResults);
            AddRecommendations(sb);

            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='footer'>");
            sb.AppendLine("            <p>Compression Algorithm Evaluator | 5 Algorithms: GZip, Brotli, RLE, LZ4, Zstd</p>");
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
                double avgRatio = 0;
                double avgSaving = 0;
                double avgCompTime = 0;

                foreach (var r in algo.Value)
                {
                    avgRatio += r.CompressionRatio;
                    avgSaving += r.GetSpaceSavingPercentage();
                    avgCompTime += r.CompressionTimeMs;
                }

                avgRatio /= algo.Value.Count;
                avgSaving /= algo.Value.Count;
                avgCompTime /= algo.Value.Count;

                sb.AppendLine("            <div class='card'>");
                sb.AppendLine($"                <h3>{algo.Key}</h3>");
                sb.AppendLine($"                <div class='value'>{avgRatio:F1}:1</div>");
                sb.AppendLine($"                <div class='label'>Average Compression Ratio</div>");
                sb.AppendLine($"                <div class='value'>{avgSaving:F1}%</div>");
                sb.AppendLine($"                <div class='label'>Space Saving</div>");
                sb.AppendLine($"                <div class='value'>{avgCompTime:F0}ms</div>");
                sb.AppendLine($"                <div class='label'>Avg Compression Time</div>");
                sb.AppendLine("            </div>");
            }

            sb.AppendLine("        </div>");
        }

        private static void AddDetailedTable(StringBuilder sb, List<Dictionary<string, CompressionResult>> allResults)
        {
            sb.AppendLine("        <h2>📋 Detailed Results</h2>");
            sb.AppendLine("        <div class='table-wrapper'>");
            sb.AppendLine("        <table>");
            sb.AppendLine("            <thead>");
            sb.AppendLine("                <tr>");
            sb.AppendLine("                    <th>File Name</th>");
            sb.AppendLine("                    <th>Algorithm</th>");
            sb.AppendLine("                    <th>Original Size</th>");
            sb.AppendLine("                    <th>Compressed Size</th>");
            sb.AppendLine("                    <th>Ratio</th>");
            sb.AppendLine("                    <th>Space Saving</th>");
            sb.AppendLine("                    <th>Compression Time</th>");
            sb.AppendLine("                </tr>");
            sb.AppendLine("            </thead>");
            sb.AppendLine("            <tbody>");

            foreach (var results in allResults)
            {
                string fileName = "";
                var bestRatio = 0.0;
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
                    sb.AppendLine($"                <tr {rowClass}>");
                    sb.AppendLine($"                    <td>{fileName}</td>");
                    sb.AppendLine($"                    <td><strong>{result.AlgorithmName}</strong></td>");
                    sb.AppendLine($"                    <td>{result.GetFormattedOriginalSize()}</td>");
                    sb.AppendLine($"                    <td>{result.GetFormattedCompressedSize()}</td>");
                    sb.AppendLine($"                    <td>{result.CompressionRatio:F2}:1</td>");
                    sb.AppendLine($"                    <td>{result.GetSpaceSavingPercentage():F1}%</td>");
                    sb.AppendLine($"                    <td>{result.CompressionTimeMs:F2}ms</td>");
                    sb.AppendLine("                </tr>");
                }
            }

            sb.AppendLine("            </tbody>");
            sb.AppendLine("        </table>");
            sb.AppendLine("        </div>");
        }

        private static void AddPerformanceCharts(StringBuilder sb, List<Dictionary<string, CompressionResult>> allResults)
        {
            sb.AppendLine("        <div class='chart-container'>");
            sb.AppendLine("            <div class='chart-title'>📈 Compression Ratio Comparison</div>");

            foreach (var results in allResults)
            {
                string fileName = results.First().Value.InputFileName;
                sb.AppendLine($"            <div class='bar-chart'>");
                sb.AppendLine($"                <div style='font-weight: bold; margin: 10px 0;'>{fileName}</div>");

                double maxRatio = results.Values.Max(r => r.CompressionRatio);

                foreach (var result in results.Values)
                {
                    double percentage = (result.CompressionRatio / maxRatio) * 100;
                    // Color coding for 5 algorithms
                    string barColor = result.AlgorithmName == "Brotli" ? "#4caf50" :
                                     (result.AlgorithmName == "Zstd" ? "#2196f3" :
                                     (result.AlgorithmName == "GZip" ? "#ff9800" :
                                     (result.AlgorithmName == "LZ4" ? "#9c27b0" : "#f44336")));

                    sb.AppendLine($"                <div class='bar-item'>");
                    sb.AppendLine($"                    <span class='bar-label'>{result.AlgorithmName}</span>");
                    sb.AppendLine($"                    <div class='progress-bar' style='width: 70%; display: inline-block;'>");
                    sb.AppendLine($"                        <div class='progress-fill' style='width: {percentage}%; background: {barColor};'></div>");
                    sb.AppendLine($"                    </div>");
                    sb.AppendLine($"                    <span style='margin-left: 10px;'>{result.CompressionRatio:F1}:1</span>");
                    sb.AppendLine($"                </div>");
                }

                sb.AppendLine($"            </div>");
            }

            sb.AppendLine("        </div>");

            // Speed Comparison Chart
            sb.AppendLine("        <div class='chart-container'>");
            sb.AppendLine("            <div class='chart-title'>⚡ Compression Speed Comparison (Lower is Better)</div>");

            var avgTimes = new Dictionary<string, double>();
            foreach (var results in allResults)
            {
                foreach (var result in results.Values)
                {
                    if (!avgTimes.ContainsKey(result.AlgorithmName))
                        avgTimes[result.AlgorithmName] = 0;
                    avgTimes[result.AlgorithmName] += result.CompressionTimeMs;
                }
            }

            double maxTime = avgTimes.Values.Max();

            foreach (var algo in avgTimes)
            {
                double avg = algo.Value / allResults.Count;
                double percentage = (maxTime - avg) / maxTime * 100;

                sb.AppendLine($"            <div class='bar-item'>");
                sb.AppendLine($"                <span class='bar-label'>{algo.Key}</span>");
                sb.AppendLine($"                <div class='progress-bar' style='width: 70%; display: inline-block;'>");
                sb.AppendLine($"                    <div class='progress-fill' style='width: {percentage}%; background: #ff5722;'></div>");
                sb.AppendLine($"                </div>");
                sb.AppendLine($"                <span style='margin-left: 10px;'>{avg:F0}ms avg</span>");
                sb.AppendLine($"            </div>");
            }

            sb.AppendLine("        </div>");
        }

        private static void AddRecommendations(StringBuilder sb)
        {
            sb.AppendLine("        <div class='chart-container'>");
            sb.AppendLine("            <div class='chart-title'>💡 Recommendations</div>");
            sb.AppendLine("            <ul style='margin-left: 20px; line-height: 1.8;'>");
            sb.AppendLine("                <li><strong>📄 Text/JSON/HTML/XML files:</strong> Use <strong style='color: #4caf50;'>Brotli</strong> or <strong style='color: #2196f3;'>Zstd</strong> for best compression ratio</li>");
            sb.AppendLine("                <li><strong>⚡ Speed-critical applications (real-time, gaming):</strong> Use <strong style='color: #9c27b0;'>LZ4</strong> for maximum speed (50-100x faster than Brotli)</li>");
            sb.AppendLine("                <li><strong>🔄 Highly repetitive data (simple graphics, logs):</strong> <strong style='color: #ff9800;'>GZip</strong> or <strong style='color: #2196f3;'>Zstd</strong> offer good balance</li>");
            sb.AppendLine("                <li><strong>🎯 General purpose / Compatibility:</strong> <strong style='color: #ff9800;'>GZip</strong> offers universal support and decent compression</li>");
            sb.AppendLine("                <li><strong>🌐 Web delivery (HTTP/HTTPS):</strong> Brotli is the modern standard supported by all major browsers</li>");
            sb.AppendLine("                <li><strong>🗄️ Database backups / Archiving:</strong> <strong style='color: #4caf50;'>Brotli</strong> or <strong style='color: #2196f3;'>Zstd</strong> for best space savings</li>");
            sb.AppendLine("                <li><strong>📦 Already compressed files (JPEG, PNG, MP3, ZIP):</strong> Don't compress again - minimal or negative gains</li>");
            sb.AppendLine("                <li><strong>🎲 Random / Encrypted data:</strong> No algorithm helps - data with high entropy cannot be compressed</li>");
            sb.AppendLine("            </ul>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class='chart-container'>");
            sb.AppendLine("            <div class='chart-title'>📊 Algorithm Summary</div>");
            sb.AppendLine("            <ul style='margin-left: 20px; line-height: 1.8;'>");
            sb.AppendLine("                <li><strong>LZ4:</strong> 🚀 Fastest compression (0.05-0.2ms), lowest ratio (1.5-3:1) - Best for real-time</li>");
            sb.AppendLine("                <li><strong>GZip:</strong> ⚡ Fast compression (0.5-2ms), good ratio (5-15:1) - Best for compatibility</li>");
            sb.AppendLine("                <li><strong>Zstd:</strong> 🏃 Fast compression (0.5-2ms), excellent ratio (10-50:1) - Best balance</li>");
            sb.AppendLine("                <li><strong>Brotli:</strong> 🐢 Slower compression (1-5ms), BEST ratio (20-10000:1) - Best for text/web</li>");
            sb.AppendLine("                <li><strong>RLE:</strong> ⚡ Very fast but only works on repetitive data - Niche use only</li>");
            sb.AppendLine("            </ul>");
            sb.AppendLine("        </div>");
        }
    }
}