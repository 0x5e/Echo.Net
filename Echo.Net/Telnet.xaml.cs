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

namespace Echo.Net
{
    using System.Threading;
    using System.Net.Sockets;

    /// <summary>
    /// TelnetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Telnet : Window
    {
        //ServerPacket.Login login_info;
        Handler handler;

        public Telnet(Handler handler)
        {
            InitializeComponent();
            this.handler = handler;
            Show();
            cmd.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Send();
        }

        private void cmd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Send();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            handler.telnet = null;
        }

        public void Send()
        {
            ClientPacket clientPacket = new ClientPacket(PacketType.Telnet);
            clientPacket.telnet = new ClientPacket.Telnet(cmd.Text);
            handler.packetStream.Send(clientPacket);
            cmd.Text = "";
        }

        public void Recv(ServerPacket.Telnet telnet)
        {
            recvBox.Text += telnet.Response + "\n";
            recvBox.ScrollToEnd();
        }

    }
}
