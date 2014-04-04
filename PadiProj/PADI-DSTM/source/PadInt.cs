using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Containers;

[assembly: InternalsVisibleTo("PADI_DSTM")]

namespace PADI_DSTM
{
    [Serializable]
    public class PadInt
    {
        private PadIntValue _value;

        List<Request> _requests;

        //Only visible to the library.
        internal PadInt(PadIntValue v)
        {
            _requests = new List<Request>();
            _value = v;
        }

        public int getId()
        {
            return _value.getId();
        }

        //Returns the value to the client and pushes a new request to the list.
        public int Read()
        {
            _requests.Add(new Request(false, _value.getId(), -1));
            return _value.getValue();
        }

        //Writes the value to _value and pushes a new request to the list.
        public void Write(int v)
        {
            _requests.Add(new Request(true, _value.getId(), v));
            _value.setValue(v);
        }

        //Only visible to the library.
        internal List<Request> getRequests()
        {
            return _requests;
        }

        public PadIntValue getValue()
        {
            return _value;
        }

        public int getAbsValue()
        {
            return _value.getValue();
        }
    }
}
