using System;
using System.Drawing.Design;

namespace DataWindow.Toolbox
{
    public class ToolboxItemUsedArgs : EventArgs
    {
        public ToolboxItemUsedArgs(ToolboxItem usedItem)
        {
            UsedItem = usedItem;
        }

        public ToolboxItem UsedItem { get; set; }
    }
}