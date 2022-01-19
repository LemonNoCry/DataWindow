using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace DataWindow.Utility
{
    internal static class ObjectExtensions
    {
        public static void CopyPropertiesTo(this object src, object dest)
        {
            var properties = TypeDescriptor.GetProperties(src);
            var properties2 = TypeDescriptor.GetProperties(dest);
            foreach (var obj in properties)
            {
                var propertyDescriptor = (PropertyDescriptor) obj;
                if (propertyDescriptor.IgnorePropertyDescriptor())
                {
                    continue;
                }

                if (!propertyDescriptor.IsReadOnly && propertyDescriptor.IsBrowsable)
                {
                    var propertyDescriptor2 = properties2[propertyDescriptor.Name];
                    if (propertyDescriptor2 != null)
                        try
                        {
                            var value = propertyDescriptor.GetValue(src);
                            var value2 = propertyDescriptor2.GetValue(dest);
                            if (value2 != null && !value2.Equals(value) || value != null) propertyDescriptor2.SetValue(dest, value);
                        }
                        catch
                        {
                        }
                }
            }
        }

        public static bool IgnorePropertyDescriptor(this PropertyDescriptor propertyDescriptor)
        {
            return propertyDescriptor.Attributes.Cast<Attribute>().Any(s => s is XmlIgnoreAttribute);
        }

        public static void GetNoIgnoreProperties(this object src)
        {
            var td = TypeDescriptor.GetProperties(src);
            for (var index = td.Count - 1; index >= 0; index--)
            {
                PropertyDescriptor pd = td[index];

                if (pd.IgnorePropertyDescriptor())
                {
                    td.RemoveAt(index);
                }
            }
        }

        public static void AddRange<T>(this HashSet<T> source, HashSet<T> target)
        {
            foreach (var t in target)
            {
                source.Add(t);
            }
        }
    }
}