using System.Collections.Generic;

namespace DataWindow.Serialization.Components
{
    internal class ReferencedItem
    {
        internal string Key { get; set; }

        internal IList<ComponentProperty> Properties { get; } = new List<ComponentProperty>();
    }
}