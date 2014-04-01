using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using Containers.source;

namespace Client
{
    public class Client : MarshalByRefObject, RemoteClientInterface{
        public static Form1 form;
        private String urlMaster;
        private String urlServer;
        private String ownUrl;
        private List<PadInt> listPadInt;
        
        //Vale a pena criar uma entidade Request??
        private List<String> listRequests;

        public void receiveResponse(Message msg)
        {
            if (msg.getUrlType())
                this.setClientUrlServer("Mensagem tem de ter um atributo que é o Url do server para passar ao cliente");
            else
            {
                //if(PadInt == null)
                // throw new PadInt não existe 
                //else
                //listPadInt.Add("O que vier da Message");
            }
        }

        public void receiveNotification(Notification noti)
        {

        }

        internal void init(String url)
        {
            if (url == null)
                throw new System.ArgumentException("URL cannot be null", "url");
            this.ownUrl = url;
        }

        internal String getClientUrl()
        {
            return this.ownUrl;
        }

        internal String getClientUrlMaster()
        {
            return this.urlMaster;
        }

        internal String getClientUrlServer()
        {
            return this.urlServer;
        }

        internal void setClientUrlServer(String urlS)
        {
            if (urlS == null)
                throw new System.ArgumentException("URL of Server cannot be null", "urlS");
            this.urlServer = urlS;
        }
        
    }
}
