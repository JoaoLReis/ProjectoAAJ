using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Containers.source;
using Interfaces;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Client.form = this;
        }

        private void URLMaster_TextChanged(object sender, EventArgs e)
        {

        }

        private void Connection_Click(object sender, EventArgs e)
        {
            Client cl = new Client();
            cl.init("tcp://localhost:" + Int32.Parse(ClientPort.Text) + "/MyRemoteObjectName");

            TcpChannel client_channel = new TcpChannel(Int32.Parse(ClientPort.Text));
            ChannelServices.RegisterChannel(client_channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Client), "MyRemoteObjectName",
            WellKnownObjectMode.Singleton);
            RemoteMasterInterface master = (RemoteMasterInterface)Activator.GetObject(typeof(RemoteMasterInterface)
                , URLMaster.Text);

            cl.setClientUrlServer(master.requestServer(cl.getClientUrl()));
        }

        private void ClientPort_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
