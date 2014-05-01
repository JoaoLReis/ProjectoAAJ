using System;
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
      
        void registerReplica();

        void prepare(Transaction t, string _coordinatorURL);
        void prepared(string url, bool sucessfull);

        //void validate();

        void status();

        void commitLocalChanges();
        void commited(string url, bool sucessfull);
        bool commit(Transaction t);

        bool abort(Transaction t);

        Transaction begin();

        void fail();
        void freeze();
        void recover();

        PadIntValue CreatePadInt(int uid);
        PadIntValue AccessPadInt(int uid);
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
        string requestServerReplica(string urlRequestServer);

        bool safeServ(string url);
        bool warnServ(string url);

        DateTime getTimeStamp();
        int getTicket();

        bool status();
        bool fail(string URL);
        bool freeze(string URL);
        bool recover(string URL);
    }
}
