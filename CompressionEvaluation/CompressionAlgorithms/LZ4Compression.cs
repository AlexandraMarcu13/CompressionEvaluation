using K4os.Compression.LZ4;

namespace CompressionEvaluation.CompressionAlgorithms
{
    public static class LZ4Compression
    {
        // Compresses byte array using LZ4 algorithm (fastest compression)
        public static byte[] Compress(byte[] data)
        {
            if (data == null || data.Length == 0)
                return new byte[0];

            int maxSize = LZ4Codec.MaximumOutputSize(data.Length);
            byte[] compressed = new byte[maxSize];

            int compressedSize = LZ4Codec.Encode(
                data, 0, data.Length,
                compressed, 0, compressed.Length,
                LZ4Level.L00_FAST);

            Array.Resize(ref compressed, compressedSize);
            return compressed;
        }

        // Decompresses LZ4 compressed byte array
        public static byte[] Decompress(byte[] compressedData, int originalSize)
        {
            if (compressedData == null || compressedData.Length == 0)
                return new byte[0];

            byte[] decompressed = new byte[originalSize];
            int decodedSize = LZ4Codec.Decode(
                compressedData, 0, compressedData.Length,
                decompressed, 0, decompressed.Length);

            if (decodedSize != originalSize)
                Array.Resize(ref decompressed, decodedSize);

            return decompressed;
        }

        // Parallel compression: chunks compressed independently with framing:
        // [int32 chunkCount][for each: int32 originalLen, int32 compressedLen][data...]
        public static byte[] CompressParallel(byte[] data)
        {
            if (data == null || data.Length == 0)
                return new byte[0];

            int processorCount = Environment.ProcessorCount;
            int chunkSize = Math.Max(1, data.Length / processorCount);
            int chunkCount = (data.Length + chunkSize - 1) / chunkSize;

            var compressedChunks = new byte[chunkCount][];
            var originalLengths = new int[chunkCount];

            Parallel.For(0, chunkCount, chunkIndex =>
            {
                int start = chunkIndex * chunkSize;
                int length = Math.Min(chunkSize, data.Length - start);

                byte[] chunk = new byte[length];
                Array.Copy(data, start, chunk, 0, length);

                originalLengths[chunkIndex] = length;
                compressedChunks[chunkIndex] = Compress(chunk);
            });

            int headerSize = 4 + chunkCount * 8;
            int total = headerSize;
            for (int i = 0; i < chunkCount; i++)
                total += compressedChunks[i].Length;

            byte[] output = new byte[total];
            BitConverter.GetBytes(chunkCount).CopyTo(output, 0);

            int offset = headerSize;
            for (int i = 0; i < chunkCount; i++)
            {
                BitConverter.GetBytes(originalLengths[i]).CopyTo(output, 4 + i * 8);
                BitConverter.GetBytes(compressedChunks[i].Length).CopyTo(output, 4 + i * 8 + 4);
                Array.Copy(compressedChunks[i], 0, output, offset, compressedChunks[i].Length);
                offset += compressedChunks[i].Length;
            }

            return output;
        }

        // Parallel decompression
        public static byte[] DecompressParallel(byte[] compressedData)
        {
            if (compressedData == null || compressedData.Length == 0)
                return new byte[0];

            int chunkCount = BitConverter.ToInt32(compressedData, 0);
            int headerSize = 4 + chunkCount * 8;

            int[] origLens = new int[chunkCount];
            int[] compLens = new int[chunkCount];
            int[] compOffsets = new int[chunkCount];

            int offset = headerSize;
            for (int i = 0; i < chunkCount; i++)
            {
                origLens[i] = BitConverter.ToInt32(compressedData, 4 + i * 8);
                compLens[i] = BitConverter.ToInt32(compressedData, 4 + i * 8 + 4);
                compOffsets[i] = offset;
                offset += compLens[i];
            }

            var chunkResults = new byte[chunkCount][];

            Parallel.For(0, chunkCount, i =>
            {
                byte[] chunk = new byte[compLens[i]];
                Array.Copy(compressedData, compOffsets[i], chunk, 0, compLens[i]);
                chunkResults[i] = Decompress(chunk, origLens[i]);
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