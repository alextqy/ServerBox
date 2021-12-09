using SysLog;
using System.Threading.Tasks;
using UDP;


namespace CrondTask
{
    public class CrondTool : Base
    {
        /// <summary>
        /// 定时任务
        /// </summary>
        public static void RunTask()
        {
            Task.Factory.StartNew(() => UDPTool.UDPServer(), TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => LogTool.LogServer(), TaskCreationOptions.LongRunning);
        }
    }
}
