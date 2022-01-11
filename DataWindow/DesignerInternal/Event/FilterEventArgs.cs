using System;
using System.Collections;

namespace DataWindow.DesignerInternal.Event
{
    public class FilterEventArgs : EventArgs
    {
        public IDictionary Data { get; set; }

        public bool Caching { get; set; }
    }
}