using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DataWindow.DesignerInternal.Event;
using DataWindow.DesignLayer;
using DataWindow.Utility;

namespace DataWindow.DesignerInternal
{
    internal class RootDesigner : DocumentDesigner
    {
        private bool _inited;

        protected override void Dispose(bool disposing)
        {
            var eventFilter = (EventFilter) GetService(typeof(EventFilter));
            if (eventFilter != null) eventFilter.KeyDown -= KeyListner;
            base.Dispose(disposing);
        }

        private void SelectNext(bool next)
        {
            ISelectionService selectionService;
            Control control;
            if ((selectionService = GetService(typeof(ISelectionService)) as ISelectionService) != null && (control = ((IDesignerHost) GetService(typeof(IDesignerHost))).RootComponent as Control) != null)
            {
                var control2 = selectionService.PrimarySelection as Control;
                if (control2 == null)
                {
                    if (next)
                    {
                        control2 = control.Controls.First();
                    }
                    else
                    {
                        control2 = control;
                        while (control2.Controls.Count != 0)
                        {
                            control2 = control2.Controls.Last();
                            if (control2 is ContainerControl) break;
                        }
                    }
                }
                else
                {
                    var flag = false;
                    var parent = control2.Parent;
                    if (parent == null || control2 == control && control2.Controls.Count == 0) return;
                    if (!next)
                    {
                        if (control2 == control)
                        {
                            control2 = control2.Controls.Last();
                        }
                        else if (control2.TabIndex == 0 && parent != control)
                        {
                            control2 = parent;
                            parent = parent.Parent;
                        }
                        else
                        {
                            control2 = parent.Controls.Previous(control2);
                            while (control2 != null && control2.Controls.Count != 0 && !(control2 is ContainerControl))
                            {
                                if (control2 is DataGridView) break;
                                control2 = control2.Controls.Last();
                            }
                        }
                    }
                    else if (control2.Controls.Count != 0 && (!(control2 is ContainerControl) && !(control2 is DataGridView) || control2 == control))
                    {
                        control2 = control2.Controls.First();
                    }
                    else
                    {
                        while (control2 == parent.Controls.Last() && parent != control)
                        {
                            control2 = parent;
                            parent = parent.Parent;
                            flag = true;
                        }

                        if (control2.Controls.Count == 0 || control2 is ContainerControl || control2 is DataGridView || flag) control2 = parent.Controls.Next(control2);
                    }
                }

                if (control2 == null) control2 = control;
                Control[] components =
                {
                    control2
                };
                selectionService.SetSelectedComponents(components, SelectionTypes.Replace);
            }
        }

        private void KeyListner(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 9)
            {
                SelectNext((e.Modifiers & Keys.Shift) == Keys.None);
                return;
            }

            var flag = false;
            Designer designer;
            if ((designer = GetService(typeof(Designer)) as Designer) != null && !designer.SnapToGrid) flag = true;
            if ((e.Modifiers & Keys.Control) != Keys.None) flag = !flag;
            Point delta;
            switch (e.KeyValue)
            {
                case 37:
                    delta = new Point(flag ? -1 : -GridSize.Width, 0);
                    break;
                case 38:
                    delta = new Point(0, flag ? -1 : -GridSize.Height);
                    break;
                case 39:
                    delta = new Point(flag ? 1 : GridSize.Width, 0);
                    break;
                case 40:
                    delta = new Point(0, flag ? 1 : GridSize.Height);
                    break;
                default:
                    return;
            }

            if ((e.Modifiers & Keys.Shift) != Keys.None)
            {
                ResizeSelection(delta);
                return;
            }

            MoveSelection(delta);
        }

        private void MoveOrResize(Point delta, ActionDelegate action)
        {
            ISelectionService selectionService;
            if ((selectionService = GetService(typeof(ISelectionService)) as ISelectionService) != null)
            {
                var selectedComponents = selectionService.GetSelectedComponents();
                if (selectedComponents.Count > 0)
                {
                    var designerHost = (IDesignerHost) GetService(typeof(IDesignerHost));
                    var control = (Control) designerHost.RootComponent;
                    if (control != null) control.SuspendLayout();
                    using (var designerTransaction = designerHost.CreateTransaction("Move or Resize controls"))
                    {
                        var control2 = selectionService.PrimarySelection as Control;
                        if (control2 != null)
                        {
                            var componentChangeService = (IComponentChangeService) GetService(typeof(IComponentChangeService));
                            foreach (var obj in selectedComponents)
                            {
                                var control3 = obj as Control;
                                if (control3 != null && control3.Parent == control2.Parent)
                                {
                                    componentChangeService.OnComponentChanging(control3, null);
                                    action(control3, delta);
                                    componentChangeService.OnComponentChanged(control3, null, null, null);
                                }
                            }
                        }

                        designerTransaction.Commit();
                    }

                    if (control != null) control.ResumeLayout();
                }
            }
        }

        private void ResizeControl(Control control, Point delta)
        {
            var size = control.Size;
            size.Width += delta.X;
            size.Height += delta.Y;
            control.SetProperty("Size", size);
        }

        private void MoveControl(Control control, Point delta)
        {
            var location = control.Location;
            location.X += delta.X;
            location.Y += delta.Y;
            control.SetProperty("Location", location);
        }

        private void ResizeSelection(Point delta)
        {
            MoveOrResize(delta, ResizeControl);
        }

        private void MoveSelection(Point delta)
        {
            MoveOrResize(delta, MoveControl);
        }

        protected override void OnCreateHandle()
        {
            base.OnCreateHandle();
            if (!_inited)
            {
                _inited = true;
                ((EventFilter) GetService(typeof(EventFilter))).KeyDown += KeyListner;
            }
        }

        private delegate void ActionDelegate(Control control, Point point);
    }
}