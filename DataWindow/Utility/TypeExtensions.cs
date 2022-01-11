using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace DataWindow.Utility
{
    internal static class TypeExtensions
    {
        public static bool IsDataCollection(this Type type)
        {
            bool result;
            if (type.FullName.IndexOf("Infragistics") >= 0)
                result = typeof(ICollection).IsAssignableFrom(type);
            else
                result = typeof(InternalDataCollectionBase).IsAssignableFrom(type) || typeof(BaseCollection).IsAssignableFrom(type);
            return result;
        }
    }
}