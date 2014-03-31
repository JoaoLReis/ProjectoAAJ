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

        //Function that tries to register a padint, returns false if the padint already exists.
        public bool regPadint(int id, string server)
        {
            if (_serverPadInts.ContainsKey(id))
                return false;
            else 
            {
                _serverPadInts.Add(id, server);
                return true;
            }
        }

        //Gets a server from a padint ID.
        public string getServer(int id)
        {
            return _serverPadInts[id].ToString();
        }

        //Registers a server on master.
        public void regServer(string server)
        {
            _AvailableServer = server;
        }

        public void checkAvailability()
        {

        }

        public void geTransID()
        {

        }
    }
}
