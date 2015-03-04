using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Echo.Net
{
    using System.Net;
    using System.IO;

    public static class Util
    {
        /// <summary>
        /// 获取地理位置
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns></returns>
        public static string GetAddr(string ip)
        {
            string strURL = "http://whois.pconline.com.cn/ipJson.jsp?json=true&ip=" + ip;
            WebRequest request = WebRequest.Create(strURL);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("GBK"));
            string responseText = reader.ReadToEnd();
            reader.Close();
            //stream.Close();
            response.Close();
            string addr = responseText.Split('"')[31];
            return addr;
        }

        /// <summary>
        /// 获取操作系统版本名称
        /// </summary>
        /// <param name="osinfo"></param>
        /// <returns></returns>
        public static string GetOSVersionString(ServerPacket.Login._OSInfo osinfo)
        {
            string strClient = "";

            if (osinfo.Major == 5 && osinfo.Minor == 0)
            {
                strClient = "Windows 2000";
            }
            else if (osinfo.Major == 5 && osinfo.Minor == 1)
            {
                strClient = "Windows XP";
            }
            else if (osinfo.Major == 6 && osinfo.Minor == 0)
            {
                strClient = "Windows Vista";
            }
            else if (osinfo.Major == 6 && osinfo.Minor == 1)
            {
                strClient = "Windows 7";
            }
            else if (osinfo.Major == 6 && osinfo.Minor == 2)
            {
                strClient = "Windows 8";
            }
            else if (osinfo.Major == 6 && osinfo.Minor == 3)
            {
                strClient = "Windows 8.1";
            }
            else if (osinfo.Major == 6 && osinfo.Minor == 4)
            {
                strClient = "Windows 10";
            }
            else
            {
                return "未知";
            }

            strClient += osinfo.IS64Bit == 0 ? " x32" : " x64";
            return strClient;
        }
    }
}
