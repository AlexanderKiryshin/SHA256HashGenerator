using SHA256HashGenerator.BlockHandlers;
using SHA256HashGenerator.BlockReaders;
using SHA256HashGenerator.DataClass;
using System;
using System.IO;
using System.Threading;

namespace SHA256HashGenerator.Processors
{
    /// <summary>
    /// ”правл€ет процессом чтени€ и вычислени€ хеша дл€ блоков с их дополнительным делением на части
    /// </summary>
    internal class PartBlocksProcessor : AbstractProcessor
    {
        private readonly int processorCount = Environment.ProcessorCount;
        private int partBlockSize;
        private int partBlocksCount;
        private int blocksCount;
        private int blocksReaded;
        public PartBlocksProcessor(Stream inputStream, int blockSize, int partBlockSize)
            : base(inputStream, blockSize, OperationType.CalculateWithParts)
        {
            this.partBlockSize = partBlockSize;
            blocksCount = (int)Math.Ceiling((double)inputStream.Length / blockSize);
            partBlocksCount = (int)Math.Ceiling((double)blockSize / partBlockSize);
        }
        protected override void Initialization()
        {
            blockReader = new PartBlockReader(inputStream, blockSize, partBlockSize);
            blocksHandler = new PartBlockHandler(processorCount, ex => Callback(ex));
        }

        protected override void Produce()
        {
            long streamLength = inputStream.Length;
            int uncalculatedBlocks = blocksCount;
            long butesReaded = 0;
            while (streamLength > butesReaded)
            {
                try
                {
                    int nextReadedBlocks;
                    nextReadedBlocks=(uncalculatedBlocks >= processorCount) ?
                        processorCount: uncalculatedBlocks;
                    for (int i = 0; i < partBlocksCount; i++)
                    {
                        for (int j = blocksReaded; j < blocksReaded+nextReadedBlocks; j++)
                        {
                            if ((long)j * blockSize + i * partBlockSize<streamLength)
                            {
                                Block nextBlock = ((PartBlockReader)blockReader).GetPartBlock(j, i);
                                butesReaded += ((PartBlock)nextBlock).Size;
                                blocksHandler.AddBlock(nextBlock);
                            }
                        }
                    }
                    blocksReaded += nextReadedBlocks;
                    uncalculatedBlocks-=nextReadedBlocks;
                }                
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    isAborted = true;
                    OutputConsole.DisplayError(ex);
                    return;
                }
            }
        }
    }
}