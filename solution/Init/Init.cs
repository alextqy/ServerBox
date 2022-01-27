using System;
using System.IO;
using Service;
using Microsoft.Data.Sqlite;

namespace Init
{
    public class SysInit : Base
    {
        /// <summary>
        /// 检查配置文件
        /// </summary>
        /// <returns></returns>
        public bool CheckConfigFile()
        {
            var CheckState = ConfigHelper.AppSettingsHelper.AppSettingsState(Tools.RootPath() + "appsettings.json");
            if (!CheckState) Tools.WarningConsole("Can't find configuration file!");
            return CheckState;
        }

        /// <summary>
        /// 设置数据库环境变量(以管理员身份运行系统)
        /// </summary>
        public bool SetDatabase()
        {
            if (Tools.OSType() == "Windows")
            {
                var DatabasePath = Tools.RootPath() + "sqlite3";
                if (!Tools.DirIsExists(DatabasePath))
                {
                    Tools.WarningConsole("Database not found!");
                    return false;
                }
                else
                {
                    var PathValue = Environment.GetEnvironmentVariable("Path"); // 获取系统变量Path的值
                    if (String.IsNullOrEmpty(PathValue))
                    {
                        Tools.WarningConsole("Sys environment error!");
                        return false;
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
                            Tools.WarningConsole(e.Message);
                            Tools.WarningConsole("Please run the system as an administrator!");
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 项目启动
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            if (!ConfigHelper.AppSettingsHelper.InitState()) // 初始化判断
            {
                #region Step1 初始化项目文件
                BakInitFile(); // 设置为起始状态
                string BaseDir = Tools.RootPath() + "Matrix"; // 新建基础目录
                if (!Tools.CreateDir(BaseDir))
                {
                    Tools.WarningConsole("ERROR: Failed to make base dir!");
                    DelInitFile(); // 回到起始状态
                    return false;
                }
                string DaoRoom = Tools.RootPath() + "DaoRoom.db"; // 数据库文件路径
                if (!Tools.CreateFile(DaoRoom))
                {
                    Tools.WarningConsole("ERROR: Failed to make database file!");
                    DelInitFile(); // 回到起始状态
                    return false;
                }
                SqliteConnection SqliteObject = new("Data Source = " + DaoRoom); // 连接数据库
                try
                {
                    SqliteObject.Open(); // 打开链接
                    SqliteCommand InitDatabase = new(CreateTable(), SqliteObject);// 初始化数据表
                    InitDatabase.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Tools.WarningConsole(e.Message);
                    Tools.WarningConsole("ERROR: Failed to connect database!");
                    DelInitFile(); // 回到起始状态
                    return false;
                }
                #endregion

                #region Step2 初始化系统数据
                try
                {
                    // 初始化超级管理员数据
                    int Secret = Tools.Random(5); // 获取随机数
                    string PWD = "123456"; // 设置管理员初始密码
                    string Password = Tools.UserPWD(PWD, Secret.ToString()); // 初始化生成管理员密码
                    string AdminSQL = "INSERT INTO user (Account, Name, Password, Secret, Status, Createtime, Admin, Avatar, Wallpaper, Permission, Master, DepartmentID) VALUES ('root', 'Admin', '" + Password + "', '" + Secret + "', 1, " + Tools.Time32().ToString() + ", 2, '', '', '1,2,3,4,5,6,7,8,9', 2, 0);"; // 写入数据
                    SqliteCommand InitAdminData = new(AdminSQL, SqliteObject); // 执行SQL
                    InitAdminData.ExecuteNonQuery();

                    // 初始化超级管理员文件夹数据
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
                }
                catch (Exception e)
                {
                    Tools.WarningConsole(e.Message);
                    Tools.WarningConsole("ERROR: Failed to initialize admin data!");
                    DelInitFile(); // 回到起始状态
                    return false;
                }
                SqliteObject.Close(); // 关闭链接
                #endregion

                #region Step3 完成初始化
                if (!Tools.CreateDir(BaseDir + "/root")) // 新建管理员目录
                {
                    Tools.WarningConsole("ERROR: Failed to make admin dir!");
                    DelInitFile(); // 回到起始状态
                    return false;
                }
                if (!ConfigHelper.AppSettingsHelper.SetInit()) // 修改配置文件
                {
                    Tools.WarningConsole("ERROR: System initialization failed!");
                    DelInitFile(); // 回到起始状态
                    return false;
                }
                #endregion

                Tools.CorrectConsole("System initialization completed!");
            }
            return true;
        }

        /// <summary>
        /// 删除配置文件和数据库文件
        /// </summary>
        public void BakInitFile()
        {
            Tools.RenameDir(Tools.RootPath() + "Matrix", "old_Matrix_" + Tools.TimeMS().ToString());
            Tools.RenameFile(Tools.RootPath() + "DaoRoom.db", "old_DaoRoom.db_" + Tools.TimeMS().ToString());
        }

        /// <summary>
        /// 删除配置文件和数据库文件
        /// </summary>
        public void DelInitFile()
        {
            Tools.DelDir(Tools.RootPath() + "Matrix", true);
            Tools.DelFile(Tools.RootPath() + "DaoRoom.db");
        }

        /// <summary>
        /// SQL
        /// </summary>
        /// <returns></returns>
        public string CreateTable()
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
            //SQLParam += "DROP TABLE IF EXISTS log;";
            //SQLParam += "CREATE TABLE log(" +
            //    "ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
            //    "IP VARCHAR(128) NOT NULL," +
            //    "ActionType INT(1) NOT NULL," +
            //    "ActionTime INT(10) NOT NULL," +
            //    "ActionDesc VARCHAR(65535) NOT NULL" +
            //    ");";

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

            // offline_task
            SQLParam += "DROP TABLE IF EXISTS offline_task;";
            SQLParam += "CREATE TABLE offline_task(" +
                "ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
                "UserID INT(10) NOT NULL," +
                "URL VARCHAR(65535) NOT NULL," +
                "State INT(1) NOT NULL," +
                "TaskMemo VARCHAR(128)," +
                "Createtime INT(10) NOT NULL" +
                ");";

            return SQLParam;
        }
    }
}
