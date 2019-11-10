/*
*	FILE			:	MainWindow.xaml.cs
*	PROJECT			:	PROG2121 - Windows and Mobile Programming
*	PROGRAMMER		:	Amy Dayasundara, Paul Smith
*	FIRST VERSION	:	2019 - 10 - 01
*	DESCRIPTION		:	
*	                    This file contains the Client application. It will send the message and user
*	                    information to the service to be translated to the other users
*
*/

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

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


        //Pause user puts them in an offline condition
        //Currently removes the user from being able to message.
        //User can still recieve messages from online users
        //Cannot send back to online users
        //Issue with going FROM offline TO online
        private void btnPauseUser_Click(object sender, RoutedEventArgs e)
        {
            //Pause a user need to temporarily disconnet from the server
            //Have a withheld component for the user - Disconnected users
            //Change text to reconnect
            //Pause the string of the user so they cannot recieve any messages
            string pauseUser = "PAUSEUSERNOW";
            byte[] sendPause = Encoding.ASCII.GetBytes(pauseUser);
            networkStream.Write(sendPause, 0, sendPause.Length);
            //Reconnect the user to the service
            //Change the text back to Disconnect
            //Make sure the user can interact with the other users 
            //Add that the user is connected to the server
        }

        //User leaves the system and closes out
        //Somehow need to make the button quit WPF
        private void btnQuitUser_Click(object sender, RoutedEventArgs e)
        {
            //Quit user need to remove user from the Dictionary
            //Gracefully close their window
            //Need to log that the user has been successfully disconnected from the service
            try
            {
                string msgString = "USERHASQUITAPP2256";
                byte[] sendQuit = Encoding.ASCII.GetBytes(msgString);
                networkStream.Write(sendQuit, 0, sendQuit.Length);
            }
            catch(Exception e)
            {
                   
            }
            tcpClient.Close();
            threadFromOtherUsers.Abort();
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
