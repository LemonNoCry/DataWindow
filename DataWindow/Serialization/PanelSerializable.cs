using System.Windows.Forms;
using DataWindow.CustomPropertys;
namespace DataWindow.Serialization
{
    public class PanelSerializable: ControlSerializable, IPropertyCollections<Panel>
    {
        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as Panel);
        }

        public CustomPropertyCollection GetCollections(Panel control)
        {
            var cpc = base.GetCollections(control);

            return cpc;
        }
    }
}