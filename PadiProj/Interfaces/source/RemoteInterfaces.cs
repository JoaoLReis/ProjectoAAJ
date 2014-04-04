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
        void receive(Message msg);
        
        void registerReplica(string url);

        void validate();

        void status();

        bool commit(Transaction t);

        bool abort();

        Transaction begin();

        void fail();

        void freeze();

        void recover();
        void registerClient(string url);
        PadIntValue CreatePadInt(int uid);
        PadIntValue AcessPadInt(int uid);
    }

    public interface RemoteClientInterface
    {
        void receiveResponse(Message msg);

        void receiveNotifications(Notification noti);

        void sendRequest(Message msg);
    }

    public interface RemoteMasterInterface
    {
        bool regPadint(int id, string server);
        int  regServer(string server);

        string getServer(int id);
        string requestServer();

        DateTime getTimeStamp();

        bool Status();
        bool Fail(string URL);
        bool Freeze(string URL);
        bool Recover(string URL);
    }
}
