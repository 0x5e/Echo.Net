using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Xml;
using System.Xml.Serialization;

public class PacketStream
{
    public TcpClient tcpClient;
    NetworkStream stream;
    XmlSerializer xmlSerializer_send;
    XmlSerializer xmlSerializer_recv;
    XmlSerializerNamespaces ns;

    public PacketStream(TcpClient tcpClient, bool isServer)
    {
        this.tcpClient = tcpClient;
        this.stream = tcpClient.GetStream();

        ns = new XmlSerializerNamespaces();
        ns.Add("", "");
        if (isServer)
        {
            xmlSerializer_send = new XmlSerializer(typeof(ServerPacket));
            xmlSerializer_recv = new XmlSerializer(typeof(ClientPacket));
        }
        else
        {
            xmlSerializer_send = new XmlSerializer(typeof(ClientPacket));
            xmlSerializer_recv = new XmlSerializer(typeof(ServerPacket));
        }
    }

    public void Send<T>(T obj)
    {
        if (!tcpClient.Client.Connected)
            return;

        MemoryStream ms = new MemoryStream();
        xmlSerializer_send.Serialize(ms, obj, ns);
        byte[] xmlbytes = ms.GetBuffer();
        int len = (int)ms.Length;

        Trace.WriteLine("-------------------------------");
        Trace.WriteLine("Send Length:" + len);
        if (len < 1024)
            Trace.WriteLine(Encoding.UTF8.GetString(xmlbytes, 0, len));

        lock (tcpClient)
        {
            //包长度
            byte[] len_array = BitConverter.GetBytes(len);
            stream.Write(len_array, 0, 4);
            //包内容
            stream.Write(xmlbytes, 0, len);

            /*
            //拆开发送
            int packet_max_len = 64 * 1024;
            for (int i = 0; i * packet_max_len < len; i++)
            {
                int before = i * packet_max_len;
                int packet_len = before + packet_max_len > len ? len - before : packet_max_len;

                stream.Write(xmlbytes, before, packet_len);
                //System.Threading.Thread.Sleep(100);
            }
            */
        }
    }

    public T Recv<T>()
    {
        byte[] len_array = new byte[4];
        stream.Read(len_array, 0, 4);

        int len = BitConverter.ToInt32(len_array, 0);
        byte[] xmlbytes = new byte[len];

        int before = 0, count = 0;
        do
        {
            count = stream.Read(xmlbytes, before, len - before);
            before += count;
        } while (count != 0 && before < len);
        //stream.Read(xmlbytes, 0, len);

        Trace.WriteLine("-------------------------------");
        Trace.WriteLine("Recv Length:" + len);
        if (len < 1024)
            Trace.WriteLine(Encoding.UTF8.GetString(xmlbytes, 0, len));

        T t = default(T);
        using (Stream xmlStream = new MemoryStream(xmlbytes))
        {
            using (XmlReader xmlReader = XmlReader.Create(xmlStream))
            {
                try
                {
                    Object obj = xmlSerializer_recv.Deserialize(xmlReader);
                    t = (T)obj;
                }
                catch (Exception)
                {
                }
            }
        }
        return t;
    }
}
