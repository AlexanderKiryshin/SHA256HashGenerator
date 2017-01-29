using SHA256HashGenerator.DataClass;
using System.Threading;

namespace SHA256HashGenerator.Blocks
{
    /// <summary>
    /// Вычисляет хеши  блоков
    /// </summary>
    public abstract class BlockHandler
    {
        protected int threadsCount;
        protected readonly Thread[] workers;

        /// <param name="threadsCount">Количество потоков</param>
        public BlockHandler(int threadsCount)
        {
            workers = new Thread[threadsCount];
            this.threadsCount = threadsCount;
        }

        public void Start()
        {
            for (int i = 0; i < workers.Length; i++)
            {
                workers[i].IsBackground = true;
                workers[i].Start();
            }
        }
        protected abstract void Run();
        public abstract void Stop();

        public void Abort()
        {
            for (int i = 0; i < workers.Length; i++)
            {
                workers[i].Abort();
            }
        }
        public abstract void AddBlock(Block block);
    }    
}

