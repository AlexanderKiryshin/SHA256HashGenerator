using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SHA256HashGenerator
{
    /// <summary>
    /// Хранит элементы заданного типа и обеспечивает потокобезопасность при их добавлении/взятии
    /// </summary>
    /// <typeparam name="T">тип хранимого элемента</typeparam>
    public class Buffer<T>
    {
        private readonly object lockObject;
        private readonly Queue<T> input;
        private readonly EventWaitHandle addItemWaitHandler = new EventWaitHandle(false, EventResetMode.AutoReset);
        private readonly EventWaitHandle getItemWaitHandler = new EventWaitHandle(false, EventResetMode.AutoReset);
        private readonly int maxBufferSize;
        public Buffer(int maxBufferSize)
        {
            lockObject = new object();
            input = new Queue<T>();
            this.maxBufferSize = maxBufferSize;
        }
        /// <summary>
        /// Добавить элемент
        /// </summary>
        /// <param name="item">элемент</param>
        public void AddItem(T item)
        {
            bool isFullQueue;
            lock (lockObject)
            {
                isFullQueue = input.Count >= maxBufferSize;
            }

            if (isFullQueue)
                addItemWaitHandler.WaitOne();

            lock (lockObject)
            {
                input.Enqueue(item);
            }
            getItemWaitHandler.Set();
        }
        /// <summary>
        /// Взять первый положенный в буфер элемент
        /// </summary>
        public T GetItem()
        {
            while (true)
            {
                T result = default(T);
                lock (lockObject)
                {
                    if (input.Count > 0)
                    {
                        result = input.Dequeue();
                        addItemWaitHandler.Set();
                        return result;
                    }                
                }
                 getItemWaitHandler.WaitOne();
            }
        }
    }
}
