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
    public class FreezeException : Exception
    {
        public FreezeException()
            : base() { }
    
        public FreezeException(string message)
            : base(message) { }
    
        public FreezeException(string format, params object[] args)
            : base(string.Format(format, args)) { }
    
        public FreezeException(string message, Exception innerException)
            : base(message, innerException) { }
    
        public FreezeException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        public FreezeException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info,
            StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
