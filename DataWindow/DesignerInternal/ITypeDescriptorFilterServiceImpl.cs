using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using DataWindow.DesignerInternal.Event;

namespace DataWindow.DesignerInternal
{
    internal class ITypeDescriptorFilterServiceImpl : ITypeDescriptorFilterService
    {
        private readonly IDesignerHost _host;

        public ITypeDescriptorFilterServiceImpl(IDesignerHost host)
        {
            _host = host;
        }

        public bool FilterAttributes(IComponent component, IDictionary attributes)
        {
            var result = false;
            IDesignerFilter designerFilter;
            if ((designerFilter = _host.GetDesigner(component) as IDesignerFilter) != null)
            {
                designerFilter.PreFilterAttributes(attributes);
                designerFilter.PostFilterAttributes(attributes);
                result = true;
            }

            if (FilterAtt != null && !(component is DesignSurface))
            {
                var filterEventArgs = new FilterEventArgs
                {
                    Data = attributes,
                    Caching = true
                };
                FilterAtt(component, filterEventArgs);
                return filterEventArgs.Caching;
            }

            return result;
        }

        public bool FilterEvents(IComponent component, IDictionary events)
        {
            var result = false;
            IDesignerFilter designerFilter;
            if ((designerFilter = _host.GetDesigner(component) as IDesignerFilter) != null)
            {
                designerFilter.PreFilterEvents(events);
                designerFilter.PostFilterEvents(events);
                result = true;
            }

            if (FilterEvnts != null && !(component is DesignSurface))
            {
                var filterEventArgs = new FilterEventArgs
                {
                    Data = events,
                    Caching = true
                };
                FilterEvnts(component, filterEventArgs);
                return filterEventArgs.Caching;
            }

            return result;
        }

        public bool FilterProperties(IComponent component, IDictionary properties)
        {
            var result = false;
            IDesignerFilter designerFilter;
            if ((designerFilter = _host.GetDesigner(component) as IDesignerFilter) != null)
            {
                designerFilter.PreFilterProperties(properties);
                designerFilter.PostFilterProperties(properties);
                result = true;
            }

            if (FilterProps != null && !(component is DesignSurface))
            {
                var filterEventArgs = new FilterEventArgs
                {
                    Data = properties,
                    Caching = true
                };
                FilterProps(component, filterEventArgs);
                return filterEventArgs.Caching;
            }

            return result;
        }

        internal event EventHandler<FilterEventArgs> FilterAtt;

        internal event EventHandler<FilterEventArgs> FilterEvnts;

        internal event EventHandler<FilterEventArgs> FilterProps;
    }
}