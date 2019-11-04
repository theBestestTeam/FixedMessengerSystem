using Server.Handles;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ServerProg
    {

        static void Main(string[] args)
        {
            new Server().clientListener();
            Console.ReadLine();
        }
       
    }
}
