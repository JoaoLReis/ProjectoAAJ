using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using Containers.source;

namespace Client
{
    class Client : MarshalByRefObject, RemoteClientInterface{
        private static Form1 form;
        private String urlMaster;
        private String urlServer;
        private String ownUrl;
        private List<PadInt> listPadInt;
        
        //Vale a pena criar uma entidade Request
        private List<String> listRequests;

        public void receive(Message msg)
        {

        }
    }
}
