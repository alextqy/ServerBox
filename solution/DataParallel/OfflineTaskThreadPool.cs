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
        public UserLogic _userLogic { set; get; }
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
                this.UnlockTask(4);
            }
            else
            {
                Entity.UserEntity UserInfo = this._fileLogic.CheckTaskUserInfo(this._offlineTaskEntity.UserID).Data;
                if (UserInfo.ID == 0)
                {
                    this.UnlockTask(4);
                }
                else
                {
                    var UserBasePath = Tools.UserBaseDir() + UserInfo.Account;
                    if (!Tools.DirIsExists(UserBasePath))
                    {
                        this.UnlockTask(4);
                    }
                    else
                    {
                        var TaskDir = UserBasePath + "/" + Tools.MD5(this._offlineTaskEntity.URL);
                        if (this.HttpDownload(this._offlineTaskEntity.URL, TaskDir, UserInfo.ID))
                        {
                            this.UnlockTask(3);
                        }
                        else
                        {
                            Tools.DelDir(TaskDir, true);
                            this.UnlockTask(4);
                        }
                    }
                }
            }
        }

        internal bool HttpDownload(string URL, string SavePath, int UserID)
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
                var FileType = Response.ContentType.Split("/")[^1].Trim();
                var FileSize = Response.ContentLength;
                var FileNewName = Tools.CheckHttpFileName(Response);

                // 该用户根目录是否已经存在同名文件
                Entity.DirEntity UserRootDir = this._fileLogic.CheckUserRootDir(UserID).Data;
                if (UserRootDir.ID == 0) { return false; }
                if (this._fileLogic.CheckOfflineTaskFileExist(UserRootDir.ID, FileNewName[..FileNewName.LastIndexOf(".")]).Data.ID > 0) { return false; }

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
                if (String.IsNullOrEmpty(FileNewName))
                {
                    FileNewName = "DownloadFile_" + Tools.TimeMS().ToString() + "." + FileType;
                }
                if (FileSize == SizeCur)
                {
                    if (Tools.RenameFile(TempFile, FileNewName))
                    {
                        if (this.CreateFileOperation(Path.Combine(Path.GetDirectoryName(TempFile), FileNewName))) { return true; }
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

        internal bool CreateFileOperation(string FilePath)
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
                var ConfigInfo = this._fileLogic.CheckFileBlockSize();
                if (!ConfigInfo.State) { return false; }
                else
                {
                    var OperationTime = Tools.TimeMS();
                    var FileDirName = Path.GetDirectoryName(FilePath); // 文件所在文件夹
                    var FileName = Tools.FileName(FilePath); // 文件本名
                    var TargetPath = Path.GetDirectoryName(FileDirName) + "/" + FileName + "." + OperationTime.ToString(); // 目标文件夹
                    if (Tools.DirIsExists(TargetPath))
                    {
                        if (!Tools.ClearDir(TargetPath)) { return false; }
                    }
                    else
                    {
                        if (!Tools.CreateDir(TargetPath)) { return false; }
                    }
                    int BlockSize = Convert.ToInt32(ConfigInfo.Data); // 切片大小
                    var UserAccount = Path.GetDirectoryName(FileDirName).Replace(@"\", "/").Split("/")[^1]; // 用户目录名称
                    var FileMD5 = Tools.FileMD5(FilePath).ToLower();
                    if (String.IsNullOrEmpty(FileMD5)) { return false; }
                    var FileSize = Tools.FileInfo(FilePath).Length;

                    Entity.UserEntity UserInfo = this._fileLogic.CheckUserByAccount(UserAccount).Data;
                    if (UserInfo.ID == 0) { return false; }
                    Entity.DirEntity UserRootDir = this._fileLogic.CheckUserRootDir(UserInfo.ID).Data;
                    if (UserRootDir.ID == 0) { return false; }

                    if (Tools.FileSlice(FilePath, TargetPath, BlockSize))
                    {
                        // 新建文件
                        Entity.FileEntity FileData = new();
                        FileData.FileName = FileName;
                        FileData.UserID = UserInfo.ID;
                        FileData.Createtime = OperationTime;
                        FileData.FileType = Tools.FileType(FilePath).Replace(".", "");
                        FileData.State = 1;
                        FileData.FileSize = FileSize.ToString();
                        var BlockSizeDecimal = Convert.ToDecimal(Convert.ToDouble(FileSize) / Convert.ToDouble(BlockSize));
                        FileData.BlockSize = (int)Math.Ceiling(BlockSizeDecimal);
                        FileData.UploadBlockSize = (int)Math.Ceiling(BlockSizeDecimal);
                        FileData.ServerStoragePath = TargetPath.Replace("\\", "/");
                        FileData.UploadPath = "offline_download";
                        FileData.DirID = UserRootDir.ID;
                        FileData.MD5 = FileMD5;
                        if (!this._fileLogic.CreateOfflineTaskFile(FileData).State)
                        {
                            Tools.DelDir(TargetPath, true);
                            return false;
                        }
                        Tools.DelDir(FileDirName, true);
                        return true;
                    }
                    else
                    {
                        Tools.DelDir(TargetPath, true);
                        return false;
                    }
                }
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
