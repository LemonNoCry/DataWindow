using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using DataWindow.DesignerInternal.Interface;

namespace DataWindow.DesignerInternal
{
    internal class DesignerSite : ISite, IServiceProvider, IServiceContainer
    {
        private readonly DesignerHost _designerHost;

        private readonly ServiceContainer _serviceContainer;

        private bool _designMode;

        private IDictionaryServiceImpl _dictionaryService;

        private NestedContainer _nestedContainer;

        public DesignerSite(DesignerHost designer, IComponent component)
        {
            _designMode = true;
            _designerHost = designer;
            _serviceContainer = new ServiceContainer(designer);
            Component = component;
        }

        public IDesigner Designer { get; set; }

        public void AddService(Type serviceType, ServiceCreatorCallback callback)
        {
            AddService(serviceType, callback, false);
        }

        public void AddService(Type serviceType, object serviceInstance)
        {
            AddService(serviceType, serviceInstance, false);
        }

        public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
        {
            _serviceContainer.AddService(serviceType, callback, promote);
        }

        public void AddService(Type serviceType, object serviceInstance, bool promote)
        {
            _serviceContainer.AddService(serviceType, serviceInstance, promote);
        }

        public void RemoveService(Type serviceType)
        {
            RemoveService(serviceType, false);
        }

        public void RemoveService(Type serviceType, bool promote)
        {
            _serviceContainer.RemoveService(serviceType, promote);
        }

        public bool DesignMode
        {
            get
            {
                var designMode = _designMode;
                _designMode = true;
                return designMode;
            }
            set => _designMode = value;
        }

        public string Name { get; set; }

        public IComponent Component { get; }

        public IContainer Container => _designerHost.Container;

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IDictionaryService))
            {
                if (_dictionaryService == null) _dictionaryService = new IDictionaryServiceImpl();
                return _dictionaryService;
            }

            if (serviceType == typeof(INestedContainer))
            {
                if (_nestedContainer == null) _nestedContainer = new NestedContainer(Component, _designerHost);
                return _nestedContainer;
            }

            var service = _serviceContainer.GetService(serviceType);
            if (service != null) return service;
            return _designerHost.GetService(serviceType);
        }
    }
}