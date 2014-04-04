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
using PADI_DSTM;
using PADI_DSTM.Library;

namespace Client
{
    public partial class Form1 : Form
    {
        ClientRemote cl = new ClientRemote();

        public Form1()
        {
            InitializeComponent();
            ClientRemote.form = this;
        }

        private void Connection_Click(object sender, EventArgs e)
        {
            
            cl.init("tcp://localhost:" + Int32.Parse(ClientPort.Text) + "/Client");

            TcpChannel client_channel = new TcpChannel(Int32.Parse(ClientPort.Text));
            ChannelServices.RegisterChannel(client_channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ClientRemote), "Client",
            WellKnownObjectMode.Singleton);

            PADI_DSTM.Library.Init();
        }

        private void transactionWrite_Click(object sender, EventArgs e)
        {
            cl.getPadInt(Int32.Parse(idPadIntBox.Text)).Write(Int32.Parse(valueBox.Text));
        }

        private void transactionRead_Click(object sender, EventArgs e)
        {
            cl.getPadInt(Int32.Parse(idPadIntBox.Text)).Read();
        }

        private void transactionSend_Click(object sender, EventArgs e)
        {
            PADI_DSTM.Library.TxCommit();
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
                cl.setListPadInt(PADI_DSTM.Library.CreatePadInt(Int32.Parse(padIntBox.Text)));
            }
        }

        private void acessPadInt_Click(object sender, EventArgs e)
        {
            cl.setListPadInt(PADI_DSTM.Library.AcessPadInt(Int32.Parse(padIntBox.Text)));
        }
    }
}
