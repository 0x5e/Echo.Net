using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

public enum InputType
{
    MOUSE = 0,
    KEYBOARD = 1,
    HARDWARE = 2,
}

[Flags()]
public enum MOUSEEVENTF
{
    MOVE = 0x0001,  //mouse move     
    LEFTDOWN = 0x0002,  //left button down     
    LEFTUP = 0x0004,  //left button up     
    RIGHTDOWN = 0x0008,  //right button down     
    RIGHTUP = 0x0010,  //right button up     
    MIDDLEDOWN = 0x0020, //middle button down     
    MIDDLEUP = 0x0040,  //middle button up     
    XDOWN = 0x0080,  //x button down     
    XUP = 0x0100,  //x button down     
    WHEEL = 0x0800,  //wheel button rolled     
    VIRTUALDESK = 0x4000,  //map to entire virtual desktop     
    ABSOLUTE = 0x8000,  //absolute move     
}

[Flags()]
public enum KEYEVENTF
{
    EXTENDEDKEY = 0x0001,
    KEYUP = 0x0002,
    UNICODE = 0x0004,
    SCANCODE = 0x0008,
}

[StructLayout(LayoutKind.Explicit)]
public struct INPUT
{
    [FieldOffset(0)]
    public Int32 type;//0-MOUSEINPUT;1-KEYBDINPUT;2-HARDWAREINPUT     
    [FieldOffset(4)]
    public KEYBDINPUT ki;
    [FieldOffset(4)]
    public MOUSEINPUT mi;
    [FieldOffset(4)]
    public HARDWAREINPUT hi;
}

[StructLayout(LayoutKind.Sequential)]
public struct MOUSEINPUT
{
    public Int32 dx;
    public Int32 dy;
    public UInt32 dwData;
    public UInt32 dwFlags;
    public UInt32 time;
    public UInt32 dwExtraInfo;
}

[StructLayout(LayoutKind.Sequential)]
public struct KEYBDINPUT
{
    public UInt16 wVk;
    public UInt16 wScan;
    public UInt32 dwFlags;
    public UInt32 time;
    public UInt32 dwExtraInfo;
}

[StructLayout(LayoutKind.Sequential)]
public struct HARDWAREINPUT
{
    public UInt32 uMsg;
    public UInt16 wParamL;
    public UInt16 wParamH;
}
