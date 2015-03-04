using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Echo.Net
{
    using System.Threading;
    using System.Net.Sockets;

    /// <summary>
    /// 负责接收数据包,处理后发送至对应窗口
    /// </summary>
    public class Handler
    {
        public Main main;
        public TcpClient tcpClient;
        public PacketStream packetStream;
        public ServerPacket.Login login_info;

        //窗口对象
        public Telnet telnet;
        public Screen screen;

        public Handler(TcpClient tcpClient, Main main)
        {
            this.main = main;
            this.tcpClient = tcpClient;
            this.packetStream = new PacketStream(tcpClient, false);
            Thread thread = new Thread(new ThreadStart(Work));
            thread.Start();
        }

        public void Work()
        {
            try
            {
                if (Login() == true)
                {
                    while (tcpClient.Client.Connected)
                    {
                        ServerPacket serverPacket = packetStream.Recv<ServerPacket>();
                        switch ((PacketType)serverPacket.type)
                        {
                            //TODO:
                            case PacketType.MoreInfo:
                                break;
                            case PacketType.Screen:
                                if (screen != null)
                                {
                                    Action<ServerPacket.Screen> action = new Action<ServerPacket.Screen>(screen.Recv);
                                    screen.Dispatcher.BeginInvoke(action, serverPacket.screen);
                                }
                                break;
                            case PacketType.Camera:
                                break;
                            case PacketType.FileInfo:
                                break;
                            case PacketType.RegInfo:
                                break;
                            case PacketType.Telnet:
                                if (telnet != null)
                                {
                                    Action<ServerPacket.Telnet> action = new Action<ServerPacket.Telnet>(telnet.Recv);
                                    telnet.Dispatcher.BeginInvoke(action, serverPacket.telnet);
                                }
                                break;
                            case PacketType.Chat:
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                tcpClient.Close();
                tcpClient = null;
            }

            if (login_info != null)
            {
                if (main.listView.Items.Contains(login_info))
                {
                    Action<ServerPacket.Login> action = new Action<ServerPacket.Login>(main.remove_item);
                    main.Dispatcher.BeginInvoke(action, login_info);
                }
            }
        }

        public bool Login()
        {
            ServerPacket serverPacket = packetStream.Recv<ServerPacket>();
            if ((PacketType)serverPacket.type != PacketType.Login)
                return false;

            ClientPacket pingPacket = new ClientPacket(PacketType.Ping);
            long old_time = DateTime.Now.Ticks;
            packetStream.Send(pingPacket);
            packetStream.Recv<ServerPacket>();
            long new_time = DateTime.Now.Ticks;

            login_info = serverPacket.login;
            login_info.LoginTime = DateTime.Now.ToString("HH:mm:ss");
            login_info.IP = tcpClient.Client.RemoteEndPoint.ToString();
            //login_info.Addr = Util.GetAddr(login_info.IP);
            login_info.OSVersion = Util.GetOSVersionString(login_info.OSInfo);
            login_info.Camera = login_info.hasCamera == 0 ? "有" : "无";
            login_info.Ping = (new_time - old_time) / 10000 + "ms";

            login_info.handler = this;
            main.Dispatcher.BeginInvoke(new Action<ServerPacket.Login>(main.add_item), login_info);

            return true;
        }

        public void OpenWindow(string name)
        {
            switch (name)
            {
                //TODO:
                case "MoreInfo":
                    break;
                case "Screen":
                    {
                        if (screen == null)
                            screen = new Screen(this);
                        else
                            screen.Activate();
                    }
                    break;
                case "Camera":
                    break;
                case "FileInfo":
                    break;
                case "RegInfo":
                    break;
                case "Telnet":
                    {
                        if (telnet == null)
                            telnet = new Telnet(this);
                        else
                            telnet.Activate();
                    }
                    break;
                case "Chat":
                    break;
            }
        }

    }
}