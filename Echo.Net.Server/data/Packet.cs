using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Xml;
using System.Xml.Serialization;

public class Packet
{
    NetworkStream stream;

    public Packet(NetworkStream stream)
    {
        this.stream = stream;
    }

    public void Send<T>(T obj)
    {
        byte[] xmlbytes;
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
        using (MemoryStream ms = new MemoryStream())
        {
            xmlSerializer.Serialize(ms, obj);
            xmlbytes = ms.ToArray();
        }

        int type = _Type.type2int(typeof(T));

        Trace.WriteLine("Type:" + typeof(T));
        Trace.WriteLine("Length:" + xmlbytes.Length);
        foreach (byte b in xmlbytes)
            Trace.Write((char)b);
        Trace.WriteLine("");
        
        lock (stream)
        {
            //包长度
            stream.WriteByte((byte)(xmlbytes.Length >> 8));
            stream.WriteByte((byte)xmlbytes.Length);

            //包类型
            stream.WriteByte((byte)type);

            //包内容
            stream.Write(xmlbytes, 0, xmlbytes.Length);
        }
    }

    public T Recv<T>()
    {
        int len = (stream.ReadByte() << 8) | stream.ReadByte();
        Type type = _Type.int2type(stream.ReadByte());

        byte[] xmlbytes = new byte[len];
        lock (stream)
        {
            stream.Read(xmlbytes, 0, len);
        }

        T t = default(T);
        XmlSerializer xmlSerializer = new XmlSerializer(type);
        using (Stream xmlStream = new MemoryStream(xmlbytes))
        {
            using (XmlReader xmlReader = XmlReader.Create(xmlStream))
            {
                Object obj = xmlSerializer.Deserialize(xmlReader);
                t = (T)obj;
            }
        }
        return t;
    }
}
