using System;
using System.Collections;
using System.ComponentModel;

namespace DataWindow.Serialization.Components
{
    internal class ArrayProperty : ComponentProperty
    {
        public ArrayProperty(object component, PropertyDescriptor property) : base(component, property)
        {
        }

        public override void SetProperty(object value)
        {
            var arrayList = new ArrayList();
            Array c;
            if ((c = property.GetValue(component) as Array) != null) arrayList.AddRange(c);
            arrayList.Add(value);
            var array = (Array) Activator.CreateInstance(property.PropertyType, arrayList.Count);
            arrayList.CopyTo(array);
            try
            {
                property.SetValue(component, array);
            }
            catch
            {
            }
        }

        public override object GetProperty()
        {
            return null;
        }
    }
}