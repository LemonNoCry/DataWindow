using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DataWindow.DesignLayer;

namespace DataWindow.DesignerInternal.Event
{
    internal class MegaAction
    {
        private readonly List<DesignerAction> _actions;

        private readonly DesignerHost _host;

        private readonly ObjectHolder _objects;

        private Dictionary<string, string> _removedControls;

        public MegaAction(DesignerHost host)
        {
            _host = host;
            _actions = new List<DesignerAction>();
            _objects = new ObjectHolder();
        }

        public void StartActions()
        {
            var componentChangeService = (IComponentChangeService) _host.GetService(typeof(IComponentChangeService));
            componentChangeService.ComponentAdded += ComponentAdded;
            componentChangeService.ComponentChanged += ComponentChanged;
            componentChangeService.ComponentChanging += ComponentChanging;
            componentChangeService.ComponentRemoving += ComponentRemoving;
        }

        public void StopActions()
        {
            var componentChangeService = (IComponentChangeService) _host.GetService(typeof(IComponentChangeService));
            componentChangeService.ComponentAdded -= ComponentAdded;
            componentChangeService.ComponentChanging -= ComponentChanging;
            componentChangeService.ComponentChanged -= ComponentChanged;
            componentChangeService.ComponentRemoving -= ComponentRemoving;
            _objects.Clear();
        }

        public void Undo()
        {
            for (var i = _actions.Count - 1; i >= 0; i--) _actions[i].Undo();
            if (_removedControls != null)
                foreach (var keyValuePair in _removedControls)
                {
                    var container = (IContainer) _host.GetService(typeof(IContainer));
                    var control = container.Components[keyValuePair.Key] as Control;
                    var control2 = container.Components[keyValuePair.Value] as Control;
                    if (control != null && control2 != null && control.Parent == null) control.Parent = control2;
                }
        }

        public void Redo()
        {
            for (var i = 0; i < _actions.Count; i++) _actions[i].Redo();
        }

        public void LoadProperties(object component, Hashtable props, DesignerHost host)
        {
            var properties = TypeDescriptor.GetProperties(component);
            var enumerator = props.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var text = (string) enumerator.Key;
                if (text != "Image" || !props.ContainsKey("ImageList"))
                {
                    var propertyDescriptor = properties.Find(text, false);
                    if (propertyDescriptor != null)
                        try
                        {
                            if (propertyDescriptor.Name == "Parent" && host != null)
                            {
                                host.Parents[component] = enumerator.Value;
                            }
                            else
                            {
                                if (propertyDescriptor.IsReadOnly && propertyDescriptor.SerializationVisibility == DesignerSerializationVisibility.Content)
                                {
                                    var value = propertyDescriptor.GetValue(component);
                                    var enumerator2 = TypeDescriptor.GetProperties(enumerator.Value).GetEnumerator();
                                    while (enumerator2.MoveNext())
                                    {
                                        var obj = enumerator2.Current;
                                        var propertyDescriptor2 = (PropertyDescriptor) obj;
                                        if (propertyDescriptor2.IsBrowsable && !propertyDescriptor2.IsReadOnly)
                                            try
                                            {
                                                var value2 = propertyDescriptor2.GetValue(enumerator.Value);
                                                propertyDescriptor2.SetValue(value, value2);
                                            }
                                            catch (Exception)
                                            {
                                            }
                                    }

                                    continue;
                                }

                                propertyDescriptor.SetValue(component, enumerator.Value);
                            }
                        }
                        catch (Exception)
                        {
                        }
                }
            }
        }

        public Hashtable StoreProperties(object control, DesignerHost host, PropertyDescriptor propDescriptor)
        {
            var hashtable = new Hashtable();
            if (control is Designer)
            {
                return hashtable;
            }

            foreach (var obj in TypeDescriptor.GetProperties(control))
            {
                var propertyDescriptor = (PropertyDescriptor) obj;
                try
                {
                    if (propertyDescriptor.IsBrowsable && (!propertyDescriptor.IsReadOnly || propertyDescriptor.SerializationVisibility == DesignerSerializationVisibility.Content))
                    {
                        var obj2 = propertyDescriptor.GetValue(control);
                        if (obj2 != null)
                        {
                            if (!(control is DesignSurface) && propertyDescriptor.Name == "Parent" && host != null)
                            {
                                var parents = host.Parents;
                                if (obj2 != parents[control])
                                {
                                    var value = obj2;
                                    if (parents[control] != null) obj2 = parents[control];
                                    parents[control] = value;
                                }
                            }

                            if (!hashtable.Contains(propertyDescriptor.Name)) hashtable.Add(propertyDescriptor.Name, obj2);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            if (propDescriptor != null && propDescriptor.Name != "Parent")
                try
                {
                    var value2 = propDescriptor.GetValue(control);
                    if (value2 != null) hashtable[propDescriptor.Name] = value2;
                }
                catch (Exception)
                {
                }

            return hashtable;
        }

        private void ComponentAdded(object sender, ComponentEventArgs e)
        {
            var item = new AddAction(_host, e.Component, this);
            _actions.Add(item);
        }

        private void ComponentChanging(object sender, ComponentChangingEventArgs e)
        {
            _objects.Add(e.Component, StoreProperties(e.Component, _host, null));
        }

        private void ComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            var propData = _objects.Get(e.Component);
            if (propData != null)
            {
                var item = new ChangeAction(_host, e.Component, propData.Properties, StoreProperties(e.Component, _host, e.Member as PropertyDescriptor), this);
                _actions.Add(item);
            }
        }

        private void ComponentRemoving(object sender, ComponentEventArgs e)
        {
            var item = new RemoveAction(_host, e.Component, this);
            _actions.Add(item);
            var control = e.Component as Control;
            if (control != null && control.Controls.Count != 0)
            {
                if (_removedControls == null) _removedControls = new Dictionary<string, string>();
                foreach (var obj in control.Controls)
                {
                    var control2 = (Control) obj;
                    _removedControls[control2.Name] = control.Name;
                }
            }
        }

        private class PropData
        {
            public PropData(object component, Hashtable properties)
            {
                Component = component;
                Properties = properties;
            }

            public object Component { get; }

            public Hashtable Properties { get; }
        }

        private class ObjectHolder : Collection<PropData>
        {
            public void Add(object component, Hashtable properties)
            {
                base.Add(new PropData(component, properties));
            }

            public PropData Get(object component)
            {
                for (var i = 0; i < Count; i++)
                {
                    var propData = base[i];
                    if (propData.Component == component)
                    {
                        RemoveAt(i);
                        return propData;
                    }
                }

                return null;
            }
        }
    }
}