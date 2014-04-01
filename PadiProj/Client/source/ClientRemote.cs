using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using Containers;

namespace Client
{
    public class ClientRemote : MarshalByRefObject, RemoteClientInterface, RemoteServerInterface{
        
        public static Form1 form;
        private String urlMaster;
        private String urlServer;
        private String ownUrl;
        private List<PadInt> listPadInt;
        private List<Request> listRequests;

        delegate void delRSDV(String msg);

        public void receiveResponse(Message msg)
        {
            if (msg.getUrlType())
                this.setClientUrlServer(msg.getMessage());
            else
            {
                form.Invoke(new delRSDV(form.showMessages), new Object[] { msg });
            }
        }

        public void receiveNotification(Notification noti)
        {

        }

        internal void createNewPadInt(int id, int value)
        {
            this.listPadInt.Add(new PadInt(id, value));
        }
        internal void init(String url)
        {
            if (url == null)
                throw new System.ArgumentException("URL cannot be null", "url");
            this.ownUrl = url;
        }

        internal void setClientUrlServer(String urlS)
        {
            if (urlS == null)
                throw new System.ArgumentException("URL of Server cannot be null", "urlS");
            this.urlServer = urlS;
        }

        internal void setListOfRequests(bool write, int padInt, int val)
        {
            this.listRequests.Add(new Request(write, padInt, val));
        }

        internal List<Request> getListRequests()
        {
            return this.listRequests;
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
    }
}
