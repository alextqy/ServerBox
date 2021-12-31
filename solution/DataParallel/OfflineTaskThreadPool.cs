using Service;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataParallel
{
    //public class TaskProcessing
    //{
    //    public string Content { set; get; }
    //    public TaskProcessing(string Content)
    //    {
    //        this.Content = Content;
    //    }
    //}

    public class OfflineTaskThreadPool
    {
        public static async void ProcessServer()
        {
            await Task.Delay(0);
            Tools.CorrectConsole("Process service is running!");
            Run();
        }

        public static void Run()
        {
            //Queue<TaskProcessing> chatMsgs = new();
            //Console.WriteLine(chatMsgs.Count);
            //chatMsgs.Enqueue(new TaskProcessing("fuck"));
            //chatMsgs.Enqueue(new TaskProcessing("you"));
            //Console.WriteLine(chatMsgs.Count);
            //Thread.Sleep(500);
            //Console.WriteLine(chatMsgs.ToArray()[0]);
            //Console.WriteLine(chatMsgs.ToArray()[1]);
            //Task.Factory.StartNew(() =>
            //{
            //    while (chatMsgs.TryDequeue(out var chat)) { Console.WriteLine(chat.Content); Console.WriteLine("==="); }
            //    chatMsgs.TrimExcess();
            //});
            //Thread.Sleep(500);
            //Console.WriteLine(chatMsgs.Count);
        }

        //public static void Run()
        //{
        //ProcessQueue<int> _processQueue = new();
        //_processQueue.ProcessItemEvent += ProcessQueueProcessItemEvent;
        //_processQueue.ProcessExceptionEvent += ProcessQueueProcessExceptionEvent;
        //while (true)
        //{
        //Thread.Sleep(500);
        //Console.WriteLine(_processQueue._isProcessing);
        //_processQueue.Enqueue(3);
        //_processQueue.Enqueue(2);
        //_processQueue.Enqueue(1);
        //Thread.Sleep(500);
        //_processQueue.Dequeue(3);
        //_processQueue.Dequeue(2);
        //_processQueue.Dequeue(1);
        //Thread.Sleep(500);
        //Console.WriteLine(_processQueue._isProcessing);
        //}
        //}

        /// <summary>
        /// 该方法对入队的每个元素进行处理
        /// </summary>
        /// <param name="value"></param>
        //private static void ProcessQueueProcessItemEvent(int value) { Console.WriteLine(value); }

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="obj">队列实例</param>
        /// <param name="e">异常对象</param>
        /// <param name="value">出错的数据</param>
        //private static void ProcessQueueProcessExceptionEvent(dynamic obj, Exception e, int value) { Console.WriteLine(e.ToString()); }
    }
}
