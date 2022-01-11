using System.Reflection;

namespace DataWindow.DesignerInternal
{
    internal class ExtInvokeParam
    {
        public MethodInfo MethodInfo { get; set; }

        public object Provider { get; set; }

        public object[] Params { get; set; }
    }
}