using System;
using System.Windows.Forms;
using DataWindow.CustomPropertys;

namespace DataWindow.Serialization
{
    [Serializable]
    public class ScrollableControlSerializable : ControlSerializable, IPropertyCollections<ScrollableControl>
    {
        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as ScrollableControl);
        }

        public CustomPropertyCollection GetCollections(ScrollableControl control)
        {
            var cpc = base.GetCollections(control);

            cpc.Add(new CustomProperty("启用滚动条", "AutoScroll", "外观", "AutoScroll 启用滚动条", control));

            return cpc;
        }
    }
}