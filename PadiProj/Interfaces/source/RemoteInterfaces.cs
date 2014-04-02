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
    }

    public interface RemoteClientInterface
    {
        void receiveResponse(Message msg);

        void receiveNotifications(Notification noti);

        void sendRequest(Message msg);
    }

    public interface RemoteMasterInterface
    {
        public const int MasterPort = 8080;

        public bool regPadint(int id, string server);
        public int regServer(string server);

        public string getServer(int id);
        public string requestServer(String urlClient);

        public DateTime getTimeStamp();
    }
}
