using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreClassLibrary
{
    public class NegativeStockException : Exception
    {
        public NegativeStockException() { }

        public NegativeStockException(string message) : base(message) { }

        public NegativeStockException(string message, Exception inner) : base(message, inner) { }

        protected NegativeStockException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}