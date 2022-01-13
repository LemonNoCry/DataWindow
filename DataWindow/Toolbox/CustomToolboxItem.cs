using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Web.UI.Design.WebControls;
using System.Windows.Forms;
using DataWindow.Core;
using DataWindow.DesignerInternal;
using DataWindow.Serialization;
using DataWindow.Utility;

namespace DataWindow.Toolbox
{
    [ToolboxItemAttribute(typeof(CustomToolboxItem))]
    public class CustomToolboxItem : ToolboxItem
    {
        public CustomToolboxItem()
        {
        }

        public CustomToolboxItem(Type type) : base(type)
        {
        }

        protected IComponent[] GetOrCreateComponentsCore(IDesignerHost host, IDictionary defaultValues)
        {
            ArrayList arrayList = new ArrayList();
            var cs = this.Properties["Source"] as ControlSerializable;
            Type type = this.GetType(host, this.AssemblyName, this.TypeName, true);
            if (type != (Type) null)
            {
                if (host != null)
                    arrayList.Add((object) host.CreateComponent(type, cs?.Name));
                else if (typeof(IComponent).IsAssignableFrom(type))
                    arrayList.Add(TypeDescriptor.CreateInstance((IServiceProvider) null, type, (Type[]) null, (object[]) null));
            }

            IComponent[] componentsCore = new IComponent[arrayList.Count];
            arrayList.CopyTo((Array) componentsCore, 0);
            return componentsCore;
        }

        protected override IComponent[] CreateComponentsCore(IDesignerHost host, IDictionary defaultValues)
        {
            var dh = (DesignerHost) host;
            var cs = (ControlSerializable) Properties["Source"];
            var isHas = dh.HasComponent(cs?.Name);
            IComponent[] componentsCore = this.GetOrCreateComponentsCore(host, defaultValues);


            for (int index1 = 0; index1 < componentsCore.Length; ++index1)
            {
                var component = componentsCore[index1];
                if (host.GetDesigner(component) is IComponentInitializer designer)
                {
                    bool flag = true;
                    try
                    {
                        Control con = (Control) component;

                        var clickPoint = (Point) defaultValues["Location"];

                        if (!isHas)
                        {
                            designer.InitializeNewComponent(defaultValues);
                            clickPoint = con.Parent.PointToClient(clickPoint);

                            cs?.ControlSerializableToControl(con);
                            con.Visible = true;
                        }
                        else
                        {
                            var parent = defaultValues["Parent"] as Control;

                            if (con.Parent == null || con.Parent != parent)
                            {
                                con.Parent = parent;
                            }

                            clickPoint = con.Parent.PointToClient(clickPoint);
                        }

                        con.Location = clickPoint;

                        flag = false;
                    }
                    finally
                    {
                        if (flag)
                        {
                            for (int index2 = 0; index2 < componentsCore.Length; ++index2)
                                host.DestroyComponent(componentsCore[index2]);
                        }
                    }
                }
            }

            return componentsCore;
        }
    }
}