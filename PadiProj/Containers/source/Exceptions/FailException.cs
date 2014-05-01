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
    public class FailException : Exception, ISerializable
    {
        public FailException()
            : base() { }
    
        public FailException(string message)
            : base(message) { }
    
        public FailException(string format, params object[] args)
            : base(string.Format(format, args)) { }
    
        public FailException(string message, Exception innerException)
            : base(message, innerException) { }
    
        public FailException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        public FailException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info,
            StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
