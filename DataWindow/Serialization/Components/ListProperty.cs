using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace DataWindow.Serialization.Components
{
    internal class ListProperty : ComponentProperty
    {
        public ListProperty(object component, PropertyDescriptor property) : base(component, property)
        {
        }

        private bool TryAddRange(object list, object item)
        {
            foreach (var methodInfo in list.GetType().GetMethods())
                if (!(methodInfo.Name != "AddRange"))
                {
                    var parameters = methodInfo.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType.IsArray)
                    {
                        var array = (Array) Activator.CreateInstance(parameters[0].ParameterType, 1);
                        array.SetValue(item, 0);
                        object[] parameters2 =
                        {
                            array
                        };
                        methodInfo.Invoke(list, parameters2);
                        return true;
                    }
                }

            return false;
        }

        public override void SetProperty(object value)
        {
            var value2 = this.property.GetValue(component);
            if (value2 == null || TryAddRange(value2, value)) return;
            IList list;
            if ((list = value2 as IList) != null)
            {
                if (list.IsFixedSize) return;
                var property = value2.GetType().GetProperty("List", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty);
                if (property != null) list = (IList) property.GetValue(value2, new object[0]);
                try
                {
                    list.Add(value);
                }
                catch
                {
                }
            }
        }

        public override object GetProperty()
        {
            return null;
        }
    }
}