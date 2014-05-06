using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Containers
{
    [Serializable]
    public class Transaction : IEquatable<Transaction>
    {
        private DateTime _timestamp;
        private List<Request> _requests;
        private int _ticket;

        public Transaction(DateTime timestamp, List<Request> requests)
        {
            _timestamp = timestamp;
            _requests = requests;
        }

        //Function used to set a timestamp of a transaction.
        public void setRequests(List<Request> l)
        {
            _requests = l;
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

        public int getTicket()
        {
            return _ticket;
        }

        public void setTicket(int ticket)
        {
            _ticket = ticket;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Transaction objAsPart = obj as Transaction;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }

        public override int GetHashCode()
        {
            return _ticket;
        }

        public bool Equals(Transaction other)
        {
            if (other == null) return false;
            return ((_ticket.Equals(other.getTicket())) && (_timestamp.Equals(other.getTimeStamp())));
        }

    }
}
