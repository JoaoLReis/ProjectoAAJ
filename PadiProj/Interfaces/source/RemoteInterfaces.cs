using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Containers.source;

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
        String requestServer(String urlClient);
    }
}
