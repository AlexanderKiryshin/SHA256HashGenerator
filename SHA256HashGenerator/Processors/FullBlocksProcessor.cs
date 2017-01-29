using SHA256HashGenerator.BlockHandlers;
using SHA256HashGenerator.BlockReaders;
using SHA256HashGenerator.DataClass;
using System;
using System.IO;
using System.Threading;

namespace SHA256HashGenerator.Processors
{
    /// <summary>
    /// Управляет процессом чтения и вычисления хеша для блоков без дополнительно их деления
    /// </summary>
    internal class FullBlocksProcessor : AbstractProcessor
    {
        public FullBlocksProcessor(Stream inputStream, int blockSize)
            : base(inputStream, blockSize, OperationType.CalculateWithoutParts)
        { }
        protected override void Initialization()
        {
            blockReader = new FullBlockReader(inputStream, blockSize);
            blocksHandler = new FullBlockHandler(Environment.ProcessorCount, ex =>Callback(ex));
        }
       
        protected override void Produce()
        {
            long streamLength = inputStream.Length;

            while (streamLength - 1 > inputStream.Position)
            {
                try
                {
                    Block nextBlock = ((FullBlockReader)blockReader).GetNextBlock();
                    blocksHandler.AddBlock(nextBlock);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    isAborted = true;
                    OutputConsole.DisplayError(ex);
                    return;
                }
                catch (ThreadAbortException)
                {
                    return;
                }
            }
        }
    }
}