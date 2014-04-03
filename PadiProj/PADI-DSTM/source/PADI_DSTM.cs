using System;
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
        private RemoteMasterInterface _master;
        private RemoteServerInterface _server;
        private List<PadInt> _requests;
        private bool _inTransaction;

        public bool Init()
        {
            _requests = new List<PadInt>();

            //Gets master.
            try
            {
                _master = (RemoteMasterInterface)Activator.GetObject(
                typeof(RemoteMasterInterface),
                "tcp://localhost:" + Interfaces.RemoteMasterInterface.MasterPort + "/master");
            }
            catch(Exception e)
            {
                
            }
            //Requests a free server.
            try
            {
                _server = (RemoteServerInterface)Activator.GetObject(
                typeof(RemoteServerInterface),
                _master.requestServer());
            }
            catch(Exception e)
            {

            }

            _inTransaction = false;

            return true;
        }

        public bool TxBegin()
        {

            _inTransaction = true;
           // _curTran = new Transaction();
        }

        public bool TxCommit()
        {

        }

        public bool TxAbort()
        {

        }

        public bool Status()
        {
            try
            {
                return _master.Status();
            }
            catch (Exception e)
            {

            }
        }

        public bool Fail(string URL)
        {
            try
            {
                return _master.Fail(URL);
            }
            catch (Exception e)
            {

            }
        }

        public bool Freeze(string URL)
        {
            try
            {
                return _master.Freeze(URL);
            }
            catch (Exception e)
            {

            }
        }

        public bool Recover(string URL)
        {
            try
            {
                return _master.Recover(URL);
            }
            catch (Exception e)
            {

            }
        }//1001
        //server

        public PadInt CreatePadInt(int uid)
        {
            //Connect to the available server where he will try to create the padint localy and update its location on to the master.
            try 
            {            
                 PadIntValue v = _server.CreatePadInt(uid);
                return new PadInt(); 
            }
            catch(Exception e)
            {

            }
        }

        public PadInt AcessPadInt(int uid)
        {
            try
            {
                return _server.AcessPadInt(uid);
            }
            catch (Exception e)
            {

            }
        }
    }
}
