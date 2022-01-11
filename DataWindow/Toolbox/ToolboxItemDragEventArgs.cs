using System;

namespace DataWindow.Toolbox
{
    public class ToolboxItemDragEventArgs : EventArgs
    {
        public ToolboxItemDragEventArgs(ToolboxBaseItem item)
        {
            Item = item;
        }

        public ToolboxBaseItem Item { get; set; }
    }
}