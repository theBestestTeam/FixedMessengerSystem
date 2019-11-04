using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IPAddress localIP = IPAddress.Parse("127.0.0.1");
        int port = 8888;
        TcpClient tcpClient;
        NetworkStream networkStream;
        volatile bool connection = true;
        Thread threadFromOtherUsers;

        public MainWindow()
        {
            InitializeComponent();
            tcpClient = new TcpClient();
            tcpClient.Connect(localIP, port);
            networkStream = tcpClient.GetStream();
            threadFromOtherUsers = new Thread(() => IncomingMessages());
            threadFromOtherUsers.Start();
        }

        public void IncomingMessages()
        {
            while (connection == true)
            {
                byte[] receivingMessage = new byte[1024];
                networkStream.Read(receivingMessage, 0, receivingMessage.Length);
                this.Dispatcher.Invoke(() => { txtChatBox.Text += Encoding.ASCII.GetString(receivingMessage) + "\n\r"; });
            }
        }

        private void btnConnectUser_Click(object sender, RoutedEventArgs e)
        {
            string msgString = txtUsername.Text;
            byte[] sendName = Encoding.ASCII.GetBytes(msgString);
            networkStream.Write(sendName, 0, sendName.Length);
        }

        private void btnPauseUser_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnQuitUser_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string msgString = messageText.Text;
            messageText.Text = "";
            byte[] sendName = Encoding.ASCII.GetBytes(msgString);
            networkStream.Write(sendName, 0, sendName.Length);

        }
    }
}
