using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml;
using System.Xml.Serialization;

class Xml
{
    /// <summary>  
    /// 对象序列化成 XML byte[]  
    /// </summary>
    public static byte[] Serialize<T>(T obj)
    {
        byte[] xmlbytes;
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
        using (MemoryStream ms = new MemoryStream())
        {
            xmlSerializer.Serialize(ms, obj);
            xmlbytes = ms.ToArray();
        }
        return xmlbytes;
    }

    /// <summary>  
    /// XML byte[] 反序列化成对象  
    /// </summary>  
    public static T Deserialize<T>(byte[] xmlbytes)
    {
        T t = default(T);
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
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
