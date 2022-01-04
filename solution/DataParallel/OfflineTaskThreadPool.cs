using Service;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataParallel
{
    public class TaskProcessing
    {
        public string Content { set; get; }
        public TaskProcessing(string Content)
        {
            this.Content = Content;
        }
    }

    public class OfflineTaskThreadPool
    {
        public static async void ProcessServer()
        {
            //Tools.CorrectConsole("Process service is running!");
            await Task.Delay(0);
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

            var ProcessCount = Tools.EP();
            Queue<TaskProcessing> OfflineTaskQueue = new();
            while (true)
            {
                Thread.Sleep(1000);
                if (ProcessCount > 0)
                {
                    if (OfflineTaskQueue.Count >= ProcessCount)
                    {
                        continue;
                    }
                    else
                    {
                        // 入队列
                        //new Task
                    }
                }
                else
                {
                    continue;
                }
            }
        }
    }
}
