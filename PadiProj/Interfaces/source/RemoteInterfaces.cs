﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Containers;


namespace Interfaces
{
    public static class Constants
    {
        public const int MasterPort = 8080;
    }

    public interface RemoteServerInterface
    {
      
        void registerReplica(string url);

        void execute(Request r);
        
        //void validate();

        void status();

        bool commit(Transaction t);

        bool abort();

        Transaction begin();

        void fail();

        void freeze();

        void recover();

        PadIntValue CreatePadInt(int uid);
        PadIntValue AcessPadInt(int uid);
    }

    public interface RemoteClientInterface
    {
        //void receiveNotifications(Notification noti);

        //void sendRequest(Message msg);
    }

    public interface RemoteMasterInterface
    {
        bool regPadint(int id, string server);
        void  regServer(string server);

        string getServer(int id);
        string requestServer();

        DateTime getTimeStamp();

        bool status();
        bool fail(string URL);
        bool freeze(string URL);
        bool recover(string URL);
    }
}
