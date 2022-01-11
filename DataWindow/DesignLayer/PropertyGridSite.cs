using System;
using System.ComponentModel;

namespace DataWindow.DesignLayer
{
    public class PropertyGridSite : ISite, IServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public PropertyGridSite(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IComponent Component => null;

        public IContainer Container => null;

        public bool DesignMode => false;

        public string Name
        {
            get => null;
            set { }
        }

        public object GetService(Type serviceType)
        {
            var serviceProvider = _serviceProvider;
            if (serviceProvider == null) return null;
            return serviceProvider.GetService(serviceType);
        }
    }
}