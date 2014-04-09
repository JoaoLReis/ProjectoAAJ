using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Containers
{
    [Serializable]
    public class TxException : Exception
    {
        public TxException()
        {

        }

        public TxException (string msg) : base(msg)
        {

        }
    }
}
