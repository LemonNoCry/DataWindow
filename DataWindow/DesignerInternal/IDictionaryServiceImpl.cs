using System.Collections;
using System.ComponentModel.Design;

namespace DataWindow.DesignerInternal
{
    internal class IDictionaryServiceImpl : IDictionaryService
    {
        private readonly IDictionary dictionary;

        public IDictionaryServiceImpl()
        {
            dictionary = new Hashtable();
        }

        public object GetValue(object key)
        {
            return dictionary[key];
        }

        public void SetValue(object key, object value)
        {
            dictionary[key] = value;
        }

        public object GetKey(object value)
        {
            foreach (var obj in dictionary)
            {
                var dictionaryEntry = (DictionaryEntry) obj;
                if (dictionaryEntry.Value == value) return dictionaryEntry.Key;
            }

            return null;
        }
    }
}