using SHA256HashGenerator.DataClass;
using System;
using System.IO;
namespace SHA256HashGenerator.BlockReaders
{
    /// <summary>
    ///
    /// </summary>
    public class FullBlockReader : BlockReader
    {
        private int blockCounter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream">Входной поток с которого читать</param>
        /// <param name="blockSize">Размер читаемого блока в байтах</param>
        public FullBlockReader(Stream stream, int blockSize)
		:base(stream,blockSize)
        {
        }

        public byte[] GetNextBlockBytes()
        {
            byte[] buffer = new byte[blockSize];
            int bytesReaded = stream.Read(buffer, 0, blockSize);

            if (bytesReaded == 0)
                return null;

            if (bytesReaded == blockSize)
                return buffer;

            byte[] result = new byte[bytesReaded];
            Array.Copy(buffer, result, result.Length);

            return result;
        }

        public Block GetNextBlock()
        {
            var buffer = GetNextBlockBytes();
            if (buffer != null)
            {
                Block block = new FullBlock
                {
                    Id = blockCounter++,
                    Size = buffer.Length,
                    Data = buffer
                };
                return block;
            }
            return null;
        }
    }
}
