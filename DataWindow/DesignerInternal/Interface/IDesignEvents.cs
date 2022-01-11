using System;
using DataWindow.DesignerInternal.Event;

namespace DataWindow.DesignerInternal.Interface
{
    public interface IDesignEvents
    {
        event EventHandler<FilterEventArgs> FilterAttributes;

        event EventHandler<FilterEventArgs> FilterEvents;

        event EventHandler<FilterEventArgs> FilterProperties;

        event AllowDesignHandler AllowDesign;

        event AddingVerbHandler AddingVerb;
    }
}