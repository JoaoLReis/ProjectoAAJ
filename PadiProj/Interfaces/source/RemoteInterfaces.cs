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
        void sendRequest(Message msg);
    }

    public interface RemoteClientInterface
    {
        void receive(Message msg);
    }

    public interface RemoteMasterInterface
    {
        public const int MasterPort = 8080;

        public bool regPadint(int id, string server);
        public void regServer(string server);

        public string getServer(int id);
        public string requestServer(String urlClient);

        public Transaction putTimeStamp(Transaction t);    
    }
}
