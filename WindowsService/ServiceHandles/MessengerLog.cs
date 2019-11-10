/*
*	FILE			:	MessengerLog.cs
*	PROJECT			:	PROG2121 - Windows and Mobile Programming
*	PROGRAMMER		:	Amy Dayasundara, Paul Smith
*	FIRST VERSION	:	2019 - 10 - 01
*	DESCRIPTION		:	
*	                    This file will append to the existing text file that 
*	                    the service created. It will log everytime the user makes an interaction with
*	                    the service and report if it is successful or if there was an exception run.
*
*/

using System;
using System.IO;

namespace WindowsService.ServiceHandles
{
    public class MessengerLog
    {
        private string MessengerFileName;

        public MessengerLog(string fileName)
        {
            MessengerFileName = fileName;
        }

        public void addMessageToFile(string message)
        {
            try
            {
                File.AppendAllText(MessengerFileName, $"[{DateTime.Now.ToString("HH:MM")}]: {message}\n" + Environment.NewLine);
            }
            catch(Exception e)
            {
                File.AppendAllText(MessengerFileName, $"[{DateTime.Now.ToString("HH:MM")}]: {e}\n" + Environment.NewLine);
            }
        }
    }
}
