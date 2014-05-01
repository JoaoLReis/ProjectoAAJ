using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using Containers;
using System.Threading;

namespace Master
{
    class MasterRemote : MarshalByRefObject, RemoteMasterInterface
    {
        private Dictionary<int, string> _serverPadInts;
        private List<string> _AvailableServer;

        private DateTime _lastTimeStamp;
        private Random rnd;

        private int _serverID;
        private DateTime CurrentDate;

        private int _commitTicket;

        public MasterRemote()
        {
            _serverID = 0;
            rnd = new Random();
            _AvailableServer = new List<string>();
            _serverPadInts = new Dictionary<int, string>();
            CurrentDate = new DateTime(0);
            _commitTicket = 1;
        }

        //Sets the lifetime of this object to indefinite.
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
            DateTime dt = new DateTime(0);

            System.Object obj = (System.Object)CurrentDate;
            System.Threading.Monitor.Enter(obj);
            try
            {
                //dt = DateTime.Now;
                //CurrentDate = Convert.ToDateTime((DateTime.Now.ToString("yyyyMMddHHmmssffff"));
                CurrentDate = DateTime.Now;
                Console.WriteLine("Generated timestamp: " + CurrentDate.ToString() + " mili: " + CurrentDate.Millisecond);
                //Locks during a milisecond.
                return CurrentDate;
            }
            finally
            {
                Thread.Sleep(1000);
                System.Threading.Monitor.Exit(obj);
            }
            
        }

        //Returns a ticket for a commit.
        public int getTicket()
        {
            return _commitTicket++;
        }

        //Sets server at url safe again.
        public bool safeServ(string url)
        {
            if (_AvailableServer.Contains(url))
            {
                return false;
            }
            _AvailableServer.Add(url);
            return true;
        }

        //Removes server at url from the available server.
        public bool warnServ(string url)
        {
            if (_AvailableServer.Contains(url))
            {
                Console.WriteLine("Server at: "+ url +" is unavailable removing from available.");
                _AvailableServer.Remove(url);
                return true;
            }
            else return false;
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

        public string requestServerReplica(string urlRequestServer)
        {
            int i;

            for (i=0; i < _AvailableServer.Count; i++)
            {
                if (i == _AvailableServer.Count - 1)
                    return _AvailableServer[0];
                else if (_AvailableServer[i] == urlRequestServer)
                    return _AvailableServer[i++];
            }
            throw new Exception();
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
