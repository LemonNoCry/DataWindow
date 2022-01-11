using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;

namespace DataWindow.DesignerInternal
{
    internal class NestedContainer : INestedContainer, IContainer, IDisposable
    {
        private readonly Dictionary<IComponent, string> components;

        private readonly IContainer container;

        public NestedContainer(IComponent owner, IDesignerHost host)
        {
            Owner = owner;
            container = (IContainer) host.GetService(typeof(IContainer));
            components = new Dictionary<IComponent, string>();
        }

        public IComponent Owner { get; }

        public ComponentCollection Components => new ComponentCollection(components.Keys.ToArray());

        public void Add(IComponent component, string name)
        {
            components.Add(component, name);
            container.Add(component, name);
        }

        public void Add(IComponent component)
        {
            Add(component, null);
        }

        public void Remove(IComponent component)
        {
            components.Remove(component);
            container.Remove(component);
        }

        public void Dispose()
        {
            var container = this.container;
            if (container == null) return;
            container.Dispose();
        }
    }
}