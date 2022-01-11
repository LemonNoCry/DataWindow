namespace DataWindow.Toolbox
{
    public class ComponentAssembly
    {
        public ComponentAssembly(string name) : this(name, null)
        {
        }

        public ComponentAssembly(string name, string hintPath)
        {
            Name = name;
            HintPath = hintPath;
        }

        public string Name { get; set; }

        public string HintPath { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}