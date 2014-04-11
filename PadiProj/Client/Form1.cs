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
            cl.init();
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
            PadiDstm.TxCommit();
        }

        public void showMessages(String msg)
        {
            receiveMessages.Text += msg + "\r\n";
        }

        private void createPadInt_Click(object sender, EventArgs e)
        {
            if (padIntBox.Text == "")
                showMessages("Para criar um PadInt tem de colocar o seu valor");
            else
            {
                cl.setListPadInt(PadiDstm.CreatePadInt(Int32.Parse(padIntBox.Text)));
            }
        }

        private void acessPadInt_Click(object sender, EventArgs e)
        {
            cl.setListPadInt(PadiDstm.AccessPadInt(Int32.Parse(padIntBox.Text)));
        }

        private void ShowAllPadInts_Click(object sender, EventArgs e)
        {
            foreach(PadInt p in cl.getPadIntList())
            {
                showMessages("ID: " + p.getId() + "\r\n" + "Value: " + p.getAbsValue());
            }
        }

        private void Begin(object sender, EventArgs e)
        {
            PadiDstm.TxBegin();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
