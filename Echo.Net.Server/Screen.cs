using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Echo.Net.Server
{
    using System.Threading;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;

    public static class Screen
    {
        static volatile int power;
        static Thread thread;

        static ImageCodecInfo imageCodecInfo = GetEncoderInfo("image/jpeg");
        static EncoderParameters ps = new EncoderParameters();

        static Rectangle rect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

        public static void Process(ClientPacket.Screen screen)
        {
            Screen.power = screen.Power;
            if (Screen.power != 0)
            {
                //屏幕
                if (screen.Quality != 0)
                {
                    ps = new EncoderParameters(1);
                    ps.Param[0] = new EncoderParameter(Encoder.Quality, screen.Quality);
                }
                if (Screen.thread == null || !Screen.thread.IsAlive)
                {
                    Screen.thread = new Thread(new ThreadStart(Work));
                    Screen.thread.Start();
                }

                //鼠标
                if (screen.mouse != null)
                    NativeMethods.mouse_event(screen.mouse.dwFlags, screen.mouse.dx, screen.mouse.dy, 0, 0);

                //键盘
                if (screen.keybd != null)
                    NativeMethods.keybd_event(screen.keybd.bVk, screen.keybd.bScan, screen.keybd.dwFlags, 0);
            }
        }

        private static void Work()
        {
            while (Screen.power != 0)
            {
                byte[] image = CaptureDesktop();

                ServerPacket serverPacket = new ServerPacket(PacketType.Screen);
                serverPacket.screen = new ServerPacket.Screen();
                serverPacket.screen.img = image;

                //如果被锁就丢弃
                if (Monitor.TryEnter(Server.packetStream.tcpClient))
                {
                    Server.packetStream.Send(serverPacket);
                    Monitor.Exit(Server.packetStream.tcpClient);
                    //Thread.Sleep(20);
                }
                else
                {
                    Trace.WriteLine("Throw Packet");
                }
            }
        }

        private static byte[] CaptureDesktop()
        {
            byte[] bytes;
            using (MemoryStream ms = new MemoryStream())
            {
                using (Bitmap bmp = new Bitmap(rect.Width, rect.Height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.CopyFromScreen(0, 0, 0, 0, rect.Size);
                        //g.Dispose();
                    }
                    bmp.Save(ms, imageCodecInfo, ps);
                    //bmp.Save(ms, ImageFormat.Jpeg);
                    //bmp.Dispose();
                }
                bytes = ms.ToArray();
            }
            return bytes;
        }
        
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo encoder in encoders)
            {
                if (encoder.MimeType == mimeType)
                    return encoder;
            }
            return null;
        }
    }
}
