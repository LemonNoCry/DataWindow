using System.Windows.Forms;
using DataWindow.CustomPropertys;
namespace DataWindow.Serialization
{
    public class CheckBoxSerializable: ControlSerializable, IPropertyCollections<CheckBox>
    {
        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as CheckBox);
        }

        public CustomPropertyCollection GetCollections(CheckBox control)
        {
            var cpc = base.GetCollections(control);

            return cpc;
        }
    }
}