using System.Collections.Generic;

namespace DataWindow.Utility
{
    public static class DictionaryExtensions
    {
        public static void AddOrModify<Tkey, TValue>(this IDictionary<Tkey, TValue> dic, Tkey key, TValue value)
        {
            if (dic.TryGetValue(key, out _))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }
    }
}