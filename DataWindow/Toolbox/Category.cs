using System.Collections.Generic;

namespace DataWindow.Toolbox
{
    public class Category
    {
        protected Category()
        {
        }

        public Category(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public bool IsEnabled { get; set; } = true;

        public List<ToolComponent> ToolComponents { get; } = new List<ToolComponent>();

        public object Clone()
        {
            var category = new Category();
            category.Name = Name;
            category.IsEnabled = IsEnabled;
            foreach (var toolComponent in ToolComponents) category.ToolComponents.Add(toolComponent.Clone());
            return category;
        }
    }
}