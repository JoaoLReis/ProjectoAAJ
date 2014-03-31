using System;
using System.Collections;
using System.Linq;
using System.Text;
using Interfaces;
using Containers;

namespace Master
{
    class MasterRemote : MarshalByRefObject, RemoteMasterInterface
    {
        private Hashtable _serverPadInts;

        private string _AvailableServer;

        public MasterRemote()
        {
            _serverPadInts = new Hashtable();
        }

        //Function that tries to register a padint, returns false if the padint already exists.
        public bool regPadint(int id, string server)
        {
            if (_serverPadInts.ContainsKey(id))
                return false;
            else 
            {
                _serverPadInts.Add(id, server);
                return true;
            }
        }

        //Gets a server from a padint ID.
        public string getServer(int id)
        {
            return _serverPadInts[id].ToString();
        }

        //Registers a server on master.
        public void regServer(string server)
        {
            _AvailableServer = server;
        }

        public void checkAvailability()
        {

        }

        //NOTE altering the timestamp of a transaction remotely alters it?!? or do we need to return it?!?
        public Transaction putTimeStamp(Transaction t)
        {
            t.setTimeStamp(genTimestamp());
            return t;
        }

        //Code to check if 2 timestamps are equal needs testing!?!?!
      /*  private static long lastTimeStamp = DateTime.UtcNow.Ticks;
        public static long UtcNowTicks
        {
            get
            {
                long original, newValue;
                do
                {
                    original = lastTimeStamp;
                    long now = DateTime.UtcNow.Ticks;
                    newValue = Math.Max(now, original + 1);
                } while (Interlocked.CompareExchange
                             (ref lastTimeStamp, newValue, original) != original);

                return newValue;
            }
        }*/

        //Temporary function to generate a timestamp for a transaction.
        private DateTime genTimestamp()
        {
            DateTime CurrentDate;
            CurrentDate = Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy"));
            return CurrentDate;
        }
    }
}
