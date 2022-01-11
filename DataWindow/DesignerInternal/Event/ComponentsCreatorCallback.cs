using System.ComponentModel;
using System.ComponentModel.Design;

namespace DataWindow.DesignerInternal.Event
{
    public delegate IComponent[] ComponentsCreatorCallback(string format, object serializedObject, IDesignerHost host);
}