using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Containers
{
    [Serializable]
    public class Message
    {
        private string _owner;
        private string _message;

        public Message(string owner, string message)
        {
            _owner = owner;
            _message = message;
        }

        public string getOwner()
        {
            return _owner;
        }

        public string getMessage()
        {
            return _message;
        }
    }
}
