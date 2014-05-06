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

        void getTicket(int ticket);

        void prepare(Transaction t, string _coordinatorURL);
        void prepared(int ticket, string url, bool sucessfull);

        bool validate(int ticket);
        void validateLocal(Transaction t);
        void validated(int ticket, string url, bool successful);

        void status();

        void commitLocalChanges(int ticket);
        void commited(int ticket, string url, bool sucessfull);
        bool commit(Transaction t);

        bool abort(int ticket);

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
        bool warnServ(string url, Exception e);

        void broadcast(List<String> participants, int ticket);

        DateTime getTimeStamp();
        int getTicket();

        bool status();
        bool fail(string URL);
        bool freeze(string URL);
        bool recover(string URL);
    }
}
