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

        private Dictionary<int, PadIntValue> _padInts;
        private int _lastCommittedtransID = 0;

        private Transaction _activeTransaction;
        private RemoteMasterInterface _master;
        private STATE _status;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public ServerRemote()
        {
            _clientURL_transid = new Hashtable();

            _padInts = new Dictionary<int, PadIntValue>();
            _status = STATE.ALIVE;
        }

        internal void regToMaster(int localport)
        {
            _ownURL = "tcp://localhost:" + localport + "/Server";
            _master = (RemoteMasterInterface)Activator.GetObject(
            typeof(RemoteMasterInterface),
            "tcp://localhost:" + Interfaces.Constants.MasterPort + "/master");
            _master.regServer(_ownURL);
        }

        public void execute(Request r)
        {
            PadIntValue value;
            if (_padInts.TryGetValue(r.involved(), out value))
            {
                if (r.isWrite())
                {
                    value.setValue(r.getVal());
                }
            }
            else throw new TxException("Tried to execute a request over padint " + r.involved() + " that doesnt exist on server " + _ownURL);
        }

        private void execute(List<Request> requests) 
        { 
        //execute active transaction if it can localy and then remotely 
            foreach (Request r in requests)
            {
                PadIntValue value;
                if (_padInts.TryGetValue(r.involved(), out value))
                {
                    if(r.isWrite())
                    {
                        value.setValue(r.getVal());
                        Console.WriteLine("Writing to padint " + value.getId() + " the value " + value.getValue());
                    }
                    else
                    {
                        Console.WriteLine("Reading padint " + value.getId() + "it has the value " + value.getValue());
                    }
                }
                else
                {
                    try
                    {
                        string serverURL = _master.getServer(r.involved());
                        RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
                        typeof(RemoteServerInterface), serverURL);
                        serv.execute(r);
                        Console.WriteLine("Padint not present on this server. Executing request on other servers...");
                    }
                    catch(TxException e)
                    {
                        Console.WriteLine(e.Message);
                        throw e;
                    }
                }
            }
        }

        private void requestTransID()
        { 
        //request to the master a Transaction ID
            _master.getTimeStamp();
        }

        private bool validate(Transaction t)
        { 
        //Start the validating proccess
            //TODO
            return true;
        }

        public void registerReplica(string url)
        { 
        //to be invoked by a replica
        }

        public bool commit(Transaction t)
        {
            execute(t.getRequests());
            return validate(t);
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
            catch(TxException e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        //Function that registers a padint on this server.
        public PadIntValue CreatePadInt(int uid)
        {
            try
            {
                _master = (RemoteMasterInterface)Activator.GetObject(
                typeof(RemoteMasterInterface),
                "tcp://localhost:" + Interfaces.Constants.MasterPort + "/master");
                if( _master.regPadint(uid, _ownURL))
                {
                    PadIntValue v = new PadIntValue(uid, 0);
                    _padInts.Add(uid, v);
                    Console.WriteLine("CreatedPadint: " + v.getId());
                    return v;
                }
                else
                {
                    //TODO
                    Console.WriteLine("Duplicated Padint");
                    throw new Exception();
                }
           }
            catch (TxException e)
           {
               Console.WriteLine("Error Creating Padint from master.");
                //throws either a new exception or the same returned from the master.
               throw new TxException("Error Creating Padint from master.");
           }
        }

        //Function that checks localy if the padint exists, if not it must ask master where it is located.
        public PadIntValue AcessPadInt(int uid)
        {
            try
            {
                //Checks to see if the padint is present locally.
	            if (_padInts.ContainsKey(uid)) 
	            {
                    return _padInts[uid];
	            }

                string serverURL = _master.getServer(uid);
                RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
                typeof(RemoteServerInterface), serverURL);

                return serv.AcessPadInt(uid);
            }
            catch(TxException e)
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
