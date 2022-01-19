using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreClassLibrary
{
    public class SellPriceTooLowException : Exception
    {
        public SellPriceTooLowException() { }

        public SellPriceTooLowException(string message) : base(message) { }

        public SellPriceTooLowException(string message, Exception inner) : base(message, inner) { }

        protected SellPriceTooLowException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}