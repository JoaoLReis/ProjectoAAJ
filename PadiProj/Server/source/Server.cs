﻿using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Interfaces;

namespace Server.source
{
    class Server
    {
        
        static void Main(string[] args)
        { 
            int localport;
            if (args.Length > 0)
            {
                System.Console.WriteLine(args[0]);
                localport = Convert.ToInt32(args[0]);
            } 
            else
            {
                System.Console.WriteLine("Please enter the desired port:");
                string port = System.Console.ReadLine();
                if (!(port == ""))
                    localport = Convert.ToInt32(port);
                else
                    localport = 1001;
            }

            TcpChannel channel = new TcpChannel(localport);
            ChannelServices.RegisterChannel(channel, true);

            ServerRemote obj = new ServerRemote();
            obj.regToMaster(localport);

            RemotingServices.Marshal(obj, "Server", typeof(ServerRemote));

            /*RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(ServerRemote),
                "Server",
                WellKnownObjectMode.Singleton);*/

            System.Console.WriteLine("<enter> para sair...");
            System.Console.ReadLine();

        }
    }
}
