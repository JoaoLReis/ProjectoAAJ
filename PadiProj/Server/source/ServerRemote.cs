using System;
using System.Collections;
using System.Linq;
using System.Text;
using Interfaces;
using Containers;

namespace Server.source
{
    class ServerRemote : MarshalByRefObject, RemoteServerInterface
    {
        private Hashtable clientURL;
        private string ownURL;
        private string masterURL;

        private Array padInts;

        private int lastCommittedtransID;

        private string activeTransaction;

        public ServerRemote()
        {
            clientURL = new Hashtable();
        }

        void send()
        {
        //send a message(to be invoked by itself)
        }

        void receive(Message msg)
        {
        //receive a message(to be invoked by others)
        }

        void execute() 
        { 
        //execute active transaction
        }

        void requestTransID()
        { 
        //request to the master a Transaction ID
        }

        void validate()
        { 
        //Start the validating proccess
        }

        void registerClient() 
        { 
        //register a client and its transaction id in the hashtable
        }

        void registerReplica()
        { 
        //to be invoked by a replica
        }

        void commit()
        { 
        //commit a transaction and send result to client
        }

        void write(int padintID)
        { 
        //write a padint
        }

        PadInt read() 
        {
            return null;
        //read a padint
        }

        void abort()
        { 
        //abort a transaction
        }

        void begin() 
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
