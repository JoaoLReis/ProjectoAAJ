using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Net.NetworkInformation;
using System.Net;
using Containers;
using Interfaces;

namespace PADI_DSTM
{
    public class PadiDstm
    {
        private static RemoteMasterInterface _master;
        private static RemoteServerInterface _server;
        private static Transaction _curTrans;
        private static bool _inTransaction;
        private static List<PadInt> _acessedPadInts;
        private static string _curServer;

        internal static bool CheckAvailableServerPort(int port)
        {
            bool isAvailable = true;

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endpoint in tcpConnInfoArray)
            {
                if (endpoint.Port == port)
                {
                    isAvailable = false;
                    break;
                }
            }
            return isAvailable;
        }

        internal static bool registerClient(int port)
        {
            try
            {
                TcpChannel client_channel = new TcpChannel(port);
                ChannelServices.RegisterChannel(client_channel, true);
            }
            catch(Exception e)
            {
                //TODO
                return false;
            }
            return true;
        }

        public static bool requestServer()
        {
            //Requests a free server.
            try
            {
                _curServer = _master.requestServer();
                _server = (RemoteServerInterface)Activator.GetObject(
                typeof(RemoteServerInterface),
                _curServer);
            }
            catch (TxException e)
            {
                return false;
            }
            return true;
        }

        //Inicializes a new List of Padints, and requests acess to the master and obtains an available server from it.
        public static bool Init()
        {
            _acessedPadInts = new List<PadInt>();

            //Finds the first port available from portMin to portMax and register a client there.
            int portMin = 2000, portMax = 4000;
            for (int i = portMin; i < portMax; i++)
            {
                if (CheckAvailableServerPort(i))
                {
                    registerClient(i);
                    break;
                }
            }
            //Gets master.
            try
            {
                _master = (RemoteMasterInterface)Activator.GetObject(
                typeof(RemoteMasterInterface),
                "tcp://localhost:" + Interfaces.Constants.MasterPort + "/master");
            }
            catch(TxException e)
            {
                return false;
            }
            if (!requestServer())
                return false;

            _inTransaction = false;

            return true;
        }

        //Invokes the creation of a transaction on the available server, the available server will request the master
        //to generate an ID for the transaction.
        public static bool TxBegin()
        {             
            try
            {
                _curTrans = _server.begin();
                _inTransaction = true;
                return true;
            }
            catch(RemotingException re)
            {
                return false;
            }
            catch (TxException e)
            {
                //TODO.
                return false;
            }
        }

        //Iterates all padints created or acessed within a transaction and concatenates all reads and writes into a single 
        //request list, it then sends this list to the available server.
        public static bool TxCommit()
        {
            try
            {
                List<Request> l = new List<Request>();
                foreach (PadInt v in _acessedPadInts)
                {
                    l.AddRange(v.getRequests());
                }
                _curTrans.setRequests(l);
                _server = (RemoteServerInterface)Activator.GetObject(
                typeof(RemoteServerInterface),
                _curServer);
                if (_server.commit(_curTrans))
                {
                    _acessedPadInts.Clear();
                    return true;
                } 
                else
                {
                    return false;
                }
            }
            //TODO make this generic, accepting a set number of fails.
            catch (Exception e)
            {
                if (e is FreezeException || e is FailException || e is RemotingException || e is TxException)
                {
                    try
                    {
                        //Warns master that this server is unavailable.
                        if (_master.warnServ(_curServer))
                        {
                            //Requests another available server.
                            if (requestServer())
                            {
                                _server = (RemoteServerInterface)Activator.GetObject(
                                typeof(RemoteServerInterface),
                                _curServer);
                                if (_server.commit(_curTrans))
                                {
                                    _acessedPadInts.Clear();
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else return false;
                        }
                        else return false;
                    }
                    catch (Exception ne)
                    {
                        return false;
                    }
                }
                return false;
            }
        }

        //Aborts a transaction.
        public static bool TxAbort()
        {
            try
            {
                _inTransaction = false;
                return _server.abort(_curTrans);
            }
            catch (TxException e)
            {
                return false;
            }
        }

        public static bool Status()
        {
            try
            {
                return _master.status();
            }
            catch (TxException e)
            {
                return false;
            }
        }

        public static bool Fail(string URL)
        {
            try
            {
                return _master.fail(URL);
            }
            catch (TxException e)
            {
                return false;
            }
        }

        public static bool Freeze(string URL)
        {
            try
            {
                return _master.freeze(URL);
            }
            catch (TxException e)
            {
                return false;
            }
        }

        public static bool Recover(string URL)
        {
            try
            {
                return _master.recover(URL);
            }
            catch (TxException e)
            {
                return false;
            }
        }

        public static PadInt CreatePadInt(int uid)
        {
            //Connect to the available server where he will try to create the padint localy and update its location on to the master.
            try 
            {
                PadIntValue val;
                val = _server.CreatePadInt(uid);
                PadInt v = new PadInt(val);
                _acessedPadInts.Add(v);
                return v; 
            }
            catch (TxException e)
            {
                throw e;
            }
        }

        public static PadInt AccessPadInt(int uid)
        {
            try
            {
                PadIntValue val;
                val = _server.AccessPadInt(uid);
                PadInt v = new PadInt(val);
                _acessedPadInts.Add(v);
                return v; 
            }
            catch (TxException e)
            {
                throw e;
            }
        }
    }
}
