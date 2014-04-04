using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Interfaces;

namespace Master
{
    class Master
    {
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(Interfaces.Constants.MasterPort);
            ChannelServices.RegisterChannel(channel, true);

            RemoteMasterInterface obj = new MasterRemote();

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(RemoteMasterInterface),
                "master",
                WellKnownObjectMode.Singleton);

            System.Console.WriteLine("<enter> para sair...");
            System.Console.ReadLine();
        }
    }
}
