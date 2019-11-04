using Server.Handles;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        public static Dictionary<string, Users> connectedUsers;
        // Thread safe Queue (FIFO) which keep client messages before deliver them
        //Safe to use in multi thread environments. Cannot use Queue.
        public static ConcurrentQueue<MessageHandler> messageQueue = new ConcurrentQueue<MessageHandler>();
        volatile bool connect;
        TcpListener server;
        public static Object lockDict;
        public static Users client;

        public Server()
        {
            connectedUsers = new Dictionary<string, Users>();
            //Need to make Listerner over the web
            IPAddress localIP = IPAddress.Parse("127.0.0.1");
            int port = 8888;
            server = new TcpListener(localIP, port);
            server.Start();
            StartThread();
            lockDict = new Object();
        }

        //Looks for Clients
        public void clientListener()
        {
            connect = true;
            while( connect == true)
            {
                TcpClient clientRunning = default;
                clientRunning = server.AcceptTcpClient();
                Thread clntThread = new Thread(() => IncomingClient(clientRunning));
                clntThread.Start();
            }
            connect = false;
        }

        //Sets up client to communicate through this server
        //Needs to check connection of the users
        private void IncomingClient(TcpClient clientRunning)
        {
            NetworkStream stream = clientRunning.GetStream();
            client = new Users(stream, clientRunning);
            //Check for connection and type 
            Task setUser = Task.Run(() => displayUsersConnected(client, stream));
        }

        private void StartThread()
        {
            Thread collectingConnection = new Thread(() => CollectingConnection());
            collectingConnection.Start();
        }

        private void CollectingConnection()
        {
            StoreMessage storage;
            
            while(connect == true)
            {
                MessageHandler message = default(MessageHandler);

                //Check to see if there are messages that need to be sent out
                //If Concurrent doesn't exist, there will be a null message trying to be passed
                if (messageQueue.TryDequeue(out message))
                {
                    storage = DistributeMessage;
                    lock (lockDict)
                    {
                        storage(message);
                    }
                }
            }
        }

        private void DistributeMessage(MessageHandler message)
        {
            foreach (KeyValuePair<string, Users> messagePair in connectedUsers)
            {
                if(messagePair.Key != message.UserId)
                {
                    messagePair.Value.SendMessage($"[{DateTime.Now.ToString("hh:mm")}] {message.Sender.UserId}: {message.MessageQueue}");
                }
            }
        }
        public void StartServer()
        {
            connect = true;

        }

        delegate void StoreMessage(MessageHandler message);

        private void displayUsersConnected(Users users, NetworkStream stream)
        {
            users.UserNameStartup();

            lock(lockDict)
            {
                if(!connectedUsers.ContainsKey(users.UserId))
                {
                    connectedUsers.Add(client.UserId, client);
                    showOnConsoleUsers(client);
                    Thread startUser = new Thread(() => client.ReceiveUsers(connectedUsers));
                    startUser.Start();
                }
            }
        }

        private void showOnConsoleUsers(Users client)
        {
            lock(lockDict)
            {
                if(connectedUsers.Count() > 1)
                {
                    foreach(KeyValuePair<string, Users> loginUser in connectedUsers)
                    {
                        if(loginUser.Key != client.UserId)
                        {
                            loginUser.Value.SendMessage($"{client.UserId} just joined the chat.");
                        }
                    }

                    client.SendMessage("Online users:");
                    foreach(KeyValuePair<string, Users> showCurrentOnline in connectedUsers)
                    {
                        if(showCurrentOnline.Key != client.UserId)
                        {
                            client.SendMessage($"-- {showCurrentOnline.Key}");
                        }
                    }
                }
                else
                {
                    client.SendMessage($"{client.UserId}, you're the only one logged in.");
                }
            }
        }
    }
}
