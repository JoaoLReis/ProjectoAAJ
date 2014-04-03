using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Containers
{
    [Serializable]
    public class PadIntValue
    {
        private int _id;
        private int _value;

        public PadIntValue(int id, int value)
        {
            _id = id;
            _value = value;
        }

        public int getId()
        {
            return _id;
        }

        public int getValue()
        {
            return _value;
        }
    }
}
