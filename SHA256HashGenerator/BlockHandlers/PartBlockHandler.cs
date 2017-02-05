using SHA256HashGenerator.Blocks;
using SHA256HashGenerator.DataClass;
using System;
using System.Threading;

namespace SHA256HashGenerator.BlockHandlers
{
    /// <summary>
    /// Вычисляет хеши  блоков которые предварительно были разделены на части заданного размера
    /// </summary>
    public class PartBlockHandler : BlockHandler
    {
        private event Action<Exception> callback;
        private const int MAX_BUFFER_SIZE = 2;
        private readonly Buffer<PartBlock>[] input;
        /// <summary>
        /// хранит номера потоков которые потоки берут отсюда в начале работы
        /// </summary>
        private readonly Buffer<int> threadNumbers;
        private int calculatedPartBlocksCount;

        /// <param name="threadsCount">количество потоков</param>
        /// <param name="callback">метод который вызывать в случае ошибки</param>
        public PartBlockHandler(int threadsCount, Action<Exception> callback)
     : base(threadsCount)
        {
            input = new Buffer<PartBlock>[threadsCount];
            threadNumbers = new Buffer<int>(threadsCount);
            for (int i = 0; i < threadsCount; i++)
            {
                threadNumbers.AddItem(i);
                input[i] = new Buffer<PartBlock>(MAX_BUFFER_SIZE);
                workers[i] = new Thread(() => Run());
                workers[i].Priority = ThreadPriority.Lowest;
                this.callback = callback;
            }

        }
        protected override void Run()
        {
            SHA256 sha256Calculator = new SHA256();
            int numberThread = threadNumbers.GetItem();
            int[] size = new int[50];
            while (true)
            {
                PartBlock block = null;
                try
                {                                        
                    block = input[numberThread].GetItem(); 
                    if (block != null)
                    {                       
                        byte[] hash = sha256Calculator.CalculateSha256(block);
                        if (hash.Length != 0)
                        {
                                OutputConsole.DisplayBlockHash(block.IdFullBlock+1, hash);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    callback?.Invoke(ex);
                    return;
                }
            }
        }
        /// <summary>
        /// Добавляет блок в буфер
        /// </summary>
        /// <param name="block">блок который нужно добавить</param>
        public override void AddBlock(Block block)
        {
            try
            {
                int index = ((PartBlock)block).IdFullBlock % threadsCount;
                input[index].AddItem((PartBlock)block);
                calculatedPartBlocksCount++;
            }
            catch (ArgumentException ex)
            {
                OutputConsole.DisplayError(ex);
                return;
            }

        }
        public override void Stop()
        {
                for (int i = 0; i < workers.Length; i++)
                    for (int j = 0; j < MAX_BUFFER_SIZE; j++)
                    {
                        input[i].AddItem(null);
                    }
            foreach (var w in workers)
            {
                w.Join();
            }
        }
    }
}

