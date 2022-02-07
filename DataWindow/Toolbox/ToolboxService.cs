using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using DataWindow.DesignLayer;

namespace DataWindow.Toolbox
{
    public class ToolboxService : IToolboxService, IDisposable
    {
        private readonly Dictionary<string, ToolboxItemCreatorCallback> _creators;

        private readonly List<Designer> _designers;

        private readonly IToolbox _toolbox;

        private Designer _defaultDesigner;

        public ToolboxService(IToolbox toolbox)
        {
            _toolbox = toolbox;
            _toolbox.BeginDragAndDrop += OnDragAndDrop;
            _toolbox.DropControl += OnDropControl;
            _creators = new Dictionary<string, ToolboxItemCreatorCallback>();
            _designers = new List<Designer>();
        }

        public Designer Designer
        {
            get => _defaultDesigner;
            set
            {
                if (value == null)
                    RemoveDesigner(_defaultDesigner);
                else if (!_designers.Exists(d => d == value)) AddDesigner(value);
                _defaultDesigner = value;
            }
        }

        public void Dispose()
        {
            _toolbox.BeginDragAndDrop -= OnDragAndDrop;
            _toolbox.DropControl -= OnDropControl;
        }

        public CategoryNameCollection CategoryNames
        {
            get
            {
                var items = _toolbox.Items;
                if (items.Count <= 0) return null;
                return new CategoryNameCollection((from c in items
                                                   select c.Name).ToArray());
            }
        }

        public string SelectedCategory
        {
            get => _toolbox.SelectedCategory;
            set => _toolbox.SelectedCategory = value;
        }

        public void AddCreator(ToolboxItemCreatorCallback creator, string format)
        {
            AddCreator(creator, format, null);
        }

        public void AddCreator(ToolboxItemCreatorCallback creator, string format, IDesignerHost host)
        {
            _creators[format] = creator;
        }

        public void AddLinkedToolboxItem(ToolboxItem toolboxItem, IDesignerHost host)
        {
        }

        public void AddLinkedToolboxItem(ToolboxItem toolboxItem, string category, IDesignerHost host)
        {
        }

        public void AddToolboxItem(ToolboxItem toolboxItem)
        {
            _toolbox.AddItem(toolboxItem, null);
        }

        public void AddToolboxItem(ToolboxItem toolboxItem, string category)
        {
            _toolbox.AddItem(toolboxItem, category);
        }

        public ToolboxItem DeserializeToolboxItem(object serializedObject)
        {
            return DeserializeToolboxItem(serializedObject, null);
        }

        public ToolboxItem DeserializeToolboxItem(object serializedObject, IDesignerHost host)
        {
            Console.WriteLine(1);
            var dataObject = serializedObject as IDataObject;
            var typeFromHandle = typeof(ToolboxItem);
            foreach (var text in dataObject.GetFormats())
            {
                ToolboxItemCreatorCallback toolboxItemCreatorCallback;
                if (_creators.TryGetValue(text, out toolboxItemCreatorCallback))
                {
                    ToolboxItem toolboxItem = null;
                    try
                    {
                        toolboxItem = toolboxItemCreatorCallback(serializedObject, text);
                    }
                    catch
                    {
                    }

                    if (toolboxItem != null) return toolboxItem;
                }
                else
                {
                    var data = dataObject.GetData(text);
                    if (typeFromHandle.IsAssignableFrom(data.GetType())) return data as ToolboxItem;
                }
            }

            return null;
        }

        public ToolboxItem GetSelectedToolboxItem()
        {
            return GetSelectedToolboxItem(null);
        }

        public ToolboxItem GetSelectedToolboxItem(IDesignerHost host)
        {
            return _toolbox.SelectedItem;
        }

        public ToolboxItemCollection GetToolboxItems(string category, IDesignerHost host)
        {
            var toolboxCategoryItem = _toolbox.Items.FirstOrDefault(i => i.Name == category);
            if (toolboxCategoryItem != null) return new ToolboxItemCollection(toolboxCategoryItem.Items);
            return null;
        }

        public ToolboxItemCollection GetToolboxItems(string category)
        {
            return GetToolboxItems(category, null);
        }

        public ToolboxItemCollection GetToolboxItems(IDesignerHost host)
        {
            var list = new List<ToolboxItem>();
            foreach (var toolboxCategoryItem in _toolbox.Items)
            {
                foreach (var obj in toolboxCategoryItem.Items)
                {
                    var item = (ToolboxItem) obj;
                    list.Add(item);
                }
            }

            return new ToolboxItemCollection(list.ToArray());
        }

        public ToolboxItemCollection GetToolboxItems()
        {
            IDesignerHost host = null;
            return GetToolboxItems(host);
        }

        public bool IsSupported(object serializedObject, ICollection filterAttributes)
        {
            return true;
        }

        public bool IsSupported(object serializedObject, IDesignerHost host)
        {
            return true;
        }

        public bool IsToolboxItem(object serializedObject, IDesignerHost host)
        {
            return DeserializeToolboxItem(serializedObject, host) != null;
        }

