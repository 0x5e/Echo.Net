using System;
using System.Xml.Serialization;

/// <summary>
/// 客户端发送的数据包格式
/// </summary>
//[Serializable()]
[XmlRoot("Client")]
public class ClientPacket
{
    public int type { get; set; }
    public Screen screen { get; set; }
    public Camera camera { get; set; }
    public FileInfo fileInfo { get; set; }
    public RegInfo regIngo { get; set; }
    public Telnet telnet { get; set; }
    public Chat chat { get; set; }
    public TaskMgr taskMgr { get; set; }

    public ClientPacket() { }

    public ClientPacket(PacketType type)
    {
        this.type = (int)type;
    }

    /// <summary>
    /// 屏幕监控
    /// </summary>
    public class Screen
    {
        /// <summary>
        /// 开关
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        /// 图像质量(0~100)
        /// </summary>
        public int Quality { get; set; }

        public Keybd keybd { get; set; }
        public Mouse mouse { get; set; }

        /// <summary>
        /// 键盘
        /// </summary>
        public class Keybd
        {
            public byte bVk { get; set; }
            public byte bScan { get; set; }
            public int dwFlags { get; set; }
        }

        /// <summary>
        /// 鼠标
        /// </summary>
        public class Mouse
        {
            public int dwFlags { get; set; }
            public int dx { get; set; }
            public int dy { get; set; }
            public int dwData { get; set; }
        }

        public Screen() { }

        public Screen(POWER power)
        {
            this.Power = (int)power;
        }
    }

    /// <summary>
    /// 视频监控
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// 开关
        /// </summary>
        public int Power { get; set; }
    }

    /// <summary>
    /// 文件管理
    /// </summary>
    public class FileInfo
    {
        /// <summary>
        /// 目录信息
        /// </summary>
        public string DirInfo { get; set; }

        /// <summary>
        /// 创建目录
        /// </summary>
        public string CreateDir { get; set; }

        /// <summary>
        /// 删除文件/目录
        /// </summary>
        public string Delete { get; set; }

        /// <summary>
        /// 源文件
        /// </summary>
        public string Move { get; set; }

        /// <summary>
        /// 目标文件
        /// </summary>
        public string MoveTo { get; set; }

        /// <summary>
        /// 移动or复制
        /// </summary>
        public bool isCopy { get; set; }
    }

    /// <summary>
    /// 注册表管理
    /// </summary>
    public class RegInfo
    {
        //TODO:
    }

    /// <summary>
    /// 远程终端
    /// </summary>
    public class Telnet
    {
        public string Request { get; set; }

        public Telnet() { }

        public Telnet(string str)
        {
            this.Request = str;
        }
    }

    /// <summary>
    /// 文字聊天
    /// </summary>
    public class Chat
    {
        /// <summary>
        /// 开关
        /// </summary>
        public int Power { get; set; }
        public string Request { get; set; }

        public Chat() { }

        public Chat(POWER power)
        {
            this.Power = (int)power;
        }

        public Chat(string str)
        {
            this.Request = str;
        }
    }

    /// <summary>
    /// 进程管理
    /// </summary>
    public class TaskMgr
    {
        public int ClosePid { get; set; }
        public int KillPid { get; set; }
    }
}

