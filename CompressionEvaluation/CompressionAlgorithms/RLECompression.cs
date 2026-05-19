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


        // Compresses file using RLE
        public static byte[] CompressFile(string inputPath)
        {
            byte[] originalData = System.IO.File.ReadAllBytes(inputPath);
            return Compress(originalData);
        }


        //Decompresses RLE file

        public static byte[] DecompressFile(string compressedPath)
        {
            byte[] compressedData = System.IO.File.ReadAllBytes(compressedPath);
            return Decompress(compressedData);
        }
    }
}
