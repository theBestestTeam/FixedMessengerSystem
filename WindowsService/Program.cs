/*
*	FILE			:	Program.cs
*	PROJECT			:	PROG2121 - Windows and Mobile Programming
*	PROGRAMMER		:	Amy Dayasundara, Paul Smith
*	FIRST VERSION	:	2019 - 10 - 01
*	DESCRIPTION		:	Contains the initial Release Service and the debug mode.
*
*/

using System.ServiceProcess;

namespace WindowsService
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //Check to make sure the service is working
            //This goes through the service and creates a file inside the debug
            //folder
#if DEBUG
            MyServices myServices = new MyServices();
            myServices.OnDebug();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

#else
            //When in release mode it it runs the services

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new MyServices()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
