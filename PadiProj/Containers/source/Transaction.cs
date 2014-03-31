using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Containers
{
    [Serializable]
    public class Transaction
    {
        private DateTime _timestamp;
        List<Request> _requests;

        public Transaction(List<Request> requests)
        {
            _timestamp = new DateTime(0);
            _requests = requests;
        }

        public Transaction(DateTime timestamp, List<Request> requests)
        {
            _timestamp = timestamp;
            _requests = requests;
        }

        //Function used to set a timestamp of a transaction.
        public void setTimeStamp(DateTime dt)
        {
            _timestamp = dt;
        }

        //Get the timestamp of when the transaction was generated.
        public DateTime getTimeStamp()
        {
            return _timestamp;
        }

        //Return all requests within this transaction.
        public List<Request> getRequests()
        {
            return _requests;
        }

    }
}
