/*
*	FILE			:	MyServices.cs
*	PROJECT			:	PROG2121 - Windows and Mobile Programming
*	PROGRAMMER		:	Amy Dayasundara, Paul Smith
*	FIRST VERSION	:	2019 - 10 - 01
*	DESCRIPTION		:	
*	                    This start the application and run the service through the system task manager.
*
*/

using System;
using System.IO;
using System.ServiceProcess;
using WindowsService.ServiceHandles;

namespace WindowsService
{
    public partial class MyServices : ServiceBase
    {
        string fileName = "MessengerLog.txt";

        public MyServices()
        {
            InitializeComponent();
            new MessengerServer(new MessengerLog(fileName)).clientListener();
        }

        public void OnDebug()
        {
            OnStart(null);
        }
        protected override void OnStart(string[] args)
        {
            try
            {
                File.Create(AppDomain.CurrentDomain.BaseDirectory + fileName);            
            }
            catch(Exception e)
            {
                File.Create(AppDomain.CurrentDomain.BaseDirectory + "Error.txt");
                File.AppendAllText("Error.txt", "Error at MyService.cs");

            }
        }

        protected override void OnStop()
        {
            File.Create(AppDomain.CurrentDomain.BaseDirectory + "OnStop.Txt");
        }
    }
}
