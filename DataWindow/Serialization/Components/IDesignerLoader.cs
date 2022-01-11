using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DataWindow.DesignerInternal;

namespace DataWindow.Serialization.Components
{
    public interface IDesignerLoader
    {
        LoadModes LoadMode { get; set; }

        StoreModes StoreMode { get; set; }

        string LogFile { get; set; }

        bool ShowErrorMessage { get; set; }

        IDesignerHost DesignerHost { get; set; }

        FormComponents Components { get; set; }
        event DrillDownHandler DrillDown;

        event ComponentEventHandler ComponentLoaded;

        void Load(Control parent, IReader reader, Dictionary<string, IComponent> components, bool ignoreParent);

        void Store(IComponent[] parents, IWriter writer);

        void SetEventSource(object eventSource);

        void BindEvents(object eventSource);

        void UnbindEvents(object eventSource);

        void RefreshEventData();
    }
}