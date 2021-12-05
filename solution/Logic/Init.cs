using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Service;

namespace Logic
{
    public class Init : Base
    {
        /// <summary>
        /// 项目启动
        /// </summary>
        /// <returns></returns>
        public static void Run()
        {
            string Profile = Tools.RootPath() + "Profile.json"; // 查看配置文件
            if (!File.Exists(Profile))
            {
                bool Step1 = Tools.CreateFile(Profile); // 创建配置文件
                if (!Step1)
                {
                    Console.WriteLine("ERROR: Failed to create configuration file!");
                    DelInitFile(); // 回到起始状态
                    Environment.Exit(0); // 程序终止
                    return;
                }

                // 写入初始化配置数据
                SystemProfile ProfileObject = new(); // 实例化配置项
                string JsonString = JsonTools.ProfileToString(ProfileObject); // 配置项转为json格式
                bool Step2 = JsonTools.StringToFile(Profile, JsonString); // 写入文件
                if (!Step2)
                {
                    Console.WriteLine("ERROR: Failed to modify configuration file!");
                    DelInitFile(); // 回到起始状态
                    Environment.Exit(0); // 程序终止
                    return;
                }
            }

            SystemProfile Config = SystemProfile.CheckConfig(); // 获取配置项实体
            if (Config.SystemInit) // 初始化判断
            {
                DelInitFile(); // 回到起始状态
                string BaseDir = Tools.RootPath() + "Matrix"; // 新建基础目录
                if (Tools.DirIsExists(BaseDir)) // 清理旧目录
                {
                    Tools.DelDir(BaseDir, true);
                }
                bool Step3 = Tools.CreateDir(BaseDir); // 创建目录
                if (!Step3)
                {
                    Console.WriteLine("ERROR: Failed to make base dir!");
                    DelInitFile(); // 回到起始状态
                    Environment.Exit(0); // 程序终止
                    return;
                }

                string DaoRoom = Tools.RootPath() + "DaoRoom.db"; // 数据库文件路径
                bool Step4 = Tools.CreateFile(DaoRoom); // 创建数据库文件
                if (!Step4)
                {
                    Console.WriteLine("ERROR: Failed to make database file!");
                    DelInitFile(); // 回到起始状态
                    Environment.Exit(0); // 程序终止
                    return;
                }

                // 连接数据库
                string ConString = "Data Source = " + DaoRoom;
                SqliteConnection SqliteObject = new(ConString);
                try
                {
                    SqliteObject.Open(); // 打开链接
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("ERROR: Failed to connect database!");
                    DelInitFile(); // 回到起始状态
                    Environment.Exit(0); // 程序终止
                    return;
                }

                // 初始化数据表
                try
                {
                    SqliteCommand InitDatabase = new(CreateTable(), SqliteObject); // 执行SQL
                    InitDatabase.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("ERROR: Failed to initialize database!");
                    SqliteObject.Close(); // 关闭链接
                    DelInitFile(); // 回到起始状态
                    Environment.Exit(0); // 程序终止
                    return;
                }

                // 初始化系统数据
                try
                {
                    int Secret = Tools.Random(5); // 获取随机数
                    string PWD = "123456"; // 设置管理员初始密码
                    string Password = Tools.UserPWD(PWD, Secret.ToString()); // 初始化生成管理员密码
                    string AdminSQL = "INSERT INTO user (Account, Name, Password, Secret, Status, Createtime, Admin, Avatar, Wallpaper, Permission, Master, DepartmentID) VALUES ('root', 'Admin', '" + Password + "', '" + Secret + "', 1, " + Tools.Time32().ToString() + ", 2, '', '', '1,2,3,4,5,6,7,8,9', 2, 0);"; // 写入数据
                    SqliteCommand InitAdminData = new(AdminSQL, SqliteObject); // 执行SQL
                    InitAdminData.ExecuteNonQuery();

                    string AdminDirSQL = "INSERT INTO Dir (DirName, ParentID, UserID, Createtime) VALUES ('root', 0, 1, " + Tools.Time32().ToString() + ");"; // 设置管理员文件夹
                    SqliteCommand InitAdminDirData = new(AdminDirSQL, SqliteObject); // 执行SQL
                    InitAdminDirData.ExecuteNonQuery();

                    // 初始化系统允许的文件类型
                    var ImgType = "jpg,jpeg,gif,png";
                    var CommonFileType = "txt,pdf,zip,torrent,rar,exe,md";
                    var OfficeType = "doc,docx,xls,xlsx,xlsm,ppt,pptx";
                    var VideoType = "mp4,avi,rmvb,rm,mkv";
                    var AudioType = "mp3,flac,ape";
                    var CADType = "dwf,dwg,skp";
                    var PCLangType = "php,py,java,go,asp,aspx,html,htm,xml,js";
                    var OtherType = "iso";
                    var TypeData = ImgType + "," + CommonFileType + "," + OfficeType + "," + VideoType + "," + AudioType + "," + CADType + "," + PCLangType + "," + OtherType;
                    string ConfigFileTypeSQL = "INSERT INTO Config (ConfigKey, ConfigDesc, ConfigType, ConfigValue) VALUES ('FileType', 'file type data', 0, '" + TypeData + "');"; // 写入数据
                    SqliteCommand InitConfigFileTypeData = new(ConfigFileTypeSQL, SqliteObject); // 执行SQL
                    InitConfigFileTypeData.ExecuteNonQuery();

                    // 设置文件分片体积
                    var BlockSize = 1024 * 1024 * 2; // bit
                    string ConfigBlockSizeSQL = "INSERT INTO Config (ConfigKey, ConfigDesc, ConfigType, ConfigValue) VALUES ('BlockSize', 'file block size data', 0, '" + BlockSize.ToString() + "');";
                    SqliteCommand InitConfigBlockSizeData = new(ConfigBlockSizeSQL, SqliteObject); // 执行SQL
                    InitConfigBlockSizeData.ExecuteNonQuery();

                    // 设置在线预览大小限制
                    var PreviewSizeLimit = 1024 * 1024 * 5;
                    string ConfigPreviewSizeLimitSQL = "INSERT INTO Config (ConfigKey, ConfigDesc, ConfigType, ConfigValue) VALUES ('PreviewSizeLimit', 'preview size limit', 0, '" + PreviewSizeLimit.ToString() + "');";
                    SqliteCommand InitConfigPreviewSizeLimitData = new(ConfigPreviewSizeLimitSQL, SqliteObject); // 执行SQL
                    InitConfigPreviewSizeLimitData.ExecuteNonQuery();

                    SqliteObject.Close(); // 关闭链接
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("ERROR: Failed to initialize admin data!");
                    SqliteObject.Close(); // 关闭链接
                    DelInitFile(); // 回到起始状态
                    Environment.Exit(0); // 程序终止
                    return;
                }

                // 新建管理员目录
                bool Step5 = Tools.CreateDir(BaseDir + "/root");
                if (!Step5)
                {
                    Console.WriteLine("ERROR: Failed to make admin dir!");
                    DelInitFile(); // 回到起始状态
                    Environment.Exit(0); // 程序终止
                    return;
                }

                // 修改配置文件
                Config.SystemInit = false; // 设置初始化完成状态
                string JsonString = JsonTools.ProfileToString(Config);
                bool Step6 = JsonTools.StringToFile(Profile, JsonString);
                if (!Step6)
                {
                    Console.WriteLine("ERROR: Failed to modify configuration file!");
                    DelInitFile(); // 回到起始状态
                    Environment.Exit(0); // 程序终止
                    return;
                }

                Console.WriteLine("System initialization completed!");
            }
            Console.WriteLine("========== Ver 1.0.0 beta ==========");
            Console.WriteLine("Bit Box is working!");
        }

