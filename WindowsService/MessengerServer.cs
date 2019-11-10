/*
*	FILE			:	MessengerServer.cs
*	PROJECT			:	PROG2121 - Windows and Mobile Programming
*	PROGRAMMER		:	Amy Dayasundara, Paul Smith
*	FIRST VERSION	:	2019 - 10 - 01
*	DESCRIPTION		:	
*                       This file contains the initial startup of the program.
*                       This sets up the server that the client will communicate through
*                       using the TcpListerner.
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WindowsService.ServiceHandles;

namespace WindowsService
{
    class MessengerServer
    {
        public static Dictionary<string, ServiceUsers> connectedServiceUsers;
        // Thread safe Queue (FIFO) which keep client messages before deliver them
        //Safe to use in multi thread environments. Cannot use Queue.
        public static ConcurrentQueue<ServiceMessageHandler> messageQueue = new ConcurrentQueue<ServiceMessageHandler>();
        volatile bool connect;
        TcpListener server;
        public static Object lockDict;
        public static ServiceUsers client;
        private MessengerLog log;

        public MessengerServer(MessengerLog messengerLog)
        {
            this.log = messengerLog;
            
            connectedServiceUsers = new Dictionary<string, ServiceUsers>();
            //Need to make Listerner over the web
            IPAddress localIP = IPAddress.Parse("127.0.0.1");
            int port = 8888;
            server = new TcpListener(localIP, port);
            server.Start();
            StartThread();
            lockDict = new Object();
        }

        //
        //METHOD      : clientListener
        //DESCRIPTION   :
        //  Listening for Clients
        //PARAMETERS:
        //  None
        //Ex    double purchaseAmount : untaxed amount
        //RETURN:
        //  None
        //
        public void clientListener()
        {
            connect = true;
            while (connect == true)
            {
                TcpClient clientRunning = default;
                clientRunning = server.AcceptTcpClient();
                Thread clntThread = new Thread(() => IncomingClient(clientRunning));
                clntThread.Start();
            }
            connect = false; //Dafuq did I do here???
        }

        //Sets up client to communicate through this server
        //Needs to check connection of the ServiceUsers
        private void IncomingClient(TcpClient clientRunning)
        {
            NetworkStream stream = clientRunning.GetStream();
            client = new ServiceUsers(stream, clientRunning);
            //Check for connection and type 
            Task setUser = Task.Run(() => displayServiceUsersConnected(client, stream));
        }

        private void StartThread()
        {
            Thread collectingConnection = new Thread(() => CollectingConnection());
            collectingConnection.Start();
        }

        private void CollectingConnection()
        {
            StoreMessage storage;

            while (connect == true)
            {
                ServiceMessageHandler message = default(ServiceMessageHandler);

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

        private void DistributeMessage(ServiceMessageHandler message)
        {
            foreach (KeyValuePair<string, ServiceUsers> messagePair in connectedServiceUsers)
            {
                if (messagePair.Key != message.UserId)
                {
                    messagePair.Value.SendMessage($"[{DateTime.Now.ToString("hh:mm")}] {message.Sender.UserId}: {message.MessageQueue}");
                }
            }
        }

        //Upon start up of the user sets the connection to true
        public void StartServer()
        {
            connect = true;

        }

        delegate void StoreMessage(ServiceMessageHandler message);

        private void displayServiceUsersConnected(ServiceUsers ServiceUsers, NetworkStream stream)
        {
            ServiceUsers.UserNameStartup();

            lock (lockDict)
            {
                if (!connectedServiceUsers.ContainsKey(ServiceUsers.UserId))
                {
                    connectedServiceUsers.Add(client.UserId, client);
                    showOnConsoleServiceUsers(client);
                    Thread startUser = new Thread(() => client.ReceiveUsers(connectedServiceUsers, log));
                    startUser.Start();
                }
            }
        }

        private void showOnConsoleServiceUsers(ServiceUsers client)
        {
            lock (lockDict)
            {
                //To connect more than one user into the server
                if (connectedServiceUsers.Count() > 1)
                {

                    foreach (KeyValuePair<string, ServiceUsers> loginUser in connectedServiceUsers)
                    {
                        if (loginUser.Key != client.UserId)
                        {
                            loginUser.Value.SendMessage($"{client.UserId} just joined the chat.");
                            log.addMessageToFile($"{client.UserId} - successfully logged into server.");
                        }
                    }

                    client.SendMessage("Online Users:");
                    foreach (KeyValuePair<string, ServiceUsers> showCurrentOnline in connectedServiceUsers)
                    {
                        if (showCurrentOnline.Key != client.UserId)
                        {
                            client.SendMessage($"-- {showCurrentOnline.Key}");
                            log.addMessageToFile($"{client.UserId} - successfully connected to other client(s).");
                        }
                        
                    }
                }
                else
                {
                    client.SendMessage($"{client.UserId}, you're the only one logged in.");
                    log.addMessageToFile($"{client.UserId} - successfully logged in.");
                }
            }
        }
    }

}