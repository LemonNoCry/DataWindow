using System.Reflection;

namespace DataWindow.Serialization.Components
{
    internal class InstanceDescriptorLoader
    {
        public InstanceDescriptorLoader(MemberInfo memberInfo, object[] args)
        {
            MemberInfo = memberInfo;
            Arguments = args;
        }

        public MemberInfo MemberInfo { get; set; }

        public object[] Arguments { get; set; }
    }
}