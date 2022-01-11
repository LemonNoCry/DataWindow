using System;
using System.Collections.Generic;

namespace DataWindow.Serialization.Components
{
    public abstract class ReaderBase<T> : IReader, IDisposable where T : IDisposable
    {
        protected T reader;
        public string Name { get; protected set; } = string.Empty;

        public string Value { get; protected set; } = string.Empty;

        public Dictionary<string, string> Attributes { get; protected set; } = new Dictionary<string, string>();

        public ReaderState State { get; protected set; }

        public abstract bool Read();

        public virtual void Dispose()
        {
            ref var ptr = ref reader;
            var t = default(T);
            if (t == null)
            {
                t = reader;
                ptr = t;
                if (t == null) return;
            }

            ptr.Dispose();
        }
    }
}