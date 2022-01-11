using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Security.Permissions;
using DataWindow.DesignerInternal.Event;

namespace DataWindow.DesignLayer
{
    internal class ToolboxItemHelper : ToolboxItem
    {
        private readonly ComponentsCreatorCallback _callback;

        private readonly string format;

        private readonly object serializedObject;

        public ToolboxItemHelper(string format, object serializedObject, ComponentsCreatorCallback callback)
        {
            _callback = callback;
            this.format = format;
            this.serializedObject = serializedObject;
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override IComponent[] CreateComponentsCore(IDesignerHost host)
        {
            return _callback(format, serializedObject, host);
        }
    }
}