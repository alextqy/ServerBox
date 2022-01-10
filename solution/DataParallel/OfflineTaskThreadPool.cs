using Logic;
using Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
        public Entity.OfflineTaskEntity _offlineTaskEntity = null;
        public string Mark { set; get; }
        public OfflineTask()
        {
            this.MissionComplete = false;
            var DbContent = new ConfigHelper.DBHelper();
            this._fileLogic = new(Tools.LocalIP(), DbContent.EnvironmentDbContent);
            var CheckData = this._fileLogic.CheckOfflineTaskAhead();
            this._offlineTaskEntity = CheckData.State == true ? CheckData.Data : null;
            this.Mark = this._offlineTaskEntity == null ? "" : Tools.MD5(this._offlineTaskEntity.URL);
        }

        public void LockTask()
        {
            this._fileLogic.SetOfflineTaskState(this._offlineTaskEntity.ID, 2);
        }

        public void UnlockTask(int State = 1)
        {
            this.MissionComplete = true;
            this._fileLogic.SetOfflineTaskState(this._offlineTaskEntity.ID, State);
        }

        public void ProcessFile()
        {
            if (this._offlineTaskEntity == null)
            {
                this.UnlockTask();
            }
            else
            {
                Entity.UserEntity UserInfo = this._fileLogic.CheckTaskUserInfo(this._offlineTaskEntity.UserID).Data;
                if (UserInfo.ID == 0)
                {
                    this.UnlockTask();
                }
                else
                {
                    var UserBasePath = Tools.UserBaseDir() + UserInfo.Account;
                    if (!Tools.DirIsExists(UserBasePath))
                    {
                        this.UnlockTask();
                    }
                    else
                    {
                        var TaskDir = UserBasePath + "/" + Tools.MD5(this._offlineTaskEntity.URL);
                        if (this.HttpDownload(this._offlineTaskEntity.URL, TaskDir)) { this.UnlockTask(3); }
                    }
                }
            }
        }

        internal bool HttpDownload(string URL, string SavePath)
        {
            Directory.CreateDirectory(SavePath); // 创建临时文件目录
            string TempFile = SavePath + "/" + Path.GetFileName("TempFile"); // 临时文件
            if (Tools.FileIsExists(TempFile))
            {
                Tools.DelFile(TempFile); // 存在则删除
            }
            if (!Tools.DirIsExists(SavePath))
            {
                if (!Tools.CreateDir(SavePath)) { return false; }
            }
            try
            {
                FileStream FS = new(TempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                HttpWebRequest Request = WebRequest.Create(URL) as HttpWebRequest; // 设置参数
                HttpWebResponse Response = Request.GetResponse() as HttpWebResponse; // 发送Post请求并获取相应回应数据
                Stream ResponseStream = Response.GetResponseStream();
                // Stream FS = new FileStream(tempFile, FileMode.Create); // 创建本地文件写入流
                byte[] Buffer = new byte[1024];
                var Size = ResponseStream.Read(Buffer, 0, Buffer.Length);
                var SizeCur = 1024;
                while (Size > 0)
                {
                    // FS.Write(bArr, 0, size);
                    FS.Write(Buffer, 0, Size);
                    Size = ResponseStream.Read(Buffer, 0, Buffer.Length);
                    SizeCur += Size;
                }
                // FS.Close();
                FS.Close();
                ResponseStream.Close();
                var FileType = Response.ContentType.Split("/")[^1].Trim();
                var FileSize = Response.ContentLength;
                var FileNewName = Tools.CheckHttpFileName(Response);
                if (String.IsNullOrEmpty(FileNewName))
                {
                    FileNewName = "DownloadFile_" + Tools.TimeMS().ToString() + "." + FileType;
                }
                if (FileSize == SizeCur)
                {
                    if (Tools.RenameFile(TempFile, FileNewName))
                    {
                        if (this.FollowUpOperation(Path.Combine(Path.GetDirectoryName(TempFile), FileNewName))) { return true; }
                        else { return false; }
                    }
                    else { return false; }
                }
                else { return false; }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        internal bool FollowUpOperation(string FilePath)
        {
            if (String.IsNullOrEmpty(FilePath))
            {
                return false;
            }
            if (!Tools.FileIsExists(FilePath))
            {
                return false;
            }
            else
            {
                // Console.WriteLine(Path.GetDirectoryName(FilePath));
                // Console.WriteLine(FilePath);
                var ConfigInfo = this._fileLogic.CheckFileBlockSize();
                if (!ConfigInfo.State) { return false; }
                else
                {
                    int FileBlockSize = Convert.ToInt32(ConfigInfo.Data);
                    Console.WriteLine(FileBlockSize);
                }
                return true;
            }
        }
    }

    public class TaskHelper
    {
        private readonly QueueHandler<OfflineTask> _queueHandler;
        public TaskHelper() { this._queueHandler = new(2); }

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
                _offlineTask.LockTask();
                this._queueHandler.Refresh();
            }
        }

        public async void PerformTask()
        {
            while (true)
            {
                Thread.Sleep(500);
                var ItemArr = this._queueHandler._queue.ToArray();
                foreach (var Item in ItemArr)
                {
                    Thread.Sleep(500);
                    await Task.Factory.StartNew(() => Item.ProcessFile(), TaskCreationOptions.LongRunning);
                }
            }
        }

        public void ConsumerRun()
        {
            while (true)
            {
                Thread.Sleep(500);
                var ItemArr = this._queueHandler._queue.ToArray();
                if (ItemArr.Length > 0)
                {
                    foreach (var Item in ItemArr)
                    {
                        if (Item.MissionComplete)
                        {
                            this._queueHandler.TryDequeue(Item);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    continue;
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

        public static async void PerformTaskProcessServer()
        {
            await Task.Delay(0);
            try
            {
                _taskHelper.PerformTask();
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
