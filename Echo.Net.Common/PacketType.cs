//数据包类型
public enum PacketType
{
    Ping = 0x01,
    Login = 0x01,
    MoreInfo = 0x02,
    Screen = 0x03,
    Camera = 0x04,
    FileInfo = 0x05,
    RegInfo = 0x06,
    Telnet = 0x07,
    Chat = 0x08,
    TaskMgr = 0x09,
}

public enum POWER
{
    OFF = 0x00,
    ON = 0x01,
}