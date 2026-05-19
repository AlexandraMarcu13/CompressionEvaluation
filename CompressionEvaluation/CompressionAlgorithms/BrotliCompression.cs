using System.IO.Compression;

namespace CompressionEvaluation.CompressionAlgorithms
{
    public static class BrotliCompression
    {
        // Compresses byte array using Brotli
        public static byte[] Compress(byte[] data)
        {
            using var outputStream = new MemoryStream();
            using (var brotliStream = new BrotliStream(outputStream, CompressionLevel.Optimal))
            {
                brotliStream.Write(data, 0, data.Length);
            }
            return outputStream.ToArray();
        }

        /// Decompresses Brotli compressed byte array
        public static byte[] Decompress(byte[] compressedData)
        {
            using var inputStream = new MemoryStream(compressedData);
            using var brotliStream = new BrotliStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();
            brotliStream.CopyTo(outputStream);
            return outputStream.ToArray();
        }

        // Compresses file using Brotli
        public static void CompressFile(string inputPath, string outputPath)
        {
            byte[] originalData = File.ReadAllBytes(inputPath);
            byte[] compressedData = Compress(originalData);
            File.WriteAllBytes(outputPath, compressedData);
        }

        // Decompresses Brotli file
        public static void DecompressFile(string compressedPath, string outputPath)
        {
            byte[] compressedData = File.ReadAllBytes(compressedPath);
            byte[] decompressedData = Decompress(compressedData);
            File.WriteAllBytes(outputPath, decompressedData);
        }
    }
}
