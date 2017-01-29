using SHA256HashGenerator.BlockReaders;
using SHA256HashGenerator.Blocks;
using System;
using System.IO;
using System.Threading;

namespace SHA256HashGenerator.Processors
{
   /// <summary>
   /// Управляет процессом чтения и вычисления хеша для блоков
   /// </summary>
   internal abstract class AbstractProcessor
    {
        protected int blockSize;
        protected readonly Stream inputStream;
        private bool isRun;
        protected bool isAborted;
        protected BlockReader blockReader;
        protected BlockHandler blocksHandler;
        protected OperationType operationType;
        private Thread producerThread;

        public AbstractProcessor(Stream inputStream, int blockSize, OperationType operationType)
        {
            this.inputStream = inputStream;
            this.blockSize = blockSize;
            this.operationType = operationType;
        }
        protected abstract void Initialization();

        protected abstract void Produce();

        protected void Process()
        {
            try
            {
                producerThread = new Thread(Produce);
                producerThread.Priority = ThreadPriority.AboveNormal;

                producerThread.Start();
                blocksHandler.Start();

                producerThread.Join();
                if (!isAborted)blocksHandler.Stop();
            }
            catch (Exception ex)
            {
                OutputConsole.DisplayError(ex);
            }
        }
        protected void Callback(Exception ex)
        {
            OutputConsole.DisplayError(ex);
            Abort();
        }
     
        public bool Run()
        {
            Initialization();

            Process();

            return !isAborted;
        }

        public void Abort()
        {
            isAborted = true;

            if (producerThread != null)
            {
                producerThread.Abort();
            }
            if (blocksHandler != null)
            {
                blocksHandler.Abort();
            }
        }
    }
}
