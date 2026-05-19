using ZstdNet;

namespace CompressionEvaluation.CompressionAlgorithms
{
    public static class ZstdCompression
    {
        private static readonly CompressionOptions DefaultOptions = new CompressionOptions(3);
        private static readonly CompressionOptions MaxOptions = new CompressionOptions(22);
        private static readonly DecompressionOptions DecompOptions = new DecompressionOptions();


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
    }
}