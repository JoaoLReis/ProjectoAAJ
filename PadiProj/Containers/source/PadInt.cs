using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Containers
{
    [Serializable]
    public class PadInt
    {
        private int _id;
        private int _value;

        public PadInt(int id)
        {
            _id = id;
            _value = 0;
        }

        public PadInt(int id, int value)
        {
            _id = id;
            _value = value;
        }

        public int getId()
        {
            return _id;
        }

        public int getVal()
        {
            return _value;
        }
    }
}
