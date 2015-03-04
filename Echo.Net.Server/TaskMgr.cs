using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Echo.Net.Server
{
    public static class TaskMgr
    {
        public static void Process(ClientPacket.TaskMgr taskMgr)
        {
            ServerPacket serverPacket = new ServerPacket(PacketType.TaskMgr);
            serverPacket.taskMgr = new ServerPacket.TaskMgr();
            if (taskMgr == null)
            {
                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
                serverPacket.taskMgr.Items = new ServerPacket.TaskMgr.TaskItem[processes.Length];
                for (int i = 0; i < processes.Length; i++)
                {
                    serverPacket.taskMgr.Items[i].Name = processes[i].ProcessName;
                    serverPacket.taskMgr.Items[i].PID = processes[i].Id;
                    serverPacket.taskMgr.Items[i].Memory = processes[i].PrivateMemorySize64;
                }

            }
            else if (taskMgr.ClosePid != 0)
                System.Diagnostics.Process.GetProcessById(taskMgr.ClosePid).Close();
            else if (taskMgr.KillPid != 0)
                System.Diagnostics.Process.GetProcessById(taskMgr.KillPid).Kill();
            Server.packetStream.Send(serverPacket);
        }
    }
}
