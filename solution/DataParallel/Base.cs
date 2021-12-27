using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace DataParallel
{
    public class Base { }

    public class ProcessQueue<T> : Base
    {
        private BlockingCollection<T> _queue { set; get; }
        private CancellationTokenSource _cancellationTokenSource { set; get; }
        private CancellationToken _cancellToken { set; get; }
        private List<Thread> _threadCollection { set; get; } // 内部线程池
        private int _isProcessing; // 队列是否正在处理数据
        private const int Processing = 1; // 有线程正在处理数据
        private const int UnProcessing = 0; // 没有线程处理数据
        private volatile bool _enabled = true; // 队列是否可用
        private int _internalThreadCount; // 内部处理线程数量
        public event Action<T> ProcessItemEvent;
        public event Action<dynamic, Exception, T> ProcessExceptionEvent; // 处理异常，需要三个参数，当前队列实例，异常，当时处理的数据

        public ProcessQueue()
        {
            _queue = new BlockingCollection<T>();
            _cancellationTokenSource = new CancellationTokenSource();
            _internalThreadCount = 1;
            _cancellToken = _cancellationTokenSource.Token;
            _threadCollection = new List<Thread>();
        }

        public ProcessQueue(int internalThreadCount) : this() { this._internalThreadCount = internalThreadCount; }
        public int GetInternalItemCount() { return _queue.Count; } // 队列内部元素的数量

        /// <summary>
        /// 入队列
        /// </summary>
        /// <param name="Items"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Enqueue(T Items)
        {
            if (Items == null) { throw new ArgumentException(null, nameof(Items)); }
            _queue.Add(Items);
            DataAdded();
        }

        public void Flush()
        {
            StopProcess();

            while (_queue.Count != 0)
            {
                if (_queue.TryTake(out T Item))
                {
                    try { ProcessItemEvent(Item); }
                    catch (Exception e) { OnProcessException(e, Item); }
                }
            }
        }

        private void DataAdded()
        {
            if (_enabled)
            {
                if (!IsProcessingItem())
                {
                    ProcessRangeItem();
                    StartProcess();
                }
            }
        }

        // 判断是否队列有线程正在处理 
        private bool IsProcessingItem() { return !(Interlocked.CompareExchange(ref _isProcessing, Processing, UnProcessing) == UnProcessing); }

        private void ProcessRangeItem() { for (int i = 0; i < this._internalThreadCount; i++) { ProcessItem(); } }

        private void ProcessItem()
        {
            Thread currentThread = new((state) =>
            {
                T Item = default;
                while (_enabled)
                {
                    try
                    {
                        try
                        {
                            Item = _queue.Take(_cancellToken);
                            ProcessItemEvent(Item);
                        }
                        catch (OperationCanceledException e)
                        {
                            Console.WriteLine(e.Message);
                        }

                    }
                    catch (Exception e)
                    {
                        OnProcessException(e, Item);
                    }
                }

            });
            _threadCollection.Add(currentThread);
        }

        private void StartProcess() { foreach (var thread in _threadCollection) { thread.Start(); } }

        private void StopProcess()
        {
            this._enabled = false;
            foreach (var thread in _threadCollection)
            {
                if (thread.IsAlive) { thread.Join(); }
            }
            _threadCollection.Clear();
        }

        private void OnProcessException(Exception ex, T item)
        {
            var tempException = ProcessExceptionEvent;
            Interlocked.CompareExchange(ref ProcessExceptionEvent, null, null);
            if (tempException != null)
            {
                ProcessExceptionEvent(this, ex, item);
            }
        }
    }
}
