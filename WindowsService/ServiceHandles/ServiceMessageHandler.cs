/*
*	FILE			:	ServiceMessageHandler.cs
*	PROJECT			:	PROG2121 - Windows and Mobile Programming
*	PROGRAMMER		:	Amy Dayasundara, Paul Smith
*	FIRST VERSION	:	2019 - 10 - 01
*	DESCRIPTION		:	
*	                    This file is used to determine the users information and if
*	                    there is an ongoing conversation. It saves the string queue for the client
*	                    information
*
*/

namespace WindowsService.ServiceHandles
{
    class ServiceMessageHandler
    {
        private ServiceUsers sender;
        public ServiceUsers Sender
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

        public ServiceMessageHandler(ServiceUsers sender, string conversation)
        {
            this.sender = sender;
            messageQueue = conversation;
            UserId = sender.UserId;

        }
    }
}
