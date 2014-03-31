﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Containers.source;

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
        void requestServer(String urlClient);
    }
}
