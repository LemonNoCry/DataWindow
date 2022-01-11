using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DataWindow.DesignerInternal.Event;

namespace DataWindow.DesignerInternal
{
    internal class AddAction : DesignerAction
    {
        private readonly Type _type;
        private Control _parent;

        protected string parentName = "";

        protected Hashtable properties;

        public AddAction(IDesignerHost host, object component, MegaAction owner) : base(host, component, owner)
        {
            _type = component.GetType();
            Control control;
            if ((control = component as Control) != null)
            {
                _parent = control.Parent;
                parentName = ComponentName(_parent);
            }
        }

        public override void Undo()
        {
            var container = (IContainer) host.GetService(typeof(IContainer));
            var component = container.Components[name];
            if (component != null)
            {
                Control control;
                if (_parent == null && (control = component as Control) != null)
                {
                    _parent = control.Parent;
                    parentName = ComponentName(_parent);
                }

                properties = owner.StoreProperties(component, null, null);
                var selectionService = (ISelectionService) host.GetService(typeof(ISelectionService));
                container.Remove(component);
                component.Dispose();
                selectionService.SetSelectedComponents(null);
            }
        }

        public override void Redo()
        {
            ((ISelectionService) host.GetService(typeof(ISelectionService))).SetSelectedComponents(null);
            Control control;
            if ((control = host.CreateComponent(_type, name) as Control) != null)
            {
                var container = (IContainer) host.GetService(typeof(IContainer));
                var control2 = parentName != "" ? container.Components[parentName] as Control : null;
                if (control2 != null && _parent != control2) _parent = control2;
                control.Parent = _parent;
                control.BringToFront();
            }

            SetProperties(properties);
        }
    }
}