        /// <summary>
        /// 删除配置文件和数据库文件
        /// </summary>
        public static void DelInitFile()
        {
            Tools.DelFile(Tools.RootPath() + "Profile.json");
            Tools.DelFile(Tools.RootPath() + "DaoRoom.db");
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <returns></returns>
        public static string CreateTable()
        {
            string SQLParam = "";

            // config
            SQLParam += "DROP TABLE IF EXISTS config;";
            SQLParam += "CREATE TABLE config(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "ConfigKey VARCHAR(128) NOT NULL," +
                "ConfigDesc VARCHAR(128) NOT NULL," +
                "ConfigType INT(1) NOT NULL," +
                "ConfigValue VARCHAR(65535) NOT NULL" +
                ");";

            // department_extra
            SQLParam += "DROP TABLE IF EXISTS department_extra;";
            SQLParam += "CREATE TABLE department_extra(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "DepartmentID INT(10) NOT NULL," +
                "ExtraDesc VARCHAR(128) NOT NULL," +
                "ExtraType INT(10) NOT NULL," +
                "ExtraValue VARCHAR(128) NOT NULL" +
                ");";

            // department
            SQLParam += "DROP TABLE IF EXISTS department;";
            SQLParam += "CREATE TABLE department(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "DepartmentName VARCHAR(128) NOT NULL," +
                "ParentID INT(10) NOT NULL," +
                "State INT(1) NOT NULL," +
                "Createtime INT(10) NOT NULL" +
                ");";

            // dir_extra
            SQLParam += "DROP TABLE IF EXISTS dir_extra;";
            SQLParam += "CREATE TABLE dir_extra(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "DirID INT(10) NOT NULL," +
                "ExtraDesc VARCHAR(128) NOT NULL," +
                "ExtraType INT(10) NOT NULL," +
                "ExtraValue INT(128) NOT NULL" +
                ");";

            // dir
            SQLParam += "DROP TABLE IF EXISTS dir;";
            SQLParam += "CREATE TABLE dir(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "DirName VARCHAR(128) NOT NULL," +
                "ParentID INT(10) NOT NULL," +
                "UserID INT(10) NOT NULL," +
                "Createtime INT(10) NOT NULL" +
                ");";

            // file_extra
            SQLParam += "DROP TABLE IF EXISTS file_extra;";
            SQLParam += "CREATE TABLE file_extra(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "FileID INT(10) NOT NULL," +
                "ExtraDesc VARCHAR(128) NOT NULL," +
                "ExtraType INT(10) NOT NULL," +
                "ExtraValue VARCHAR(128) NOT NULL" +
                ");";

            // file
            SQLParam += "DROP TABLE IF EXISTS file;";
            SQLParam += "CREATE TABLE file(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "FileName VARCHAR(128) NOT NULL," +
                "UserID INT(10) NOT NULL," +
                "Createtime INT(10) NOT NULL," +
                "FileType VARCHAR(128) NOT NULL," +
                "State INT(1) NOT NULL," +
                "FileSize VARCHAR(16) NOT NULL," +
                "BlockSize INT(65535) NOT NULL," +
                "UploadBlockSize INT(65535) NOT NULL," +
                "ServerStoragePath VARCHAR(65535) NOT NULL," +
                "UploadPath VARCHAR(65535) NOT NULL," +
                "DirID INT(10) NOT NULL," +
                "MD5 VARCHAR(128) NOT NULL" +
                ");";

            // log
            SQLParam += "DROP TABLE IF EXISTS log;";
            SQLParam += "CREATE TABLE log(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "IP VARCHAR(128) NOT NULL," +
                "ActionType INT(1) NOT NULL," +
                "ActionTime INT(10) NOT NULL," +
                "ActionDesc VARCHAR(65535) NOT NULL" +
                ");";

            // message
            SQLParam += "DROP TABLE IF EXISTS message;";
            SQLParam += "CREATE TABLE message(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "Title VARCHAR(128) NOT NULL," +
                "Content VARCHAR(128) NOT NULL," +
                "SenderID INT(10) NOT NULL," +
                "ReceiverID INT(10) NOT NULL," +
                "State INT(1) NOT NULL," +
                "Createtime INT(10) NOT NULL" +
                ");";

            // outer_token
            SQLParam += "DROP TABLE IF EXISTS outer_token;";
            SQLParam += "CREATE TABLE outer_token(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "UserID INT(10) NOT NULL," +
                "OuterToken VARCHAR(65535) NOT NULL," +
                "TokenDesc VARCHAR(128) NOT NULL," +
                "Createtime INT(10) NOT NULL" +
                ");";

            // token
            SQLParam += "DROP TABLE IF EXISTS token;";
            SQLParam += "CREATE TABLE token(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "UserID INT(10) NOT NULL," +
                "Token VARCHAR(128) NOT NULL," +
                "TokenType INT(1) NOT NULL," +
                "Createtime INT(10) NOT NULL" +
                ");";

            // user_extra
            SQLParam += "DROP TABLE IF EXISTS user_extra;";
            SQLParam += "CREATE TABLE user_extra(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "UserID INT(10) NOT NULL," +
                "ExtraDesc VARCHAR(128) NOT NULL," +
                "ExtraType INT(10) NOT NULL," +
                "ExtraValue VARCHAR(128) NOT NULL" +
                ");";

            // user
            SQLParam += "DROP TABLE IF EXISTS user;";
            SQLParam += "CREATE TABLE user(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "Account VARCHAR(128) NOT NULL," +
                "Name VARCHAR(128) NOT NULL," +
                "Password VARCHAR(128) NOT NULL," +
                "Secret INT(10) NOT NULL," +
                "Status INT(1) NOT NULL," +
                "Createtime INT(10) NOT NULL," +
                "Admin INT(1) NOT NULL," +
                "Avatar VARCHAR(2097152) NOT NULL," +
                "Wallpaper VARCHAR(65535) NOT NULL," +
                "Permission VARCHAR(128) NOT NULL," +
                "Master INT(1) NOT NULL," +
                "DepartmentID INT(10) NOT NULL" +
                ");";

            // department_file
            SQLParam += "DROP TABLE IF EXISTS department_file;";
            SQLParam += "CREATE TABLE department_file(" +
                "ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "DepartmentID INT(10) NOT NULL," +
                "FileID INT(10) NOT NULL," +
                "UserID INT(10) NOT NULL," +
                "Createtime INT(10) NOT NULL" +
                ");";

            // tag
            SQLParam += "DROP TABLE IF EXISTS tag;";
            SQLParam += "CREATE TABLE tag(" +
                "ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
                "TagName VARCHAR(68) NOT NULL," +
                "TagMemo VARCHAR(128)," +
                "UserID INT(10) NOT NULL" +
                ");";

            // file_tag
            SQLParam += "DROP TABLE IF EXISTS file_tag;";
            SQLParam += "CREATE TABLE file_tag(" +
                "ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
                "FileID INT(10) NOT NULL," +
                "TagID INT(10) NOT NULL" +
                ");";

            return SQLParam;
        }

        /// <summary>
        /// 定时任务
        /// </summary>
        public static void RunTask()
        {
            //Task.Factory.StartNew(() => Init.CrondTask1(), TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => Init.UDPServer(), TaskCreationOptions.LongRunning);
        }

        //async public static void CrondTask1()
        //{
        //    await Task.Delay(0);
        //    while (true)
        //    {
        //        Thread.Sleep(1000);
        //        Console.WriteLine("A" + "---" + Tools.TimeMS());
        //    }
        //}

        /// <summary>
        /// UDP广播服务
        /// </summary>
        async public static void UDPServer()
        {
            await Task.Delay(0);

            var UDPPort = ConfigHelper.GetConfig("UDP_Port");
            var URLS = ConfigHelper.GetConfig("urls").Split(";");
            var URL_HTTP = Tools.Explode(":", URLS[0])[2];
            var URL_HTTPS = "null";
            if (URLS[1].Length > 0)
            {
                URL_HTTPS = Tools.Explode(":", URLS[1])[2];
            }
            var Message = LocalIP() + ":" + URL_HTTP + "_" + LocalIP() + ":" + URL_HTTPS; // 待发送的信息

            var IPSegment = Tools.Explode(".", LocalIP())[2]; // 获取服务器网段
            // var UDPAddr = "192.168." + IPSegment + ".255"; // 构造UDP服务端地址
            var UDPAddr = IPAddress.Broadcast;

            UdpClient UDP = new();
            UDP.Connect(UDPAddr, Convert.ToInt32(UDPPort));
            Byte[] Data = Encoding.Default.GetBytes(Message);

            Console.WriteLine("UDP server working on {0}", UDPAddr + ":" + Convert.ToInt32(UDPPort));

            while (true)
            {
                try
                {
                    UDP.Send(Data, Data.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Thread.Sleep(1500);
            }
        }

        /// <summary>
        /// UDP广播接收服务(未使用)
        /// </summary>
        async public static void ReceiveUDPServer()
        {
            await Task.Delay(0);
            int Recv;
            var Data = new byte[64];
            var UDPPort = ConfigHelper.GetConfig("UDP_Port");
            var Message = ""; // 待发送的信息
            Socket Newsock = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // 实例化端口
            Newsock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); // 地址可重复使用
            IPEndPoint IP = new(IPAddress.Any, Convert.ToInt32(UDPPort)); // 得到本机IP 设置TCP端口号
            try
            {
                Newsock.Bind(IP); // 绑定网络地址
                Console.WriteLine("UDP: server working on {0}", LocalIP() + ":" + Convert.ToInt32(UDPPort));

                IPEndPoint Sender = new(IPAddress.Any, 0); // 获取得到客户端IP
                var Remote = (EndPoint)(Sender); // 客户端IP
                Recv = Newsock.ReceiveFrom(Data, ref Remote);
                Console.WriteLine("Message received from {0}: ", Remote.ToString());
                var ClientMessage = Encoding.ASCII.GetString(Data, 0, Recv); // 从客户端接收到的信息
                if (ClientMessage == "bitbox")
                {
                    var URLS = Environment.GetEnvironmentVariable("ASPNETCORE_URLS").Split(";");
                    var URL_HTTPS = Tools.Explode(":", URLS[0])[2];
                    var URL_HTTP = Tools.Explode(":", URLS[1])[2];
                    Message = LocalIP() + ":" + URL_HTTPS + "_" + LocalIP() + ":" + URL_HTTP;
                }
                Data = Encoding.ASCII.GetBytes(Message);
                Newsock.SendTo(Data, Data.Length, SocketFlags.None, Remote); // 发送信息
                Recv = Newsock.ReceiveFrom(Data, ref Remote);
                Newsock.SendTo(Data, Recv, SocketFlags.None, Remote);
                //Console.WriteLine(Encoding.ASCII.GetString(Data, 0, Recv));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Newsock.Close();
            }
        }

        /// <summary>
        /// 获取本地IP地址
        /// </summary>
        /// <returns></returns>
        public static string LocalIP()
        {
            string HostName = Dns.GetHostName();
            var IPAddrList = Dns.GetHostAddresses(HostName);
            var Result = "";
            foreach (IPAddress IP in IPAddrList)
            {
                if (IP.AddressFamily == AddressFamily.InterNetwork)
                {
                    Result = IP.ToString();
                }
            }
            return Result;
        }

        async public static void GetProcesses()
        {
            await Task.Delay(0);
            foreach (var P in Tools.GetProcesses())
            {
                Console.WriteLine(P.ProcessName);
            }
            Thread.Sleep(5000);
        }

        /// <summary>
        /// 设置数据库环境变量(以管理员身份运行系统)
        /// </summary>
        public static void SetDatabase()
        {
            if (Tools.OSType() == "Windows")
            {
                var SysParentPath = (Directory.GetParent(Tools.RootPath()).Parent.ToString()).Replace("\\", "/");
                var DatabasePath = SysParentPath + "/sqlite3";
                if (!Tools.DirIsExists(DatabasePath))
                {
                    Console.WriteLine("Database not found !!!");
                    System.Environment.Exit(0);
                }
                else
                {
                    var PathValue = Environment.GetEnvironmentVariable("Path"); // 获取系统变量Path的值
                    if (PathValue == null || PathValue == "")
                    {
                        Console.WriteLine("Sys environment error !!!");
                        System.Environment.Exit(0);
                    }
                    if (!PathValue.Contains(DatabasePath))
                    {
                        var PathData = PathValue + ";" + DatabasePath;
                        try
                        {
                            Environment.SetEnvironmentVariable("Path", PathData, EnvironmentVariableTarget.Machine);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Please run the system as an administrator !!!");
                            System.Environment.Exit(0);
                        }
                    }
                }
            }
        }

        public static string CMD(string CommandLine)
        {
            CommandLine = CommandLine.Trim().TrimStart('&') + "&exit";//&执行两条命令的标识，这里第二条命令的目的是当调用ReadToEnd()方法是，不会出现假死状态
            string outputMsg = "";
            Process pro = new();
            pro.StartInfo.FileName = "cmd.exe";//调用cmd.exe
            pro.StartInfo.UseShellExecute = false;//是否启用shell启动进程
            pro.StartInfo.RedirectStandardError = true;
            pro.StartInfo.RedirectStandardInput = true;
            pro.StartInfo.RedirectStandardOutput = true;//重定向的设置
            pro.StartInfo.CreateNoWindow = true;//不创建窗口
            pro.Start();
            pro.StandardInput.WriteLine(CommandLine);//执行cmd语句
            pro.StandardInput.AutoFlush = true;

            outputMsg += pro.StandardOutput.ReadToEnd();//读取返回信息
            //outputMsg=outputMsg.Substring(outputMsg.IndexOf(commandLine)+commandLine.Length);//返回发送命令之后的信息

            pro.WaitForExit();//等待程序执行完退出，不过感觉不用这条命令，也可以达到同样的效果
            pro.Close();

            return outputMsg;
        }
    }

    /// <summary>
    /// 获取配置文件
    /// </summary>
    public static class _ConfigHelper
    {
        private static IConfiguration _Configuration { get; set; }

        static _ConfigHelper()
        {
            //在当前目录或者根目录中寻找appsettings.json文件
            var ConfigFileName = "appsettings.json";

            var Directory = AppContext.BaseDirectory;
            Directory = Directory.Replace("\\", "/");

            var ConfigFilePath = $"{Directory}/{ConfigFileName}";

            var Builder = new ConfigurationBuilder().AddJsonFile(ConfigFilePath, false, true);

            _Configuration = Builder.Build();
        }

        public static string GetConfig(string ConfigName)
        {
            return _Configuration[ConfigName];
        }
    }
}
