/*
*	FILE			:	ServiceUser.cs
*	PROJECT			:	PROG2121 - Windows and Mobile Programming
*	PROGRAMMER		:	Amy Dayasundara, Paul Smith
*	FIRST VERSION	:	2019 - 10 - 01
*	DESCRIPTION		:	
*	                    The Service User will store any connections made. It will also
*	                    handle the message being sent to the server and how it will interact with
*	                    the server and clients
*
*/

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WindowsService.ServiceHandles
{
    public class ServiceUsers
    {
        NetworkStream userStream;
        TcpClient userClient;
        private string userId;
        private bool connection;

        public string UserId
        {
            get
            {
                return userId;
            }
            set
            {
                userId = value;
            }
        }

        public object MyService { get; private set; }

        public ServiceUsers(NetworkStream newStream, TcpClient newClient)
        {
            userStream = newStream;
            userClient = newClient;
            UserId = "New User";
            connection = true;
        }

        //send messages to user
        public void SendMessage(string Message)
        {
            try
            {
                byte[] message = Encoding.ASCII.GetBytes(Message);
                userStream.Write(message, 0, message.Length);
            }
            catch (Exception overflow)
            {
                Console.WriteLine(overflow);
            }
        }

        public void ReceiveUsers(Dictionary<string, ServiceUsers> keyValuePairs, MessengerLog messengerLog)
        {
            while (connection == true)
            {
                //Take recieved message, convert to string 
                byte[] recievedMessage = new byte[1024];
                userStream.Read(recievedMessage, 0, recievedMessage.Length);
                string convertedToString = Encoding.ASCII.GetString(recievedMessage).Trim('\0');

                //Left the chat code here
                //Make sure to remove the users information if they have closed out of the client
                if (convertedToString == "USERHASQUITAPP2256")
                {
                    lock (MessengerServer.lockDict)
                    {
                        connection = false;
                        messengerLog.addMessageToFile($"\n{this.UserId} has left the messenger.");
                        MessengerServer.connectedServiceUsers.Remove(this.UserId);
                        Thread.CurrentThread.Interrupt();
                        foreach (KeyValuePair<string, ServiceUsers> valuePair in keyValuePairs)
                        {
                            if (valuePair.Key != this.UserId)
                            {
                                valuePair.Value.SendMessage($"\n{this.UserId} has left messenger.");
                            }
                        }
                    }
                    continue;
                }
                //Check to see if the user has simply just pause their client and that they cannot
                //recieve any messages from the other users
                else if (convertedToString == "PAUSEUSERNOW")
                {
                    lock (MessengerServer.lockDict)
                    {
                        if(this.connection == true)
                        {
                            this.connection = false;
                            messengerLog.addMessageToFile($"\n{this.UserId} has went offline the messenger.");
                            foreach (KeyValuePair<string, ServiceUsers> valuePair in keyValuePairs)
                            {
                                if (valuePair.Key != this.UserId)
                                {
                                    valuePair.Value.SendMessage($"\n{this.UserId} has went offline messenger.");
                                }
                            }
                        }
                        else if(this.connection == false)
                        {
                            this.connection = true;
                            messengerLog.addMessageToFile($"\n{this.UserId} has come back online.");
                            foreach (KeyValuePair<string, ServiceUsers> valuePair in keyValuePairs)
                            {
                                if (valuePair.Key != this.UserId)
                                {
                                    valuePair.Value.SendMessage($"\n{this.UserId} has come back online.");
                                }
                            }
                        }
                    }
                    continue;
                }
                else
                {
                    //Store recieved messages from the user that sent message
                    ServiceMessageHandler saveMessage = new ServiceMessageHandler(this, convertedToString);
                    //storeMessage(saveMessage);
                    MessengerServer.messageQueue.Enqueue(saveMessage);
                    Console.WriteLine($"{this.UserId} sent: " + convertedToString);
                }


            }
        }

        public void UserNameStartup()
        {
            byte[] userName = new byte[1024];
            userStream.Read(userName, 0, userName.Length);
            string convertUserName = Encoding.ASCII.GetString(userName).Trim('\0');
            userId = convertUserName;
            Console.WriteLine($"{userId} has entered the chat.");
        }

    }
}
