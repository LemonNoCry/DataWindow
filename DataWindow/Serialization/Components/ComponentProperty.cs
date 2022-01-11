using System.ComponentModel;

namespace DataWindow.Serialization.Components
{
    internal class ComponentProperty
    {
        protected object component;

        protected PropertyDescriptor property;

        public ComponentProperty(object component, PropertyDescriptor property)
        {
            this.component = component;
            this.property = property;
        }

        public virtual void SetProperty(object value)
        {
            if (property.GetValue(component) == value) return;
            property.SetValue(component, value);
        }

        public virtual object GetProperty()
        {
            return property.GetValue(component);
        }
    }
}