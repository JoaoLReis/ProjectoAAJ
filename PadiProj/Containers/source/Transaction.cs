using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Containers.source
{
    [Serializable]
    class Transaction
    {
        private DateTime _timestamp;
        List<Request> _requests;

        public Transaction(DateTime timestamp, List<Request> requests)
        {
            _timestamp = timestamp;
            _requests = requests;
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
