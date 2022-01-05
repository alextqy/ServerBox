using SysLog;
using System.Threading.Tasks;
using UDP;
using DataParallel;

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
            Task.Factory.StartNew(() => OfflineTaskThreadPool.Test(), TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => OfflineTaskThreadPool.ProducerProcessServer(), TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => OfflineTaskThreadPool.ConsumerProcessServer(), TaskCreationOptions.LongRunning);
        }
    }
}
