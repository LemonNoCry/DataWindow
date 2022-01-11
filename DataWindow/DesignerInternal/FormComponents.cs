using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DataWindow.DesignerInternal.Interface;

namespace DataWindow.DesignerInternal
{
    public class FormComponents : IFormComponents
    {
        private readonly Dictionary<string, IComponent> _components = new Dictionary<string, IComponent>();

        public IDictionary<string, IComponent> Components => _components;

        public IComponent Get(string name)
        {
            IComponent result;
            _components.TryGetValue(name, out result);
            return result;
        }

        public void Add(string name, IComponent component)
        {
            if (_components.FirstOrDefault(c => c.Value == component).Value != null) return;
            _components[name] = component;
        }

        public void Remove(string name)
        {
            if (!_components.ContainsKey(name)) return;
            _components.Remove(name);
        }

        public void Clear()
        {
            _components.Clear();
        }
    }
}