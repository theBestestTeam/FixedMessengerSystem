using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handles
{
    class MessageHandler
    {
        private Users sender;
        public Users Sender
        {
            get
            {
                return sender;
            }
            set
            {
                sender = value;
            }
        }

        private string messageQueue;
        public string MessageQueue
        {
            get
            {
                return messageQueue;
            }
            set
            {
                messageQueue = value;
            }
        }

        private string userId;
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

        public MessageHandler(Users sender, string conversation)
        {
            this.sender = sender;
            messageQueue = conversation;
            UserId = sender.UserId;
        }

    }
}
