using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateSystem
{
    public class Follow : IDisposable
    {
        public virtual void Initaialize() { }

        protected virtual void ExceptionHandle(Exception e) { }
        public virtual void Dispose() { }
    }
}
