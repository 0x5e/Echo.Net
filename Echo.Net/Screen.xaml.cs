using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

namespace Echo.Net
{
    using System.IO;

    /// <summary>
    /// Screen.xaml 的交互逻辑
    /// </summary>
    public partial class Screen : Window
    {
        Handler handler;
        POWER power;

        public Screen(Handler handler)
        {
            InitializeComponent();
            this.handler = handler;
            Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Remote(POWER.ON, 50);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Remote(POWER.OFF, 0);
            handler.screen = null;
        }

        private void Screen_MouseEnter(object sender, MouseEventArgs e)
        {
            if (handler.tcpClient == null || handler.tcpClient.Client.Connected == false || power == POWER.OFF || monitor.Source == null)
                return;

            ClientPacket clientPacket = new ClientPacket(PacketType.Screen);
            clientPacket.screen = new ClientPacket.Screen(POWER.ON);
            clientPacket.screen.mouse = new ClientPacket.Screen.Mouse();
            Point point = e.GetPosition(monitor);
            clientPacket.screen.mouse.dx = (int)(point.X * 65535 / monitor.Source.Width);
            clientPacket.screen.mouse.dy = (int)(point.Y * 65535 / monitor.Source.Height);
            clientPacket.screen.mouse.dwFlags = (int)(MOUSEEVENTF.MOVE | MOUSEEVENTF.ABSOLUTE);
            handler.packetStream.Send(clientPacket);
        }

        private void Screen_MouseButton(object sender, MouseButtonEventArgs e)
        {
            if (handler.tcpClient == null || handler.tcpClient.Client.Connected == false || power == POWER.OFF)
                return;

            ClientPacket clientPacket = new ClientPacket(PacketType.Screen);
            clientPacket.screen = new ClientPacket.Screen(POWER.ON);
            clientPacket.screen.mouse = new ClientPacket.Screen.Mouse();
            MOUSEEVENTF mouseeventf = 0;
            if (e.ChangedButton == MouseButton.Left)
            {
                mouseeventf |= e.ButtonState == MouseButtonState.Pressed ? MOUSEEVENTF.LEFTDOWN : MOUSEEVENTF.LEFTUP;
            }
            if (e.ChangedButton == MouseButton.Right)
            {
                mouseeventf |= e.ButtonState == MouseButtonState.Pressed ? MOUSEEVENTF.RIGHTDOWN : MOUSEEVENTF.RIGHTUP;
            }
            if (e.ChangedButton == MouseButton.Middle)
            {
                mouseeventf |= e.ButtonState == MouseButtonState.Pressed ? MOUSEEVENTF.MIDDLEDOWN : MOUSEEVENTF.MIDDLEUP;
            }
            clientPacket.screen.mouse.dwFlags = (int)mouseeventf;
            handler.packetStream.Send(clientPacket);
        }

        private void Screen_KeyBoard(object sender, KeyEventArgs e)
        {
            if (handler.tcpClient == null || handler.tcpClient.Client.Connected == false || power == POWER.OFF)
                return;

            ClientPacket clientPacket = new ClientPacket(PacketType.Screen);
            clientPacket.screen = new ClientPacket.Screen(POWER.ON);
            clientPacket.screen.keybd = new ClientPacket.Screen.Keybd();
            clientPacket.screen.keybd.bVk = (byte)e.Key;
            clientPacket.screen.keybd.bScan = 0;
            clientPacket.screen.keybd.dwFlags = (int)(e.KeyStates == KeyStates.None ? KEYEVENTF.KEYUP : 0);
            handler.packetStream.Send(clientPacket);
        }

        /// <summary>
        /// 参数设置
        /// </summary>
        /// <param name="power">开关</param>
        /// <param name="quailty">图像质量</param>
        public void Remote(POWER power, int quailty)
        {
            this.power = power;
            ClientPacket clientPacket = new ClientPacket(PacketType.Screen);
            clientPacket.screen = new ClientPacket.Screen(power);
            clientPacket.screen.Quality = quailty;
            if (handler.tcpClient == null || handler.tcpClient.Client.Connected == false)
                return;
            handler.packetStream.Send(clientPacket);
        }

        long old_time = 0;
        public void Recv(ServerPacket.Screen screen)
        {
            MemoryStream ms = new MemoryStream(screen.img);
            BitmapImage WpfBitmap = new BitmapImage();
            WpfBitmap.BeginInit();
            WpfBitmap.StreamSource = ms;
            WpfBitmap.EndInit();

            monitor.Source = WpfBitmap;
            //ImageBrush imagebrush = new ImageBrush(WpfBitmap);
            //new Rectangle().Fill = imagebrush;
            //new Canvas().Background = imagebrush;

            if (old_time != 0)
            {
                string fps = (10000000 / (DateTime.Now.Ticks - old_time)) + " fps";
                Title = "屏幕监控  " + handler.login_info.Addr + "  " + fps;
            }
            old_time = DateTime.Now.Ticks;
        }
    }
}
