using System;
using System.Windows.Forms;
using DataWindow.CustomPropertys;

namespace DataWindow.Serialization
{
    [Serializable]
    public class TextBoxSerializable : ControlSerializable, IPropertyCollections<TextBox>
    {
        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as TextBox);
        }

        public CustomPropertyCollection GetCollections(TextBox control)
        {
            var cpc = base.GetCollections(control);

            return cpc;
        }
    }
}