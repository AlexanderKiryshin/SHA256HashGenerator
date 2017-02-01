using SHA256HashGenerator.DataClass;
using System;
using System.IO;
namespace SHA256HashGenerator.BlockReaders
{
    /// <summary>
    /// Читает данные из заданного потока и возвращает их 
    /// в виде блоков разделеных на части заданного размера
    /// </summary>
    public class PartBlockReader : BlockReader
    {
        private readonly int partBlockSize;
        private bool bBlockIsOver;
        public PartBlockReader(Stream stream, int blockSize, int partBlockSize)
        : base(stream, blockSize)
        {
            this.partBlockSize = partBlockSize;
        }

        private byte[] GetPartBlockBytes(int fullBlockId, int partBlockId)
        {
            try
            {
                int bytesReaded;
                int readedByteBlock = partBlockId * partBlockSize;
                long length = stream.Length;
                bBlockIsOver = readedByteBlock + partBlockSize > blockSize;
                bool bIncorrectFullBlockId = (long)fullBlockId * blockSize + 
                    readedByteBlock - stream.Length >= partBlockSize;
                if (bIncorrectFullBlockId)
                 {
                    throw new ArgumentException();
                }
                int nextReadedBytes;
                if (readedByteBlock < blockSize - partBlockSize)
                {
                    nextReadedBytes = partBlockSize;
                }
                else
                {
                    if (readedByteBlock > blockSize)
                    {
                        throw new ArgumentException();
                    }
                    else
                    {
                        nextReadedBytes = blockSize - readedByteBlock;
                        bBlockIsOver = true;
                    }
                }
                byte[] buffer = new byte[nextReadedBytes];
                stream.Position =(long)fullBlockId * blockSize + readedByteBlock;
                bytesReaded = stream.Read(buffer, 0, nextReadedBytes);
                if (bytesReaded == 0)
                    return null;
                if (bytesReaded == partBlockSize)
                    return buffer;
                bBlockIsOver = true;
                byte[] result = new byte[bytesReaded];
                Array.Copy(buffer, result, result.Length);
                return result;
            }
            catch (ArgumentException ex)
            {
                OutputConsole.DisplayError(ex);
                return null;
            }
        }
        public Block GetPartBlock(int fullBlockId, int partBlockId)
        {
            var buffer = GetPartBlockBytes(fullBlockId,partBlockId);
            PartBlock block = new PartBlock();

            if (bBlockIsOver)
            {
                block.SizeFullBlock = partBlockId * partBlockSize + buffer.Length;
            }
            
            block.Id = partBlockId;
            block.Size = buffer.Length;
            block.Data = buffer;
            block.IdFullBlock = fullBlockId;
            return block;
        }
    }
}