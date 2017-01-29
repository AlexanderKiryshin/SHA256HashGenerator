using SHA256HashGenerator.Blocks;
using System;
using System.Threading;
using SHA256HashGenerator.DataClass;

namespace SHA256HashGenerator.BlockHandlers
{
    /// <summary>
    /// Вычисляет хеши  блоков
    /// </summary>
    public class FullBlockHandler : BlockHandler
    {
        private readonly Buffer<FullBlock> input;
        Action<Exception> callback;

        /// <param name="threadsCount">количество потоков</param>
        /// <param name="callback">метод который вызывать в случае ошибки</param>
        public  FullBlockHandler(int threadsCount, Action<Exception> callback)
	 :base(threadsCount)
        {         
            input = new Buffer<FullBlock>(threadsCount * 2);
            for (int i = 0; i < threadsCount; i++)
            {
                workers[i] = new Thread(() => Run());
                workers[i].Priority = ThreadPriority.Lowest;
            }
            this.callback = callback;
        }
        protected override void Run()
        {
            SHA256 sha256Calculator = new SHA256();
            while (true)
            {
                FullBlock block = null;
                try
                {
                    block = input.GetItem();
                    if (block != null)
                    {
                        byte[] hash = sha256Calculator.CalculateSha256(block);
                        OutputConsole.DisplayBlockHash(block.Id+1, hash);
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
        public override void Stop()
        {
                for (int i = 0; i < workers.Length; i++)
                    input.AddItem(null);
            foreach (var w in workers)
                {
                    w.Join();
                }
        }
        public override void AddBlock(Block block)
        {
            input.AddItem((FullBlock)block);
        }
    }
}
