using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Security.Permissions;

namespace Containers
{
    [Serializable()]
    public class TxException : Exception
    {
        public TxException()
            : base() { }
    
        public TxException(string message)
            : base(message) { }
    
        public TxException(string format, params object[] args)
            : base(string.Format(format, args)) { }
    
        public TxException(string message, Exception innerException)
            : base(message, innerException) { }
    
        public TxException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        public TxException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info,
            StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
     }
}
