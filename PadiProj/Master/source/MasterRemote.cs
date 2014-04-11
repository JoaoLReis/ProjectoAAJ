using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using Containers;

namespace Master
{
    class MasterRemote : MarshalByRefObject, RemoteMasterInterface
    {
        private Dictionary<int, string> _serverPadInts;
        private List<string> _AvailableServer;

        private DateTime _lastTimeStamp;
        private Random rnd;

        private int _serverID;

        private bool _lockTaken;
        private DateTime CurrentDate;

        public MasterRemote()
        {
            _serverID = 0;
            rnd = new Random();
            _AvailableServer = new List<string>();
            _serverPadInts = new Dictionary<int, string>();
            _lockTaken = false;
        }

        public override object InitializeLifetimeService()
        {

            return null;

        }

        //Function that tries to register a padint, returns false if the padint already exists.
        public bool regPadint(int id, string server)
        {
            if (_serverPadInts.ContainsKey(id))
            {
                return false;
            }
            else 
            {
                _serverPadInts.Add(id, server);
                return true;
            }
        }

        //Gets a server from a padint ID.
        public string getServer(int id)
        {
            return _serverPadInts[id];
        }

        //Registers a server on master.
        public void regServer(string server)
        {
            Console.WriteLine("Registered server: " + server);
            _AvailableServer.Add(server);
        }

        //NOTE altering the timestamp of a transaction remotely alters it?!? or do we need to return it?!?
        public DateTime getTimeStamp()
        {
            DateTime tmp = genTimestamp();
            return tmp;
        }

        //Temporary function to generate a timestamp for a transaction.
        //TESTING REQUIRED!?!
        private DateTime genTimestamp()
        {
           /* try
            {
                System.Threading.Monitor.Enter(CurrentDate, ref _lockTaken);*/
                //CurrentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyyMMddHHmmssffff"));
                CurrentDate = DateTime.Now; 
                Console.WriteLine("Generated timestamp: " + CurrentDate.ToString());
                return CurrentDate;
            /*}
            finally
            {
                System.Threading.Monitor.Exit(CurrentDate);
            }  */
        }

        public string requestServer()
        {
            if(_AvailableServer.Count != 0)
            {
                
                int r = rnd.Next(_AvailableServer.Count);
                string URL = _AvailableServer[r];
                Console.WriteLine("Returning available server: " + URL);
                return URL;
            } else
            {
                //TODO
                throw new Exception();
            }

        }

        public bool status()
        {
            foreach (string URL in _AvailableServer)
            {
                try 
                {
                    RemoteServerInterface server = (RemoteServerInterface)Activator.GetObject(
                    typeof(RemoteServerInterface), URL);
                    server.status();
                }
                catch(Exception e)
                {

                    return false;
                }
            }
            return true;
        }

        public bool fail(string URL)
        {
            try
            {
                RemoteServerInterface server = (RemoteServerInterface)Activator.GetObject(
                typeof(RemoteServerInterface), URL);
                server.fail(); 
            }
            catch (Exception e)
            {

                return false;
            }
            return true;
        }

        public bool freeze(string URL)
        {
            try
            {
                RemoteServerInterface server = (RemoteServerInterface)Activator.GetObject(
                typeof(RemoteServerInterface), URL);
                server.freeze();
            }
            catch (Exception e)
            {

                return false;
            }
            return true;
        }

        public bool recover(string URL)
        {
            try
            {
                RemoteServerInterface server = (RemoteServerInterface)Activator.GetObject(
                typeof(RemoteServerInterface), URL);
                server.recover();
            }
            catch (Exception e)
            {

                return false;
            }
            return true;
        }
    }
}
