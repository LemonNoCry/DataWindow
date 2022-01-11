using System.Collections.ObjectModel;

namespace DataWindow.Serialization.Components
{
    internal class ReferencedCollection : Collection<ReferencedItem>
    {
        public void Add(string key, ComponentProperty property)
        {
            foreach (var referencedItem in this)
                if (referencedItem.Key == key)
                {
                    referencedItem.Properties.Add(property);
                    return;
                }

            base.Add(new ReferencedItem
            {
                Key = key,
                Properties =
                {
                    property
                }
            });
        }
    }
}