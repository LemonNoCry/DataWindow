using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace DataWindow.DesignerInternal
{
    internal class ISelectionServiceImpl : ISelectionService
    {
        private readonly IDesignerHost host;

        private readonly ArrayList selectedComponents;

        private IComponent removedComponent;

        public ISelectionServiceImpl(IDesignerHost host)
        {
            this.host = host;
            selectedComponents = new ArrayList();
            IComponentChangeService componentChangeService;
            if ((componentChangeService = host.GetService(typeof(IComponentChangeService)) as IComponentChangeService) != null) componentChangeService.ComponentRemoving += OnComponentRemoving;
        }

        public event EventHandler SelectionChanging;

        public event EventHandler SelectionChanged;

        public object PrimarySelection
        {
            get
            {
                if (selectedComponents.Count > 0) return selectedComponents[0];
                return null;
            }
        }

        public int SelectionCount => selectedComponents.Count;

        public ICollection GetSelectedComponents()
        {
            return selectedComponents.ToArray();
        }

        public bool GetComponentSelected(object component)
        {
            return selectedComponents.Contains(component);
        }

        public void SetSelectedComponents(ICollection components, SelectionTypes selectionType)
        {
            var flag = false;
            var flag2 = false;
            if (removedComponent != null && components != null && components.Count == 1)
            {
                var enumerator = components.GetEnumerator();
                enumerator.MoveNext();
                if (enumerator.Current == removedComponent) return;
            }

            if (components == null) components = new object[1];
            var designerHost = host as DesignerHost;
            var arrayList = new ArrayList(selectedComponents);
            if ((selectionType & SelectionTypes.Primary) == SelectionTypes.Primary)
            {
                flag = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                flag2 = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
            }

            if (selectionType == SelectionTypes.Replace)
            {
                selectedComponents.Clear();
                var enumerator2 = components.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    var obj = enumerator2.Current;
                    if (obj != removedComponent && obj != null && !selectedComponents.Contains(obj) && (designerHost == null || designerHost.DesignContainer != null || obj != host.RootComponent)) selectedComponents.Add(obj);
                }

                goto IL_241;
            }

            if (!flag && !flag2 && components.Count == 1)
                foreach (var item in components)
                    if (!selectedComponents.Contains(item))
                        selectedComponents.Clear();
            foreach (var obj2 in components)
                if (obj2 != removedComponent && obj2 != null && (designerHost == null || designerHost.DesignContainer != null || obj2 != host.RootComponent))
                {
                    if (flag || flag2)
                    {
                        if (selectedComponents.Contains(obj2))
                            selectedComponents.Remove(obj2);
                        else
                            selectedComponents.Insert(0, obj2);
                    }
                    else if (!selectedComponents.Contains(obj2))
                    {
                        selectedComponents.Add(obj2);
                    }
                    else
                    {
                        selectedComponents.Remove(obj2);
                        selectedComponents.Insert(0, obj2);
                    }
                }

            IL_241:
            var flag3 = true;
            if (arrayList.Count != selectedComponents.Count)
                flag3 = false;
            else
                for (var i = 0; i < arrayList.Count; i++)
                {
                    var obj3 = arrayList[i];
                    var obj4 = selectedComponents[i];
                    if (!obj3.Equals(obj4))
                    {
                        flag3 = false;
                        break;
                    }
                }

            if (!flag3)
                try
                {
                    var selectionChanging = SelectionChanging;
                    if (selectionChanging != null) selectionChanging(this, EventArgs.Empty);
                    var selectionChanged = SelectionChanged;
                    if (selectionChanged != null) selectionChanged(this, EventArgs.Empty);
                }
                catch (Exception)
                {
                }
        }

        public void SetSelectedComponents(ICollection components)
        {
            SetSelectedComponents(components, SelectionTypes.Replace);
        }

        internal void OnSelectionChanging(EventArgs e)
        {
            var selectionChanging = SelectionChanging;
            if (selectionChanging == null) return;
            selectionChanging(this, e);
        }

        internal void OnSelectionChanged(EventArgs e)
        {
            var selectionChanged = SelectionChanged;
            if (selectionChanged == null) return;
            selectionChanged(this, e);
        }

        internal void OnComponentRemoving(object sender, ComponentEventArgs e)
        {
            removedComponent = e.Component;
        }

        internal void OnComponentRemoved(object sender, ComponentEventArgs e)
        {
            removedComponent = null;
            if (selectedComponents.Contains(e.Component))
            {
                OnSelectionChanging(EventArgs.Empty);
                selectedComponents.Remove(e.Component);
                OnSelectionChanged(EventArgs.Empty);
            }
        }
    }
}