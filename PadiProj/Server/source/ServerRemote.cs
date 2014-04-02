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
        private Hashtable clientURL_transid;
        private string clientURLs;

        private string ownURL;
        private string replicaURL;
        private string masterURL;

        private List<PadInt> padInts;

        private int lastCommittedtransID = 0;

        private Transaction activeTransaction;

        private RemoteMasterInterface master;

        public ServerRemote(int localport)
        {
            clientURL_transid = new Hashtable();
            ownURL = "tcp://localhost:" + localport + "/obj";
            padInts = new List<PadInt>();
            master = (RemoteMasterInterface)Activator.GetObject(
                typeof(RemoteMasterInterface),
                "tcp://localhost:" + RemoteMasterInterface.MasterPort + "/obj");
            master.regServer(ownURL);
        }

        private void sendToClient(string url, Message msg)
        {
            //send a message(to be invoked by itself)
            RemoteClientInterface client = (RemoteClientInterface)Activator.GetObject(
                typeof(RemoteClientInterface), url);
            client.receiveResponse(msg);
        }

        //formerly receive
        void receive(Message msg)
        {
        //receive a message(to be invoked by others)
        }

        private void execute() 
        { 
        //execute active transaction
        }

        private void requestTransID()
        { 
        //request to the master a Transaction ID
        }

        void validate()
        { 
        //Start the validating proccess
        }

        void registerReplica(string url)
        { 
        //to be invoked by a replica
        }

        void commit()
        { 
        //commit a transaction and send result to client
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

        private void begin() 
        { 
        //begin a transaction
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
