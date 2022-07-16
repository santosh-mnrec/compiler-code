using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLanguage
{

    [Serializable]
    public class EvalException : Exception
    {
        public EvalException() { }
        public EvalException(string message) : base(message) { }
        public EvalException(string message, Exception inner) : base(message, inner) { }
        protected EvalException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
