using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Containers;


namespace Interfaces
{
    public interface RemoteServerInterface
    {
        void receive(Message msg);
        
        void registerReplica(string url);
        
        void validate();

        void status();

        void commit();

        void abort();

        void begin();

        void fail();

        void freeze();

        void recover();
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
        int regServer(string server);

        string getServer(int id);
        string requestServer(String urlClient);

        DateTime getTimeStamp();
    }
}
