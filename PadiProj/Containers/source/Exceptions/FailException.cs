using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Containers
{
    [Serializable]
    public class FailException : TxException
    {
        public FailException()
        {

        }

        public FailException(string msg) : base(msg)
        {

        }
    }
}
