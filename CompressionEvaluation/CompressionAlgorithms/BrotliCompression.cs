using System.IO.Compression;
using System.Buffers.Binary;

namespace CompressionEvaluation.CompressionAlgorithms
{
    public static class BrotliCompression
    {
        // Minimum chunk size for parallel compression.
        // Below this, chunk-dictionary resets hurt compression ratio more than
        // parallelism helps speed, so we reduce the chunk count instead.
        private const int MinParallelChunkSize = 1024 * 1024; // 1 MB

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

        // Decompresses Brotli compressed byte array
        public static byte[] Decompress(byte[] compressedData)
        {
            using var inputStream = new MemoryStream(compressedData);
            using var brotliStream = new BrotliStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();
            brotliStream.CopyTo(outputStream);
            return outputStream.ToArray();
        }

        // Parallel compression: chunks compressed independently, framed with a header:
        // [int32 chunkCount][for each chunk: int32 compressedLen][concatenated compressed chunks]
        //
        // Chunk count is capped so each chunk is at least MinParallelChunkSize.
        // This preserves compression ratio on small files (fewer dictionary resets)
        // while still parallelizing large files across all available cores.
        public static byte[] CompressParallel(byte[] data)
        {
            if (data == null || data.Length == 0)
                return new byte[0];

            int processorCount = Environment.ProcessorCount;
            int maxChunksByMinSize = Math.Max(1, data.Length / MinParallelChunkSize);
            int targetChunks = Math.Max(1, Math.Min(processorCount, maxChunksByMinSize));
            int chunkSize = Math.Max(1, data.Length / targetChunks);
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

            int headerSize = 4 + chunkCount * 4;
            int total = headerSize;
            for (int i = 0; i < chunkResults.Length; i++)
                total += chunkResults[i].Length;

            byte[] output = new byte[total];
            BinaryPrimitives.WriteInt32LittleEndian(output.AsSpan(0, 4), chunkCount);

            int offset = headerSize;
            for (int i = 0; i < chunkResults.Length; i++)
            {
                BinaryPrimitives.WriteInt32LittleEndian(output.AsSpan(4 + i * 4, 4), chunkResults[i].Length);
                Array.Copy(chunkResults[i], 0, output, offset, chunkResults[i].Length);
                offset += chunkResults[i].Length;
            }

            return output;
        }

        // Parallel decompression: reads the header to find chunk boundaries,
        // decompresses each chunk in parallel, concatenates.
        public static byte[] DecompressParallel(byte[] compressedData)
        {
            if (compressedData == null || compressedData.Length == 0)
                return new byte[0];

            int chunkCount = BinaryPrimitives.ReadInt32LittleEndian(compressedData.AsSpan(0, 4));
            int headerSize = 4 + chunkCount * 4;

            int[] chunkLengths = new int[chunkCount];
            int[] chunkOffsets = new int[chunkCount];
            int offset = headerSize;
            for (int i = 0; i < chunkCount; i++)
            {
                chunkLengths[i] = BinaryPrimitives.ReadInt32LittleEndian(compressedData.AsSpan(4 + i * 4, 4));
                chunkOffsets[i] = offset;
                offset += chunkLengths[i];
            }

            var chunkResults = new byte[chunkCount][];

            Parallel.For(0, chunkCount, i =>
            {
                byte[] chunk = new byte[chunkLengths[i]];
                Array.Copy(compressedData, chunkOffsets[i], chunk, 0, chunkLengths[i]);
                chunkResults[i] = Decompress(chunk);
            });

            int total = 0;
            for (int i = 0; i < chunkResults.Length; i++)
                total += chunkResults[i].Length;

            byte[] output = new byte[total];
            int outOffset = 0;
            for (int i = 0; i < chunkResults.Length; i++)
            {
                Array.Copy(chunkResults[i], 0, output, outOffset, chunkResults[i].Length);
                outOffset += chunkResults[i].Length;
            }

            return output;
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