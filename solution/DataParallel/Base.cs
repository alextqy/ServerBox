using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public Queue<T> _queue = new();
        public int _queueSize { set; get; }
        public bool _shutdown { set; get; }
        public QueueHandler(int QueueSize = 0) { this._queueSize = QueueSize > 0 ? QueueSize : Tools.EP(); }

        public void Enqueue(T Item)
        {
            this._queue.TrimExcess();
            if (Item == null) { return; }
            lock (this._queue)
            {
                if (this._queue.Count == this._queueSize)
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
        public bool TryDequeue(T Item)
        {
            this._queue.TrimExcess();
            lock (this._queue)
            {
                if (this._queue.Count == 0)
                {
                    if (this._shutdown)
                    {
                        Item = default;
                        return false;
                    }
                    Monitor.Wait(this._queue);
                }
                Item = this._queue.Dequeue();
                if (this._queue.Count == 0)
                {
                    return true;
                }
                if (this._queue.Count >= this._queueSize)
                {
                    Monitor.PulseAll(this._queue);
                }
                return true;
            }
        }

        // 获取出列对象
        public T Dequeue()
        {
            this._queue.TrimExcess();
            lock (this._queue)
            {
                while (_queue.Count == 0)
                {
                    Monitor.Wait(this._queue);
                }
                T Item = this._queue.Dequeue();
                if (this._queue.Count >= this._queueSize)
                {
                    Monitor.PulseAll(this._queue);
                }
                return Item;
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

        // 刷新
        public void Refresh()
        {
            this._queue.TrimExcess();
        }

        // 统计
        public int Count()
        {
            return this._queue.Count;
        }
    }

    /// <summary>
    /// 生产者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Producer<T>
    {
        public QueueHandler<T> QueueContainer = new();

        public Producer(QueueHandler<T> queue) { this.QueueContainer = queue; }

        public void Produce(T Item) { this.QueueContainer.Enqueue(Item); }
    }

    /// <summary>
    /// 消费者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Consumer<T>
    {
        public QueueHandler<T> QueueContainer = new();

        public Consumer(QueueHandler<T> queue) { this.QueueContainer = queue; }

        public T Consume() { return this.QueueContainer.Dequeue(); }
    }

}
