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

            // Calculate maximum possible compressed size
            int maxSize = LZ4Codec.MaximumOutputSize(data.Length);
            byte[] compressed = new byte[maxSize];

            // Compress with FAST level for maximum speed
            int compressedSize = LZ4Codec.Encode(
                data, 0, data.Length,
                compressed, 0, compressed.Length,
                LZ4Level.L00_FAST);

            // Trim to actual size
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

            // If decoded size doesn't match expected, resize
            if (decodedSize != originalSize)
                Array.Resize(ref decompressed, decodedSize);

            return decompressed;
        }
    }
}