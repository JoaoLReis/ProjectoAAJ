using System;
using System.Collections;
using System.Linq;
using System.Text;
using Interfaces;
using Containers;

namespace Master
{
    class MasterRemote : MarshalByRefObject, RemoteMasterInterface
    {
        private Hashtable _serverPadInts;

        private string _AvailableServer;

        public MasterRemote()
        {
            _serverPadInts = new Hashtable();
        }

        public void regPadint(int id, string server)
        {
            _serverPadInts.Add(id, server);
        }

        public bool exPadint(int id)
        {
            return _serverPadInts.ContainsKey(id);
        }

        public void regServer(string server)
        {
            _AvailableServer = server;
        }

        public void checkAvailability()
        {

        }
    }
}
