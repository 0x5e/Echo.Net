using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable()]
public class Telnet
{
    public string data { get; set; }
    public Telnet(string str)
    {
        data = str;
    }
}

