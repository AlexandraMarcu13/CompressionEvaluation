using ZstdNet;
using System.Buffers.Binary;

namespace CompressionEvaluation.CompressionAlgorithms
{
    public static class ZstdCompression
    {
        private static readonly CompressionOptions DefaultOptions = new CompressionOptions(3);
        private static readonly CompressionOptions MaxOptions = new CompressionOptions(22);

        // Minimum chunk size for parallel compression.
        // Below this, chunk-dictionary resets hurt compression ratio more than
        // parallelism helps speed, so we reduce the chunk count instead.
        private const int MinParallelChunkSize = 1024 * 1024; // 1 MB

        // Compresses byte array using Zstandard algorithm (level 3)
        public static byte[] Compress(byte[] data)
        {
            if (data == null || data.Length == 0)
                return new byte[0];

            using var compressor = new Compressor(DefaultOptions);
            return compressor.Wrap(data);
        }

        // Compresses byte array using Zstandard with maximum compression (level 22)
        public static byte[] CompressMax(byte[] data)
        {
            if (data == null || data.Length == 0)
                return new byte[0];

            using var compressor = new Compressor(MaxOptions);
            return compressor.Wrap(data);
        }

        // Decompresses Zstandard compressed byte array
        public static byte[] Decompress(byte[] compressedData)
        {
            if (compressedData == null || compressedData.Length == 0)
                return new byte[0];

            using var decompressor = new Decompressor();
            return decompressor.Unwrap(compressedData);
        }

        // Parallel compression with framing:
        // [int32 chunkCount][for each: int32 compressedLen][compressed chunks...]
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
            for (int i = 0; i < chunkCount; i++)
                total += chunkResults[i].Length;

            byte[] output = new byte[total];
            BinaryPrimitives.WriteInt32LittleEndian(output.AsSpan(0, 4), chunkCount);

            int offset = headerSize;
            for (int i = 0; i < chunkCount; i++)
            {
                BinaryPrimitives.WriteInt32LittleEndian(output.AsSpan(4 + i * 4, 4), chunkResults[i].Length);
                Array.Copy(chunkResults[i], 0, output, offset, chunkResults[i].Length);
                offset += chunkResults[i].Length;
            }

            return output;
        }

        // Parallel decompression with framing
        public static byte[] DecompressParallel(byte[] compressedData)
        {
            if (compressedData == null || compressedData.Length == 0)
                return new byte[0];

            int chunkCount = BinaryPrimitives.ReadInt32LittleEndian(compressedData.AsSpan(0, 4));
            int headerSize = 4 + chunkCount * 4;

            int[] lens = new int[chunkCount];
            int[] offsets = new int[chunkCount];
            int offset = headerSize;
            for (int i = 0; i < chunkCount; i++)
            {
                lens[i] = BinaryPrimitives.ReadInt32LittleEndian(compressedData.AsSpan(4 + i * 4, 4));
                offsets[i] = offset;
                offset += lens[i];
            }

            var chunkResults = new byte[chunkCount][];

            Parallel.For(0, chunkCount, i =>
            {
                byte[] chunk = new byte[lens[i]];
                Array.Copy(compressedData, offsets[i], chunk, 0, lens[i]);
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
    }
}