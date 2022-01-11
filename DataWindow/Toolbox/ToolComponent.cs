using System.IO;
using System.Reflection;

namespace DataWindow.Toolbox
{
    public class ToolComponent
    {
        protected ToolComponent()
        {
        }

        public ToolComponent(string fullName, ComponentAssembly assembly, bool enabled)
        {
            FullName = fullName;
            AssemblyName = assembly.Name;
            HintPath = assembly.HintPath;
            IsEnabled = enabled;
        }

        private string AssemblyFileNameWithoutPath
        {
            get
            {
                var length = AssemblyName.IndexOf(',');
                return AssemblyName.Substring(0, length) + ".dll";
            }
        }

        public string AssemblyName { get; set; }

        public string FileName
        {
            get
            {
                if (HintPath == null) return null;
                return Path.Combine(HintPath, AssemblyFileNameWithoutPath);
            }
        }

        public string HintPath { get; set; }

        public string FullName { get; set; }

        public string Name
        {
            get
            {
                var num = FullName.LastIndexOf('.');
                if (num > 0) return FullName.Substring(num + 1);
                return FullName;
            }
        }

        public string Namespace
        {
            get
            {
                var num = FullName.LastIndexOf('.');
                if (num > 0) return FullName.Substring(0, num);
                return string.Empty;
            }
        }

        public bool IsEnabled { get; set; } = true;

        public Assembly LoadAssembly()
        {
            Assembly result;
            if (HintPath != null)
                result = Assembly.LoadFrom(FileName);
            else
                result = Assembly.Load(AssemblyName);
            return result;
        }

        public ToolComponent Clone()
        {
            return new ToolComponent
            {
                FullName = FullName,
                AssemblyName = AssemblyName,
                IsEnabled = IsEnabled
            };
        }
    }
}