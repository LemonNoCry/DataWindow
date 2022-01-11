using System.Drawing.Design;

namespace DataWindow.Toolbox
{
    public class ToolboxBaseItem
    {
        public ToolboxBaseItem(string text, string displayName, int image, bool isGroup)
        {
            Text = text;
            DisplayName = displayName;
            ImageIndex = image;
            IsGroup = isGroup;
            if (isGroup) Tag = ToolboxCategoryState.Expanded;
        }

        public ToolboxBaseItem(string text, string displayName, int image, ToolboxItem toolboxItem)
        {
            Text = text;
            DisplayName = displayName;
            ImageIndex = image;
            IsGroup = false;
            Tag = toolboxItem;
        }

        public string Text { get; set; }

        public string DisplayName { get; set; }

        public int ImageIndex { get; set; }

        public bool IsGroup { get; set; }

        public object Tag { get; set; }

        public string ShowDisplayText()
        {
            return string.IsNullOrWhiteSpace(DisplayName) ? Text : DisplayName;
        }
    }
}