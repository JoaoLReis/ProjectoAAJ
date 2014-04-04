using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using Containers;

namespace Server.source
{
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

        public ServerRemote(int localport)
        {
            _clientURL_transid = new Hashtable();
            _ownURL = "tcp://localhost:" + localport + "/obj";
            _padInts = new List<PadIntValue>();
            _master = (RemoteMasterInterface)Activator.GetObject(
                typeof(RemoteMasterInterface),
                "tcp://localhost:" + Interfaces.Constants.MasterPort + "/obj");
            _master.regServer(_ownURL);
        }

        private void sendToClient(string url, Message msg)
        {
            //send a message(to be invoked by itself)
            RemoteClientInterface client = (RemoteClientInterface)Activator.GetObject(
                typeof(RemoteClientInterface), url);
            client.receiveResponse(msg);
        }

        private void masterMsg(string msg)
        {

        }

        //formerly receive
        void receive(Message msg)
        {
        //receive a message(to be invoked by others)
            if (msg.getOwner() == _masterURL)
            {
                masterMsg(msg.getMessage());
            }
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

        void validate()
        { 
        //Start the validating proccess
        }

        void registerReplica(string url)
        { 
        //to be invoked by a replica
        }

        bool commit(Transaction t)
        { 
        //commit a transaction and send result to client
            return false;
        }

        private void write(int padintID)
        { 
        //write a padint
        }

        private PadInt read() 
        {
            return null;
        //read a padint
        }

        void abort()
        { 
        //abort a transaction
        }

        //Creates a transaction and generates a timestamp.
        Transaction begin()
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
        PadIntValue CreatePadInt(int uid)
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
        PadIntValue AcessPadInt(int uid)
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

        void status() 
        { 
        //
        }

        void fail() 
        { 
        //
        }

        void freeze() 
        { 
        //
        }

        void recover() 
        { 
        //
        }

    }
}
