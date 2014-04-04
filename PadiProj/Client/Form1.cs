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
using Containers;
using Interfaces;

namespace Client
{
    public partial class Form1 : Form
    {
        private int idPadInt = 0;

        ClientRemote cl = new ClientRemote();

        public Form1()
        {
            InitializeComponent();
            ClientRemote.form = this;
        }

        private void URLMaster_TextChanged(object sender, EventArgs e)
        {

        }

        private void Connection_Click(object sender, EventArgs e)
        {
            
            cl.init("tcp://localhost:" + Int32.Parse(ClientPort.Text) + "/MyRemoteObjectName");

            TcpChannel client_channel = new TcpChannel(Int32.Parse(ClientPort.Text));
            ChannelServices.RegisterChannel(client_channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ClientRemote), "MyRemoteObjectName",
            WellKnownObjectMode.Singleton);
            RemoteMasterInterface master = (RemoteMasterInterface)Activator.GetObject(typeof(RemoteMasterInterface)
                , URLMaster.Text);

            cl.setClientUrlServer(master.requestServer(cl.getClientUrl()));
        }

        private void ClientPort_TextChanged(object sender, EventArgs e)
        {

        }

        private void transactionWrite_Click(object sender, EventArgs e)
        {
            cl.setListOfRequests(true, (Int32.Parse(idPadIntBox.Text)), (Int32.Parse(valueBox.Text)));
        }

        private void transactionRead_Click(object sender, EventArgs e)
        {
            cl.setListOfRequests(false, (Int32.Parse(idPadIntBox.Text)), (Int32.Parse(valueBox.Text)));
        }

        private void transactionSend_Click(object sender, EventArgs e)
        {
            RemoteServerInterface server = (RemoteServerInterface)Activator.GetObject(typeof(RemoteServerInterface)
                , cl.getClientUrlServer());

            //server.sendTransaction(cl.getListRequests());
        }

        public void showMessages(String msg)
        {
            receiveMessages.Text += "\r\n" + msg;
        }

        private void createPadInt_Click(object sender, EventArgs e)
        {
            if (padIntBox.Text == null)
                showMessages("Para criar um PadInt tem de colocar o seu valor");
            else
            {
                idPadInt += idPadInt++;
                cl.createNewPadInt(idPadInt - 1, (Int32.Parse(padIntBox.Text)));
            }
        }
    }
}
