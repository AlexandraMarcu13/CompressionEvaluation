namespace CompressionEvaluation.CompressionAlgorithms
{
    public static class RLECompression
    {
        // Compresses byte array using Run-Length Encoding(RLE)
        public static byte[] Compress(byte[] data)
        {
            if (data == null || data.Length == 0)
                return new byte[0];

            var result = new List<byte>();
            int count = 1;

            for (int i = 1; i <= data.Length; i++)
            {
                if (i < data.Length && data[i] == data[i - 1])
                {
                    count++;
                }
                else
                {
                    // Encode run (max count 255 per run)
                    while (count > 255)
                    {
                        result.Add(255);
                        result.Add(data[i - 1]);
                        count -= 255;
                    }
                    result.Add((byte)count);
                    result.Add(data[i - 1]);
                    count = 1;
                }
            }

            return result.ToArray();
        }

        // Decompresses RLE compressed byte array
        public static byte[] Decompress(byte[] compressedData)
        {
            if (compressedData == null || compressedData.Length == 0)
                return new byte[0];

            var result = new List<byte>();

            for (int i = 0; i < compressedData.Length; i += 2)
            {
                if (i + 1 < compressedData.Length)
                {
                    byte count = compressedData[i];
                    byte value = compressedData[i + 1];

                    for (int j = 0; j < count; j++)
                    {
                        result.Add(value);
                    }
                }
            }

            return result.ToArray();
        }

        // Parallel compression.
        //
        // Each thread must NOT compute its own chunk boundaries independently:
        // if chunk N extends its end to finish a run, and chunk N+1 backs up its
        // start to begin that same run, both chunks emit the same bytes and the
        // concatenated output decodes to a longer array than the original.
        //
        public static byte[] CompressParallel(byte[] data)
        {
            if (data == null || data.Length == 0)
                return new byte[0];

            int processorCount = Environment.ProcessorCount;

            // Avoid creating tiny chunks on small files (thread overhead dominates).
            int targetChunks = Math.Max(1, Math.Min(processorCount, data.Length / 4096));
            if (targetChunks == 1)
                return Compress(data);

            // Compute strictly increasing run-aligned boundaries:
            // boundaries[0] = 0, boundaries[targetChunks] = data.Length,
            // and boundaries[i] sits on a position where data[pos] != data[pos - 1].
            var boundaries = new int[targetChunks + 1];
            boundaries[0] = 0;
            boundaries[targetChunks] = data.Length;

            int approxChunkSize = data.Length / targetChunks;
            for (int i = 1; i < targetChunks; i++)
            {
                int pos = i * approxChunkSize;

                // Advance to the next run boundary (start of a new run).
                while (pos < data.Length && pos > 0 && data[pos] == data[pos - 1])
                    pos++;

                boundaries[i] = pos;
            }

            // Guard against non-strictly-increasing boundaries (e.g. homogeneous input).
            for (int i = 1; i <= targetChunks; i++)
            {
                if (boundaries[i] <= boundaries[i - 1])
                    boundaries[i] = Math.Min(boundaries[i - 1] + 1, data.Length);
            }

            var chunkResults = new byte[targetChunks][];

            Parallel.For(0, targetChunks, chunkIndex =>
            {
                int start = boundaries[chunkIndex];
                int end = boundaries[chunkIndex + 1];

                if (end <= start)
                {
                    chunkResults[chunkIndex] = new byte[0];
                    return;
                }

                byte[] chunk = new byte[end - start];
                Array.Copy(data, start, chunk, 0, chunk.Length);
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

        // Parallel decompression.
        //
        // RLE pairs are stateless, so we can split the compressed stream at any pair
        // boundary. Each chunk of pairs decodes independently; concatenation is the
        // original data.
        public static byte[] DecompressParallel(byte[] compressedData)
        {
            if (compressedData == null || compressedData.Length == 0)
                return new byte[0];

            int totalPairs = compressedData.Length / 2;

            int processorCount = Environment.ProcessorCount;
            int targetChunks = Math.Max(1, Math.Min(processorCount, totalPairs / 2048));
            if (targetChunks == 1)
                return Decompress(compressedData);

            int pairsPerChunk = totalPairs / targetChunks;

            var chunkResults = new byte[targetChunks][];

            Parallel.For(0, targetChunks, chunkIndex =>
            {
                int startPair = chunkIndex * pairsPerChunk;
                int endPair = (chunkIndex == targetChunks - 1)
                    ? totalPairs
                    : (chunkIndex + 1) * pairsPerChunk;

                int startByte = startPair * 2;
                int byteLength = (endPair - startPair) * 2;

                byte[] chunk = new byte[byteLength];
                Array.Copy(compressedData, startByte, chunk, 0, byteLength);

                chunkResults[chunkIndex] = Decompress(chunk);
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

        // Compresses file using RLE
        public static byte[] CompressFile(string inputPath)
        {
            byte[] originalData = System.IO.File.ReadAllBytes(inputPath);
            return Compress(originalData);
        }

        // Decompresses RLE file
        public static byte[] DecompressFile(string compressedPath)
        {
            byte[] compressedData = System.IO.File.ReadAllBytes(compressedPath);
            return Decompress(compressedData);
        }
    }
}