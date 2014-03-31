using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Containers.source
{
    [Serializable]
    class Request
    {
        private bool _write;
        private int _padint;
        private int _val;

        public Request(bool write, int padint, int val)
        {
            _write = write;
            _padint= padint;
            _val = val;
        }

        //Returns if this request is a write or a read.
        public bool isWrite()
        {
            return _write;
        }

        //Returns the padint wich the request is being made on.
        public int involved()
        {
            return _padint;
        }

        //Returns the value if a write.
        public int getVal()
        {
            if (_write)
                return _val;
            else return -1;
        }
    }
}
