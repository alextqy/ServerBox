using Service;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDP
{
    public class UDPTool : Base
    {
        /// <summary>
        /// UDP广播服务
        /// </summary>
        public static async void UDPServer()
        {
            await Task.Delay(0);

            //var UDPPort = ConfigHelper.AppSettingsHelper.GetSettings("UDPPort");
            //var URLS = ConfigHelper.AppSettingsHelper.GetSettings("urls").Split(";");
            //var URL_HTTP = Tools.Explode(":", URLS[0])[2];
            //var URL_HTTPS = "null";
            //if (URLS[1].Length > 0) { URL_HTTPS = Tools.Explode(":", URLS[1])[2]; }
            //var Message = Tools.LocalIP() + ":" + URL_HTTP + "_" + Tools.LocalIP() + ":" + URL_HTTPS; // 待发送的信息
            //var IPSegment = Tools.Explode(".", Tools.LocalIP())[2]; // 获取服务器网段
            // var UDPAddr = "192.168." + IPSegment + ".255"; // 构造UDP服务端地址
            //var UDPAddr = IPAddress.Broadcast;

            var UDPAddr = IPAddress.Broadcast;
            var UDPPort = ConfigHelper.AppSettingsHelper.GetSettings("UDPPort");

            UdpClient UDP = new();
            UDP.Connect(UDPAddr, Convert.ToInt32(UDPPort));
            Byte[] Data = Encoding.Default.GetBytes(ConfigHelper.AppSettingsHelper.GetSettings("URL").Replace("*", Tools.LocalIP()));
            Console.WriteLine("UDP server working on {0}", UDPAddr + ":" + Convert.ToInt32(UDPPort));
            while (true)
            {
                try
                {
                    UDP.Send(Data, Data.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Thread.Sleep(1500);
            }
        }

        /// <summary>
        /// UDP广播接收服务(未使用)
        /// </summary>
        public static async void ReceiveUDPServer()
        {
            await Task.Delay(0);
            int Recv;
            var Data = new byte[64];
            var UDPPort = ConfigHelper.AppSettingsHelper.GetSettings("UDPPort");
            var Message = ""; // 待发送的信息
            Socket Newsock = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // 实例化端口
            Newsock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); // 地址可重复使用
            IPEndPoint IP = new(IPAddress.Any, Convert.ToInt32(UDPPort)); // 得到本机IP 设置TCP端口号
            try
            {
                Newsock.Bind(IP); // 绑定网络地址
                Console.WriteLine("UDP: server working on {0}", Tools.LocalIP() + ":" + Convert.ToInt32(UDPPort));

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
                    Message = Tools.LocalIP() + ":" + URL_HTTPS + "_" + Tools.LocalIP() + ":" + URL_HTTP;
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
    }
}
