using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace DataWindow.Toolbox
{
    [ToolboxItemAttribute(typeof(CustomToolboxItem))]
    public class CustomToolboxItem : ToolboxItem
    {
        public CustomToolboxItem()
        {
            
        }
        public CustomToolboxItem(Type type):base(type)
        {
            
        }

        protected override IComponent[] CreateComponentsCore(IDesignerHost host)
        {
            return base.CreateComponentsCore(host);
        }

        protected override IComponent[] CreateComponentsCore(IDesignerHost host, IDictionary defaultValues)
        {
            return base.CreateComponentsCore(host, defaultValues);
        }
    }
}