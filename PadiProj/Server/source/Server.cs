using System;
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
        static int serverport = 1001;


        static void Main(string[] args)
        {
            int localport = serverport++;
            TcpChannel channel = new TcpChannel(localport);
            ChannelServices.RegisterChannel(channel, true);

            ServerRemote obj = new ServerRemote();
            obj.regToMaster(localport);
        
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(ServerRemote),
                "Server",
                WellKnownObjectMode.Singleton);

            System.Console.WriteLine("<enter> para sair...");
            System.Console.ReadLine();

        }
    }
}
