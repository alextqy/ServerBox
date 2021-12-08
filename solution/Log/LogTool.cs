using System;
using System.Threading;
using System.Threading.Tasks;

namespace Log
{
    public class LogTool
    {
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

        }

    }
}
