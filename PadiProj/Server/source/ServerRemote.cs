using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;

namespace Server.source
{
    class ServerRemote : MarshalByRefObject, RemoteServerInterface
    {
        private string clientURL;
        private string ownURL;

        private Array padInts;

        private int lastCommittedtransID;

        void send() {}
        void receive() { }
        void execute() { }
        void requestTransID() { }
        void validate() { }
        void registerClient() { }
        void registerReplica() { }
        void commit() { }
        void write() { }
        void read() { }
        void abort() { }
        void begin() { }
        void status() { }
        void fail() { }
        void freeze() { }
        void recover() { }

    }
}
