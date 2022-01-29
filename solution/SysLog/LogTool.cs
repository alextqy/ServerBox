using Service;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SysLog
{
    public class LogTool
    {
        private static readonly string LogFile = Tools.RootPath() + "Log/" + Tools.NowYear() + Tools.NowMonth() + Tools.NowDay() + ".log";

        public LogTool() { }

        /// <summary>
        /// 日志服务
        /// </summary>
        public static async void LogServer()
        {
            await Task.Delay(0);
            while (true)
            {
                LogHandler();
                Thread.Sleep(1500);
            }
        }

        public static void LogHandler()
        {
            var LogDir = Tools.RootPath() + "Log/";
            Tools.CreateDir(LogDir);
            Tools.CreateFile(LogFile);
        }

        public static bool WriteLog(string Content)
        {
            Tools.CreateFile(LogFile);
            return Tools.WriteFile(LogFile, Content, true);
        }

        public static string ReadLog(int TimeStamp = 0)
        {
            string SetLogDate;
            if (TimeStamp == 0)
            {
                SetLogDate = Tools.NowYear() + Tools.NowMonth() + Tools.NowDay();
            }
            else
            {
                SetLogDate = Tools.TimeToStr(TimeStamp).Split(" ")[0].Replace("-", "");
            }
            var LogFile = Tools.RootPath() + "Log/" + SetLogDate + ".log";
            if (!Tools.FileIsExists(LogFile))
            {
                return "";
            }
            else
            {
                return Tools.ReadFile(LogFile);
            }
        }

        public static bool DelLog(int TimeStamp = 0)
        {
            string SetLogDate;
            if (TimeStamp == 0)
            {
                SetLogDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
            }
            else
            {
                var Date = Tools.TimeStampToDateTime(TimeStamp);
                SetLogDate = Date.Year.ToString() + Date.Month.ToString() + Date.Day.ToString();
            }
            return Tools.DelFile(Tools.RootPath() + "Log/" + SetLogDate + ".log");
        }

        public static bool ClearLog()
        {
            return Tools.DelDir(Tools.RootPath() + "Log", true);
        }

    }
}
