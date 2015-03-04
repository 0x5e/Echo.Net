using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;

[Serializable()]
public class LoginInfo
{
    public string name { get; set; }
    public string os { get; set; }
    public string camera { get; set; }
    public string version { get; set; }
    public string ping { get; set; }

    public string addr { get; set; }//127.0.0.1:60000
    public string location { get; set; }//地理位置
    public string login_time { get; set; }//登录时间戳

    public void getInfo()
    {
        name = System.Environment.MachineName;
        os = System.Environment.OSVersion.VersionString;
        version = "1.0";
        ping = "0";
    }
}
