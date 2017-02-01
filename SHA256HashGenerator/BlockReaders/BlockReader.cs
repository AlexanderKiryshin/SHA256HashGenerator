using System.IO;
namespace SHA256HashGenerator.BlockReaders
{
    public abstract class BlockReader
    {
        public const int LIMIT_DATA_IN_ONE_BLOCK = 200097152;
        protected readonly Stream stream;
        protected readonly int blockSize;
        public BlockReader(Stream stream, int blockSize)
        {
            this.stream = stream;
            this.blockSize = blockSize;
        }
        public static Stream GetInputStream(string inputFile)
        {
            if (!File.Exists(inputFile))
            {
                throw new FileNotFoundException("Файл не найден", inputFile);
            }

            return File.Open(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public static void DisposeStream(Stream stream)
        {
            if (stream != null)
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}