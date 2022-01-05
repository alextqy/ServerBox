using Service;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace DataParallel
{
    public class Base { }

    /// <summary>
    /// 队列脚手架
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueueHandler<T> : Base
    {
        private Queue<T> _queue = new();
        private int _queueSize { set; get; }
        private bool _shutdown { set; get; }
        public QueueHandler() { this._queueSize = Tools.EP(); }

        public void Enqueue(T Item)
        {
            this._queue.TrimExcess();
            lock (this._queue)
            {
                while (this._queue.Count >= this._queueSize)
                {
                    Monitor.Wait(this._queue);
                }
                this._queue.Enqueue(Item);
                if (this._queue.Count == 1)
                {
                    Monitor.PulseAll(this._queue);
                }
            }
        }

        // 直接出队列
        public T Dequeue()
        {
            this._queue.TrimExcess();
            lock (this._queue)
            {
                while (this._queue.Count == 0)
                {
                    Monitor.Wait(this._queue);
                }
                T Item = this._queue.Dequeue();
                if (this._queue.Count == this._queueSize - 1)
                {
                    Monitor.PulseAll(this._queue);
                }
                return Item;
            }
        }

        // 获取出列对象
        public bool TryDequeue(out T Item)
        {
            this._queue.TrimExcess();
            lock (this._queue)
            {
                while (this._queue.Count == 0)
                {
                    if (this._shutdown)
                    {
                        Item = default;
                        return false;
                    }
                    Monitor.Wait(this._queue);
                }
                Item = this._queue.Dequeue();
                if (this._queue.Count == this._queueSize - 1)
                {
                    Monitor.PulseAll(this._queue);
                }
                return true;
            }
        }

        // 结束释放
        public void Dispose()
        {
            lock (this._queue)
            {
                this._shutdown = true;
                Monitor.PulseAll(this._queue);
            }
        }
    }

    /// <summary>
    /// 生产者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Producer<T> : QueueHandler<T>
    {
        public Producer(T Item) { this.Enqueue(Item); }
    }

    /// <summary>
    /// 消费者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Consumer<T> : QueueHandler<T>
    {
        public T Item { set; get; }
        public Consumer() { this.Item = this.Dequeue(); }
    }

}