        public bool IsToolboxItem(object serializedObject)
        {
            return IsToolboxItem(serializedObject, null);
        }

        public void Refresh()
        {
            _toolbox.Refresh();
        }

        public void RemoveCreator(string format, IDesignerHost host)
        {
            _creators.Remove(format);
        }

        public void RemoveCreator(string format)
        {
            RemoveCreator(format, null);
        }

        public void RemoveToolboxItem(ToolboxItem toolboxItem, string category)
        {
            _toolbox.RemoveItem((ToolboxItem) toolboxItem, category);
        }

        public void RemoveToolboxItem(ToolboxItem toolboxItem)
        {
            _toolbox.RemoveItem((ToolboxItem) toolboxItem, null);
        }

        public void SelectedToolboxItemUsed()
        {
            _toolbox.SelectedItem = null;
        }

        public object SerializeToolboxItem(ToolboxItem toolboxItem)
        {
            if (toolboxItem != null) return new DataObject(toolboxItem);
            return null;
        }

        public bool SetCursor()
        {
            var cursor = _toolbox.SelectedItem == null ? Cursors.Arrow : Cursors.Cross;
            if (cursor == Cursors.Arrow)
            {
                foreach (var designer in _designers)
                {
                    if (designer.DesignContainer != null || designer.DesignedForm == null) return false;
                    designer.DesignedForm.Cursor = Cursors.Arrow;
                }

                return false;
            }

            foreach (var designer2 in _designers)
                if (designer2.DesignContainer != null)
                    designer2.DesignContainer.Cursor = cursor;
                else
                    designer2.DesignedForm.Cursor = cursor;
            return true;
        }

        public void SetSelectedToolboxItem(ToolboxItem toolboxItem)
        {
            _toolbox.SelectedItem = (ToolboxItem) toolboxItem;
        }

        public void AddDesigner(Designer designer)
        {
            if (designer != null)
            {
                designer.DesignerHost.AddService(typeof(IToolboxService), this);
                _designers.Add(designer);
                if (_designers.Count == 1) _defaultDesigner = designer;
            }
        }

        public void RemoveDesigner(Designer designer)
        {
            if (_designers.Exists(d => d == designer))
            {
                designer.DesignerHost.RemoveService(typeof(IToolboxService));
                _designers.Remove(designer);
                if (_defaultDesigner == designer)
                {
                    if (_designers.Count > 0)
                    {
                        _defaultDesigner = _designers[0];
                        return;
                    }

                    _defaultDesigner = null;
                }
            }
        }

        private void OnDragAndDrop(object sender, EventArgs e)
        {
            var selectedItem = _toolbox.SelectedItem;
            Control control;
            if (selectedItem != null && (control = _toolbox as Control) != null)
            {
                var data = (DataObject) SerializeToolboxItem(selectedItem);
                control.DoDragDrop(data, DragDropEffects.All);
            }
        }

        private void OnDropControl(object sender, EventArgs e)
        {
            var selectedItem = _toolbox.SelectedItem;
            var designer = Designer;
            if (selectedItem != null && designer != null)
            {
                using (var designerTransaction = designer.DesignerHost.CreateTransaction("Create component"))
                {
                    var component = selectedItem.CreateComponents(designer.DesignerHost)[0];
                    Control control;
                    if ((control = component as Control) != null)
                        try
                        {
                            var designerHost = designer.DesignerHost;
                            var control2 = (Control) designerHost.RootComponent;
                            var componentChangeService = (IComponentChangeService) designerHost.GetService(typeof(IComponentChangeService));
                            //((ComponentDesigner) designerHost.GetDesigner(component)).InitializeNewComponent(new Hashtable());
                            componentChangeService.OnComponentChanging(component, null);
                            control.SuspendLayout();
                            control.Parent = control2;
                            control.Location = new Point((control2.Width - control.Width) / 2, (control2.Height - control.Height) / 2);
                            if (string.IsNullOrEmpty(control.Text))
                            {
                                control.Text = control.Name;
                            }

                            control.ResumeLayout();
                            componentChangeService.OnComponentChanged(component, null, null, null);
                            var selectionService = (ISelectionService) designerHost.GetService(typeof(ISelectionService));
                            Control[] components =
                            {
                                control
                            };
                            selectionService.SetSelectedComponents(components, SelectionTypes.Replace);
                        }
                        catch (Exception)
                        {
                        }

                    designerTransaction.Commit();
                }

                _toolbox.SelectedItem = null;
            }
        }

        internal ToolboxItem CreateToolboxItem(Type type)
        {
            ToolboxItemAttribute toolboxItemAttribute;
            if ((toolboxItemAttribute = TypeDescriptor.GetAttributes(type)[typeof(ToolboxItemAttribute)] as ToolboxItemAttribute) != null)
            {
                var constructor = toolboxItemAttribute.ToolboxItemType.GetConstructor(new Type[0]);
                if (constructor != null)
                {
                    var toolboxItem = (ToolboxItem) constructor.Invoke(new object[0]);
                    toolboxItem.Initialize(type);
                    return toolboxItem;
                }
            }

            return new ToolboxItem(type);
        }
    }
}