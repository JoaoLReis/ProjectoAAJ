using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using Containers;

namespace Server.source
{
     public enum STATE { FROZEN, ALIVE, FAILED };

    class ServerRemote : MarshalByRefObject, RemoteServerInterface
    {
        //enum CM {CLIENT, MASTER};
        private Hashtable _clientURL_transid;
        private string _clientURLs;

        private string _ownURL;
        private string _replicaURL;
        private string _masterURL;

        private List<PadIntValue> _padInts;

        private int _lastCommittedtransID = 0;

        private Transaction _activeTransaction;

        private RemoteMasterInterface _master;

        private STATE _status;

        public ServerRemote(int localport)
        {
            _clientURL_transid = new Hashtable();
            _ownURL = "tcp://localhost:" + localport + "/obj";
            _padInts = new List<PadIntValue>();
            _status = STATE.ALIVE;
        }

        internal void regToMaster()
        {
            _master = (RemoteMasterInterface)Activator.GetObject(
            typeof(RemoteMasterInterface),
            "tcp://localhost:" + Interfaces.Constants.MasterPort + "/master");
            _master.regServer(_ownURL);
        }

        private void execute() 
        { 
        //execute active transaction
        }

        private void requestTransID()
        { 
        //request to the master a Transaction ID
            _master.getTimeStamp();
        }

        public void validate()
        { 
        //Start the validating proccess
        }

        public void registerReplica(string url)
        { 
        //to be invoked by a replica
        }

        public bool commit(Transaction t)
        { 
        //commit a transaction and send result to client
            return false;
        }

        /*private void write(int padintID)
        { 
        //write a padint
        }

        private PadInt read() 
        {
            return null;
        //read a padint
        }*/

        public bool abort()
        { 
        //abort a transaction
            return false;
        }

        //Creates a transaction and generates a timestamp.
        public Transaction begin()
        {
            try
            {
                DateTime dt = _master.getTimeStamp();
                return( new Transaction(dt, null));
            }
            catch(Exception e)
            {
                //test if its needed to catch and rethrow an exception.
                return null;
            }
        }

        //Function that registers a padint on this server.
        public PadIntValue CreatePadInt(int uid)
        {
            try
            {
                //Registers on master.
                _master.regPadint(uid, _ownURL);
                PadIntValue v = new PadIntValue(uid, 0); 
                _padInts.Add(v);
                return v;
            }
            catch(Exception e)
            {
                //throws either a new exception or the same returned from the master.
                return null;
            }
        }

        //Function that checks localy if the padint exists, if not it must ask master where it is located.
        public PadIntValue AcessPadInt(int uid)
        {
            try
            {
                //Checks to see if the padint is present locally.
	            foreach (PadIntValue v in _padInts) 
	            {
	                if (uid == v.getId());
                        return v;
	            }

                string serverURL = _master.getServer(uid);
                RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
                typeof(RemoteServerInterface), serverURL);

                return serv.AcessPadInt(uid);
            }
            catch(Exception e)
            {
                //throws either a new exception or the same returned from the master.
                return null;
            }
        }

        public void status() 
        {
            if(_status == STATE.ALIVE)
                Console.Out.WriteLine("Server at: " + _ownURL + "STATE.ALIVE");
            else if (_status == STATE.FAILED)
                Console.Out.WriteLine("Server at: " + _ownURL + "STATE.FAILED");
            else if (_status == STATE.FROZEN)
                Console.Out.WriteLine("Server at: " + _ownURL + "STATE.FROZEN");
        }

        public void fail() 
        {
            _status = STATE.FAILED;
        }

        public void freeze() 
        {
            _status = STATE.FROZEN;
        }

        public void recover() 
        {
            _status = STATE.ALIVE;
        }
    }
}
