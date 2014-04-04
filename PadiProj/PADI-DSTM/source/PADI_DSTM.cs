﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Containers;
using Interfaces;

namespace PADI_DSTM
{
    public class Library
    {
        private static RemoteMasterInterface _master;
        private static RemoteServerInterface _server;
        private static Transaction _curTrans;
        private static bool _inTransaction;
        private static List<PadInt> _acessedPadInts;

        //Inicializes a new List of Padints, and requests acess to the master and obtains an available server from it.
        public static bool Init()
        {
            _acessedPadInts = new List<PadInt>();

            //Gets master.
            /*try
            {*/
                _master = (RemoteMasterInterface)Activator.GetObject(
                typeof(RemoteMasterInterface),
                "tcp://localhost:" + Interfaces.Constants.MasterPort + "/master");
            /*}
            catch(Exception e)
            {
                return false;
            }*/
            //Requests a free server.
            /*try
            {*/
                string url = _master.requestServer();
                _server = (RemoteServerInterface)Activator.GetObject(
                typeof(RemoteServerInterface),
                url);
                _server.status();
            /*}
            catch(Exception e)
            {
                return false;
            }*/

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
            catch (Exception e)
            {
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
            catch(Exception e)
            {
                return false;
            }
        }

        //Aborts a transaction.
        public static bool TxAbort()
        {
            try
            {
                _inTransaction = false;
                return _server.abort();
            }
            catch (Exception e)
            {
                //TODO
                throw new Exception();
            }
        }

        public static bool Status()
        {
            try
            {
                return _master.status();
            }
            catch (Exception e)
            {
                //TODO
                throw new Exception();
            }
        }

        public static bool Fail(string URL)
        {
            try
            {
                return _master.fail(URL);
            }
            catch (Exception e)
            {
                //TODO
                throw new Exception();
            }
        }

        public static bool Freeze(string URL)
        {
            try
            {
                return _master.freeze(URL);
            }
            catch (Exception e)
            {
                //TODO
                throw new Exception();
            }
        }

        public static bool Recover(string URL)
        {
            try
            {
                return _master.recover(URL);
            }
            catch (Exception e)
            {
                //TODO
                throw new Exception();
            }
        }

        public static PadInt CreatePadInt(int uid)
        {
            //Connect to the available server where he will try to create the padint localy and update its location on to the master.
            PadIntValue val;
            try 
            {            
                val = _server.CreatePadInt(uid);
            }
            catch(Exception e)
            {
                //TODO
                throw new Exception();
            }
            PadInt v = new PadInt(val);
            _acessedPadInts.Add(v);
            return v; 
        }

        public static PadInt AcessPadInt(int uid)
        {
            PadIntValue val;
            try
            {
                val = _server.AcessPadInt(uid);
            }
            catch (Exception e)
            {
                //TODO
                throw new Exception();
            }
            PadInt v = new PadInt(val);
            _acessedPadInts.Add(v);
            return v; 
        }
    }
}
