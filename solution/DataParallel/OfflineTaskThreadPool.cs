using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataParallel
{
    public class OfflineTaskThreadPool
    {
        public static async void ProcessServer()
        {
            await Task.Delay(0);
            ProcessQueue<int> processQueue = new();
            while (true)
            {
                processQueue.ProcessExceptionEvent += ProcessQueueProcessExceptionEvent;
                processQueue.ProcessItemEvent += ProcessQueueProcessItemEvent;
                processQueue.Enqueue(1);
                processQueue.Enqueue(2);
                processQueue.Enqueue(3);
                Thread.Sleep(1500);
            }
        }

        /// <summary>
        /// 该方法对入队的每个元素进行处理
        /// </summary>
        /// <param name="value"></param>
        private static void ProcessQueueProcessItemEvent(int value) { Console.WriteLine(value); }

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="obj">队列实例</param>
        /// <param name="e">异常对象</param>
        /// <param name="value">出错的数据</param>
        private static void ProcessQueueProcessExceptionEvent(dynamic obj, Exception e, int value) { Console.WriteLine(e.ToString()); }
    }
}
