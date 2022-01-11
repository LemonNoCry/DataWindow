using System.Collections;

namespace DataWindow.Utility
{
    internal static class IEnumerableExtensions
    {
        public static bool Contains(this IEnumerable list, object value)
        {
            if (list == null) return false;

            var enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
                if (enumerator.Current == value)
                    return true;
            return false;
        }
    }
}