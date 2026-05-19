using System.IO.Compression;

namespace CompressionEvaluation.CompressionAlgorithms
{
    public static class GZipCompression
    {

        // Compresses byte array using GZip
        public static byte[] Compress(byte[] data)
        {
            using var outputStream = new MemoryStream();
            using (var gzipStream = new GZipStream(outputStream, CompressionLevel.Optimal))
            {
                gzipStream.Write(data, 0, data.Length);
            }
            return outputStream.ToArray();
        }


        // Decompresses GZip compressed byte array
        public static byte[] Decompress(byte[] compressedData)
        {
            using var inputStream = new MemoryStream(compressedData);
            using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();
            gzipStream.CopyTo(outputStream);
            return outputStream.ToArray();
        }


        // Compresses file using GZip
        public static void CompressFile(string inputPath, string outputPath)
        {
            byte[] originalData = File.ReadAllBytes(inputPath);
            byte[] compressedData = Compress(originalData);
            File.WriteAllBytes(outputPath, compressedData);
        }

        // Decompresses GZip file
        public static void DecompressFile(string compressedPath, string outputPath)
        {
            byte[] compressedData = File.ReadAllBytes(compressedPath);
            byte[] decompressedData = Decompress(compressedData);
            File.WriteAllBytes(outputPath, decompressedData);
        }
    }
}
