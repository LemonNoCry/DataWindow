using System;
using System.Drawing.Design;

namespace DataWindow.Toolbox
{
    public interface IToolbox
    {
        ToolboxCategoryCollection Items { get; }

        ToolboxItem SelectedItem { get; set; }

        string SelectedCategory { get; set; }
        event EventHandler BeginDragAndDrop;

        event EventHandler DropControl;

        event EventHandler<ToolboxItemUsedArgs> ToolboxItemUsed;

        void AddItem(ToolboxItem item, string category);

        void RemoveItem(ToolboxItem item, string category);

        void Refresh();
    }
}