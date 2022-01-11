using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace DataWindow.DesignerInternal.Event
{
    internal abstract class DesignerAction
    {
        protected IDesignerHost host;

        protected string name;

        protected MegaAction owner;

        public DesignerAction(IDesignerHost host, object control, MegaAction owner)
        {
            this.host = host;
            name = ComponentName(control as Component);
            this.owner = owner;
        }

        public abstract void Undo();

        public abstract void Redo();

        public virtual void Dispose()
        {
            host = null;
        }

        protected string ComponentName(Component control)
        {
            string text;
            if (control == null)
            {
                text = null;
            }
            else
            {
                var site = control.Site;
                text = site != null ? site.Name : null;
            }

            return text ?? "";
        }

        protected void SetProperties(Hashtable props)
        {
            var component = ((IContainer) host.GetService(typeof(IContainer))).Components[name];
            if (component != null) owner.LoadProperties(component, props, (DesignerHost) host);
        }
    }
}