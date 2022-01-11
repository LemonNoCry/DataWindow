using System.Drawing.Design;

namespace DataWindow.Toolbox
{
    public class ToolboxCategoryItem
    {
        public ToolboxCategoryItem(string name)
        {
            Name = name;
        }

        public ToolboxCategoryItem(string name, ToolboxItemCollection items)
        {
            Name = name;
            Items = items;
        }

        public string Name { get; set; }

        public ToolboxItemCollection Items { get; set; }
    }
}