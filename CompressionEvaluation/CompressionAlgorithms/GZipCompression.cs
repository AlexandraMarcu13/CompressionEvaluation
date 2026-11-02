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

        // Parallel compression: split into chunks, compress each as its own GZip member
        // in parallel, concatenate. Concatenated GZip members are valid per RFC 1952.
        public static byte[] CompressParallel(byte[] data)
        {
            if (data == null || data.Length == 0)
                return new byte[0];

            int processorCount = Environment.ProcessorCount;
            int chunkSize = Math.Max(1, data.Length / processorCount);
            int chunkCount = (data.Length + chunkSize - 1) / chunkSize;

            var chunkResults = new byte[chunkCount][];

            Parallel.For(0, chunkCount, chunkIndex =>
            {
                int start = chunkIndex * chunkSize;
                int length = Math.Min(chunkSize, data.Length - start);

                byte[] chunk = new byte[length];
                Array.Copy(data, start, chunk, 0, length);

                chunkResults[chunkIndex] = Compress(chunk);
            });

            int total = 0;
            for (int i = 0; i < chunkResults.Length; i++)
                total += chunkResults[i].Length;

            byte[] output = new byte[total];
            int offset = 0;
            for (int i = 0; i < chunkResults.Length; i++)
            {
                Array.Copy(chunkResults[i], 0, output, offset, chunkResults[i].Length);
                offset += chunkResults[i].Length;
            }

            return output;
        }

        // Parallel decompression: GZipStream reads concatenated members as one stream,
        // so this uses the same decompressor (kept as a separate method for symmetry).
        public static byte[] DecompressParallel(byte[] compressedData)
        {
            // Concatenated GZip members are decoded by a single GZipStream call.
            return Decompress(compressedData);
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