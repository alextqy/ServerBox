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

    public class OfflineTask
    {
        public bool MissionComplete { set; get; }
        public FileLogic _fileLogic { set; get; }
        public Entity.OfflineTaskEntity _offlineTaskEntity { set; get; }
        public string Mark { set; get; }
        public OfflineTask()
        {
            this.MissionComplete = false;
            var DbContent = new ConfigHelper.DBHelper();
            this._fileLogic = new(Tools.LocalIP(), DbContent.EnvironmentDbContent);
            var CheckData = _fileLogic.CheckOfflineTaskAhead();
            this._offlineTaskEntity = CheckData.State == true ? CheckData.Data : null;
            this.Mark = this._offlineTaskEntity == null ? "" : Tools.MD5(this._offlineTaskEntity.URL);
            this.LockTask();
        }

        public void LockTask()
        {
            _fileLogic.SetOfflineTaskState(this._offlineTaskEntity.ID, 2);
        }

        public void PerformTask()
        {
        }
    }

    public class TaskHelper
    {
        private readonly QueueHandler<OfflineTask> _queueHandler = new();
        public TaskHelper() { this._queueHandler = new(); }

        #region 生产者/消费者 模式
        public void ProducerRun()
        {
            while (true)
            {
                Thread.Sleep(500);
                OfflineTask _offlineTask = new();
                if (_offlineTask._offlineTaskEntity == null) { continue; }
                if (this.DataExists(_offlineTask)) { continue; }
                this._queueHandler.Enqueue(_offlineTask);
                this._queueHandler.Refresh();
            }
        }

        public void ConsumerRun()
        {
            while (true)
            {
                Thread.Sleep(1500);
                var ItemArr = this._queueHandler._queue.ToArray();
                //Console.WriteLine(ItemArr.Length);
                foreach (var Item in ItemArr)
                {
                    //Item.PerformTask();
                    //if (Item.MissionComplete)
                    //{
                    this._queueHandler.TryDequeue(Item);
                    //}
                }
            }
        }
        #endregion

        // 去重
        public bool DataExists(OfflineTask Item)
        {
            var ItemArr = this._queueHandler._queue.ToArray();
            foreach (var item in ItemArr)
            {
                if (item.Mark == Item.Mark) { return true; }
            }
            return false;
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

        private static TaskHelper _taskHelper = new();

        public static async void ProducerProcessServer()
        {
            await Task.Delay(0);
            try
            {
                _taskHelper.ProducerRun();
            }
            catch (Exception e)
            {
                Tools.WarningConsole(e.Message);
            }
        }

        public static async void ConsumerProcessServer()
        {
            await Task.Delay(0);
            try
            {
                _taskHelper.ConsumerRun();
            }
            catch (Exception e)
            {
                Tools.WarningConsole(e.Message);
            }
        }
    }
}
