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
            ClientChat.form = this;
        }

        private void URLMaster_TextChanged(object sender, EventArgs e)
        {

        }

        private void Connection_Click(object sender, EventArgs e)
        {
            TcpChannel client_channel = new TcpChannel(Int32.Parse(ClientPort.Text));
            ChannelServices.RegisterChannel(client_channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Client), "MyRemoteObjectName",
            WellKnownObjectMode.Singleton);
            RemoteMasterInterface s = (RemoteMasterInterface)Activator.GetObject(typeof(RemoteMasterInterface)
                , URLMaster.Text);

            s.requestServer("Falta criar URL do client");
        }

        private void ClientPort_TextChanged(object sender, EventArgs e)
        {

        }

        public class ClientChat : MarshalByRefObject, RemoteClientInterface
        {
            public static Form1 form;
        }
    }
}
