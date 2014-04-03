using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Containers;

namespace Interfaces
{
    //#define const int MasterPort = 8080;
    public interface RemoteServerInterface
    {
        void sendToServer(Message msg);
        void registerReplica(string url);
        void registerClient(string url);
        PadInt CreatePadInt(int uid);
        PadInt AcessPadInt(int uid);
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
