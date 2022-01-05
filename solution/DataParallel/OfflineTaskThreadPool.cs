using Logic;
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

    public class ConsumeHelper
    {
        public bool MissionComplete { set; get; }
        public FileLogic _fileLogic { set; get; }
        public ConsumeHelper()
        {
            this.MissionComplete = false;
            var DbContent = new ConfigHelper.DBHelper();
            this._fileLogic = new(Tools.LocalIP(), DbContent.EnvironmentDbContent);
        }

        public static void Consume()
        {
            Thread.Sleep(500);
            Console.WriteLine("done");
        }
    }

    public class ProducerHelper<ConsumeHelper> : Producer<ConsumeHelper>
    {
        public ProducerHelper(ConsumeHelper Item) : base(Item) => this.Enqueue(Item);
    }

    public class ConsumerHelper<ConsumeHelper> : Consumer<ConsumeHelper>
    {
        public new dynamic Item { set; get; }
        public ConsumerHelper() : base() => this.Item = this.Dequeue();
        public void Consume()
        {
            this.Item.Consume();
        }
    }

    public class OfflineTaskThreadPool
    {
        //public static void Run()
        //{
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
        //}

        public static async void Test()
        {
            await Task.Delay(0);
            ConsumeHelper.Consume();
        }

        public static async void ProducerProcessServer()
        {
            await Task.Delay(0);
            ProducerRun();
        }

        public static async void ConsumerProcessServer()
        {
            await Task.Delay(0);
            ConsumerRun();
        }

        #region 生产者 消费者 模式
        public static void ProducerRun()
        {
            //Console.WriteLine("ProducerRun");
        }

        public static void ConsumerRun()
        {
            //Console.WriteLine("ConsumerRun");
        }
        #endregion
    }
}
