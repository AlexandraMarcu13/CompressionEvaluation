namespace CompressionEvaluation.Models
{
    public class CompressionResult
    {
        public string AlgorithmName { get; set; }
        public string InputFileName { get; set; }
        public long OriginalSize { get; set; }
        public long CompressedSize { get; set; }
        public double CompressionRatio { get; set; }
        public double CompressionTimeMs { get; set; }
        public double DecompressionTimeMs { get; set; }

        // --- Parallel stats ---
        public long CompressedSizeParallel { get; set; }
        public double CompressionRatioParallel { get; set; }
        public double CompressionTimeMsParallel { get; set; }
        public double DecompressionTimeMsParallel { get; set; }

        public double GetSpaceSavingPercentage()
        {
            return (1 - (double)CompressedSize / OriginalSize) * 100;
        }

        public double GetSpaceSavingPercentageParallel()
        {
            return (1 - (double)CompressedSizeParallel / OriginalSize) * 100;
        }

        public double GetCompressionSpeedup()
        {
            if (CompressionTimeMsParallel <= 0) return 0;
            return CompressionTimeMs / CompressionTimeMsParallel;
        }

        public double GetDecompressionSpeedup()
        {
            if (DecompressionTimeMsParallel <= 0) return 0;
            return DecompressionTimeMs / DecompressionTimeMsParallel;
        }

        public string GetFormattedOriginalSize()
        {
            return FormatBytes(OriginalSize);
        }

        public string GetFormattedCompressedSize()
        {
            return FormatBytes(CompressedSize);
        }

        public string GetFormattedCompressedSizeParallel()
        {
            return FormatBytes(CompressedSizeParallel);
        }

        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:F2} {sizes[order]}";
        }

        public override string ToString()
        {
            return $"{AlgorithmName,-15} | " +
                   $"Original: {GetFormattedOriginalSize(),-12} | " +
                   $"Comp(seq): {GetFormattedCompressedSize(),-12} | " +
                   $"Comp(par): {GetFormattedCompressedSizeParallel(),-12} | " +
                   $"Ratio(seq): {CompressionRatio:F2}:1 | " +
                   $"Ratio(par): {CompressionRatioParallel:F2}:1 | " +
                   $"CompTime(seq): {CompressionTimeMs:F2}ms | " +
                   $"CompTime(par): {CompressionTimeMsParallel:F2}ms | " +
                   $"Decomp(seq): {DecompressionTimeMs:F2}ms | " +
                   $"Decomp(par): {DecompressionTimeMsParallel:F2}ms";
        }
    }
}