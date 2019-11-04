using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Server;

namespace Server.Handles
{

    class Users
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

        public Users(NetworkStream newStream, TcpClient newClient)
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
            catch(Exception overflow)
            {
                Console.WriteLine(overflow);
            }
        }

        public void ReceiveUsers(Dictionary<string, Users> keyValuePairs)
        {
            while(connection == true)
            {
                //Take recieved message, convert to string 
                byte[] recievedMessage = new byte[1024];
                userStream.Read(recievedMessage, 0, recievedMessage.Length);
                string convertedToString = Encoding.ASCII.GetString(recievedMessage).Trim('\0');

                //Store recieved messages from the user that sent message
                MessageHandler saveMessage = new MessageHandler(this, convertedToString);
                Server.messageQueue.Enqueue(saveMessage);
                Console.WriteLine($"{this.UserId} sent: " + convertedToString);

                //Left the chat code here
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
