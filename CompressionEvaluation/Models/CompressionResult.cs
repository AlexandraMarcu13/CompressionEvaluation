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

        public double GetSpaceSavingPercentage()
        {
            return (1 - (double)CompressedSize / OriginalSize) * 100;
        }

        public string GetFormattedOriginalSize()
        {
            return FormatBytes(OriginalSize);
        }

        public string GetFormattedCompressedSize()
        {
            return FormatBytes(CompressedSize);
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
                   $"Compressed: {GetFormattedCompressedSize(),-12} | " +
                   $"Ratio: {CompressionRatio:F2}:1 | " +
                   $"Space Saving: {GetSpaceSavingPercentage():F1}% | " +
                   $"Comp Time: {CompressionTimeMs:F2}ms | " +
                   $"Decomp Time: {DecompressionTimeMs:F2}ms";
        }
    }
}