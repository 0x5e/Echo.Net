using System;
using System.Xml.Serialization;

/// <summary>
/// 服务端发送的数据包格式
/// </summary>
[XmlRoot("Server")]
public class ServerPacket
{
    public int type { get; set; }
    public Login login { get; set; }
    public MoreInfo moreInfo { get; set; }
    public Screen screen { get; set; }
    public Camera camera { get; set; }
    public FileInfo fileInfo { get; set; }
    public RegInfo regInfo { get; set; }
    public Telnet telnet { get; set; }
    public Chat chat { get; set; }
    public TaskMgr taskMgr { get; set; }

    public ServerPacket() { }

    public ServerPacket(PacketType type)
    {
        this.type = (int)type;
    }

    /// <summary>
    /// 登录信息
    /// </summary>
    public class Login
    {
        /// <summary>
        /// 计算机名称
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// 操作系统信息
        /// </summary>
        public _OSInfo OSInfo { get; set; }

        /// <summary>
        /// 摄像头
        /// </summary>
        public int hasCamera { get; set; }

        /// <summary>
        /// 服务端版本
        /// </summary>
        public string Version { get; set; }

        //以下为ListView显示用
        [XmlIgnore()]

        /// <summary>
        /// IP地址:端口
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 地理位置
        /// </summary>
        public string Addr { get; set; }

        /// <summary>
        /// 登录时间戳
        /// </summary>
        public string LoginTime { get; set; }

        /// <summary>
        /// 操作系统版本/位数
        /// </summary>
        public string OSVersion { get; set; }

        /// <summary>
        /// 摄像头
        /// </summary>
        public string Camera { get; set; }

        /// <summary>
        /// Ping
        /// </summary>
        public string Ping { get; set; }

        /// <summary>
        /// Handler对象
        /// </summary>
        public object handler;

        public class _OSInfo
        {
            public int Major { get; set; }
            public int Minor { get; set; }
            public int Build { get; set; }
            public int IS64Bit { get; set; }

            public _OSInfo()
            {
                Major = System.Environment.OSVersion.Version.Major;
                Minor = System.Environment.OSVersion.Version.Minor;
                Build = System.Environment.OSVersion.Version.Build;
                IS64Bit = IntPtr.Size * 8 == 64 ? 1 : 0;
            }
        }

        public Login()
        {
            MachineName = System.Environment.MachineName;
            OSInfo = new _OSInfo();
            //OSVersion = System.Environment.OSVersion.Version.ToString();
            //hasCamera = 0;
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }

    //详细信息
    public class MoreInfo
    {
        //TODO:
    }

    //屏幕监控
    public class Screen
    {
        public byte[] img { get; set; }
        //public byte[] sound { get; set; }
    }

    //视频监控
    public class Camera
    {
        public byte[] img { get; set; }
        //public byte[] sound { get; set; }
    }

    //文件管理
    public class FileInfo
    {
        public int retCode { get; set; }
        public DirInfo dirInfo { get; set; }

        //目录信息
        public class DirInfo
        {
            //当前路径
            public string Path { get; set; }
            //文件列表
            public File[] Files { get; set; }

            //文件信息
            public class File
            {
                //文件名
                public string Name { get; set; }
                //文件or文件夹
                public int isFile { get; set; }
                //文件大小
                public long Size { get; set; }
                //创建时间
                public long CreateTime { get; set; }
                //修改时间
                public long LastWriteTime { get; set; }
            }
        }

        //TODO:
    }

    //注册表信息
    public class RegInfo
    {
        //TODO:
    }

    //远程终端
    public class Telnet
    {
        public string Response { get; set; }

        public Telnet() { }

        public Telnet(string str)
        {
            this.Response = str;
        }
    }

    //文字聊天
    public class Chat
    {
        public string Response { get; set; }

        public Chat() { }

        public Chat(string str)
        {
            this.Response = str;
        }
    }

    //进程管理
    public class TaskMgr
    {
        public int retCode { get; set; }
        public TaskItem[] Items { get; set; }

        public class TaskItem
        {
            public string Name { get; set; }
            public int PID { get; set; }
            //public string UserName { get; set; }
            //public int CPU { get; set; }
            public long Memory { get; set; }
            //public string Description { get; set; }
            //public string CommandLine { get; set; }
        }

    }

}

