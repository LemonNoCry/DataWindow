using DataWindow.DesignerInternal;
using DataWindow.DesignerInternal.Event;
using DataWindow.DesignerInternal.Interface;
using DataWindow.Serialization.Components;
using DataWindow.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml;
using DataWindow.Core;

namespace DataWindow.DesignLayer
{
    [ToolboxBitmap(typeof(Designer))]
    public class Designer : Component
    {
        private const string CLIPBOARD_FORMAT = "_SmartFormDesigner_Controls";

        private readonly Dictionary<string, ComponentsCreatorCallback> _creatorCallbacks;

        private readonly List<Control> _pastedControls = new List<Control>();

        private bool _active;

        private Dictionary<object, Dictionary<string, object>> _changedDefaults;

        private Dictionary<string, IComponent> _designedComponents;

        private DesignerHost _designerHost;

        private IDesignerLoader _designerLoader;

        private Size _pasteShift = new Size(0, 0);

        public Designer()
        {
            _creatorCallbacks = new Dictionary<string, ComponentsCreatorCallback>();
            _designerHost = new DesignerHost();
            _designerHost.Owner = this;
            _designerHost.AddService(typeof(Designer), this);
            DesignEvents = (IDesignEvents) _designerHost.GetService(typeof(IDesignEvents));
            SelectionService = (ISelectionService) _designerHost.GetService(typeof(ISelectionService));
            ComponentChangeService = (IComponentChangeService) _designerHost.GetService(typeof(IComponentChangeService));
        }

        [Category("设计器选项")]
        [DefaultValue(true)]
        [Description("是否启用智能标记")]
        public bool UseSmartTags
        {
            get => GetDesignerOption<bool>("UseSmartTags");
            set => SetDesignerOption("UseSmartTags", value);
        }

        [Category("设计器选项")]
        [DefaultValue(true)]
        [Description("是否自动打开对象绑定智能标记")]
        public bool ObjectBoundSmartTagAutoShow
        {
            get => GetDesignerOption<bool>("ObjectBoundSmartTagAutoShow");
            set => SetDesignerOption("ObjectBoundSmartTagAutoShow", value);
        }

        [Category("设计器选项")]
        [DefaultValue(true)]
        [Description("启用或禁用设计器中的对齐线")]
        public bool UseSnapLines
        {
            get => GetDesignerOption<bool>("UseSnapLines");
            set => SetDesignerOption("UseSnapLines", value);
        }

        [Category("设计器选项")]
        [DefaultValue(true)]
        [Description("对齐到网格")]
        public bool SnapToGrid
        {
            get => GetDesignerOption<bool>("SnapToGrid");
            set => SetDesignerOption("SnapToGrid", value);
        }

        [Category("设计器选项")]
        [DefaultValue(false)]
        [Description("是否显示风格")]
        public bool ShowGrid
        {
            get => GetDesignerOption<bool>("ShowGrid");
            set => SetDesignerOption("ShowGrid", value);
        }

        [Category("设计器选项")]
        [DefaultValue(524296)]
        [Description("设计器网格大小")]
        public Size GridSize
        {
            get => GetDesignerOption<Size>("GridSize");
            set => SetDesignerOption("GridSize", value);
        }

        [Browsable(false)]
        [DefaultValue(false)]
        public bool Active
        {
            get => _active;
            set
            {
                if (_active != value)
                {
                    _active = value;
                    if (_designerLoader != null) _designerLoader.DesignerHost = value ? _designerHost : null;
                    EnableEdit(value);
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(null)]
        public Dictionary<string, IComponent> DesignedComponents
        {
            get => _designedComponents;
            set
            {
                if (_designedComponents != null)
                {
                    var flag = false;
                    if (_designerLoader != null && _designerLoader.DesignerHost == null)
                    {
                        flag = true;
                        _designerLoader.DesignerHost = _designerHost;
                    }

                    FormComponents formComponents;
                    if ((formComponents = _designerHost.GetService(typeof(FormComponents)) as FormComponents) != null)
                        using (var enumerator = _designedComponents.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                var item = enumerator.Current;
                                var keyValuePair = formComponents.Components.FirstOrDefault(c => c.Value == item.Value);
                                if (keyValuePair.Key != null) formComponents.Remove(keyValuePair.Key);
                            }
                        }

                    if (flag) _designerLoader.DesignerHost = null;
                }

                _designedComponents = value;
            }
        }

        [Browsable(false)]
        [DefaultValue(null)]
        public SortedList<string, string> DesignedContainers => _designerHost.DesignedContainers;

        [DefaultValue(null)]
        public Control DesignContainer
        {
            get => _designerHost.DesignContainer;
            set => _designerHost.DesignContainer = value;
        }

        [DefaultValue(null)]
        public Control DesignedForm
        {
            get => _designerHost.DesignedForm;
            set
            {
                _designerHost.DesignedForm = value;
                if (_designerLoader != null) _designerLoader.SetEventSource(value);
            }
        }

        [Browsable(false)]
        [DefaultValue(null)]
        public DesignerHost DesignerHost => _designerHost;

        [Browsable(true)]
        [Description("获取或设置 DesignerLoader")]
        [DefaultValue(null)]
        public IDesignerLoader DesignerLoader
        {
            get => _designerLoader;
            set
            {
                if (_designerLoader != null) _designerLoader.DesignerHost = null;
                _designerLoader = value;
                if (_designerLoader != null) _designerLoader.ShowErrorMessage = _designerHost.ShowErrorMessage;
            }
        }

        [DefaultValue(LoadModes.Default)]
        [Description("")]
        public LoadModes LoadMode
        {
            get
            {
                if (_designerLoader == null) return LoadModes.Default;
                return _designerLoader.LoadMode;
            }
            set
            {
                if (_designerLoader != null) _designerLoader.LoadMode = value;
            }
        }

        [DefaultValue(false)]
        [Description("启用或禁用显示错误消息框")]
        public bool ShowErrorMessage
        {
            get => _designerHost.ShowErrorMessage;
            set
            {
                _designerHost.ShowErrorMessage = value;
                if (_designerLoader != null) _designerLoader.ShowErrorMessage = value;
            }
        }

        [Browsable(false)]
        [DefaultValue(null)]
        public string LogFile
        {
            get => _designerHost.LogName;
            set
            {
                _designerHost.LogName = value;
                if (_designerLoader != null) _designerLoader.LogFile = value;
            }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        public bool IsDirty { get; private set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(null)]
        public string LayoutXml
        {
            get
            {
                var stringWriter = new StringWriter();
                using (IWriter writer = new XmlFormWriter(new XmlTextWriter(stringWriter)))
                {
                    StoreInternal(writer);
                }

                ClearDirty();
                return stringWriter.ToString();
            }
            set
            {
                using (IReader reader = new XmlFormReader(new XmlTextReader(new StringReader(value))))
                {
                    LoadInternal(reader);
                }

                ClearDirty();
            }
        }

        public override ISite Site
        {
            get => base.Site;
            set
            {
                if (base.Site != value) base.Site = value;
            }
        }

        [Browsable(false)]
        [DefaultValue(0)]
        public int UndoCount => _designerHost.UndoCount;

        [Browsable(false)]
        [DefaultValue(0)]
        public int RedoCount => _designerHost.RedoCount;

        [Browsable(false)]
        public ISelectionService SelectionService { get; }

        [Browsable(false)]
        public IComponentChangeService ComponentChangeService { get; }

        [Browsable(false)]
        public IDesignEvents DesignEvents { get; }

        public event EventHandler DirtyChanged;

        public event KeyEventHandler KeyUp
        {
            add => GetEventFilterService().KeyUp += value;
            remove => GetEventFilterService().KeyUp -= value;
        }

        public event KeyEventHandler KeyDown
        {
            add => GetEventFilterService().KeyDown += value;
            remove => GetEventFilterService().KeyDown -= value;
        }

        public event EventHandler DoubleClick
        {
            add => GetEventFilterService().DoubleClick += value;
            remove => GetEventFilterService().DoubleClick -= value;
        }

        public event MouseEventHandler MouseUp
        {
            add => GetEventFilterService().MouseUp += value;
            remove => GetEventFilterService().MouseUp -= value;
        }

        public event MouseEventHandler MouseDown
        {
            add => GetEventFilterService().MouseDown += value;
            remove => GetEventFilterService().MouseDown -= value;
        }

        public event AllowDesignHandler AllowDesign
        {
            add => ((IDesignEvents) _designerHost.GetService(typeof(IDesignEvents))).AllowDesign += value;
            remove => ((IDesignEvents) _designerHost.GetService(typeof(IDesignEvents))).AllowDesign -= value;
        }

        private EventFilter GetEventFilterService()
        {
            return (EventFilter) _designerHost.GetService(typeof(EventFilter));
        }

        private void SetDesignerOption<T>(string name, T value)
        {
            ((DesignerOptionService) _designerHost.GetService(typeof(DesignerOptionService))).Options.Properties[name].SetValue(null, value);
        }

        private T GetDesignerOption<T>(string name)
        {
            if (_designerHost == null)
            {
                return default;
            }

            return (T) ((DesignerOptionService) _designerHost.GetService(typeof(DesignerOptionService))).Options.Properties[name].GetValue(null);
        }

        ~Designer()
        {
            base.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _designerHost.Dispose();
                _designerHost = null;
            }

            base.Dispose(disposing);
        }

        public void InitLayout(string xml)
        {
            using (IReader reader = new XmlFormReader(new XmlTextReader(new StringReader(xml))))
            {
                LoadInternal(reader);
            }

            ClearDirty();
            ResetUndo();
        }

        private bool LoadInternal(IReader reader)
        {
            if (_designerLoader == null)
            {
                NoDesignerLoaderControl();
                return false;
            }

            if (_designerHost.DesignedForm.HasChildren)
            {
                _designerHost.DesignedForm.Controls.Clear();
            }

            _designerLoader.Components = _designerHost.GetService(typeof(FormComponents)) as FormComponents;
            _designerLoader.Load(_designerHost.DesignedForm, reader, _designedComponents, false);
            _designerLoader.Components = null;
            ClearDirty();
            return true;
        }

        public void LoadFromFile(string fileName)
        {
            if (fileName.EndsWith(".xml"))
                using (var xmlFormReader = new XmlFormReader(fileName))
                {
                    LoadInternal(xmlFormReader);
                    return;
                }

            using (var textFormReader = new TextFormReader(fileName))
            {
                LoadInternal(textFormReader);
            }
        }

        public void InitLoadFromFile(string fileName)
        {
            LoadFromFile(fileName);
            ResetUndo();
        }

        public void Load(ref XmlReader reader)
        {
            using (var xmlFormReader = new XmlFormReader(reader))
            {
                LoadInternal(xmlFormReader);
            }
        }

        public void Store(ref XmlWriter writer)
        {
            using (var xmlFormWriter = new XmlFormWriter(writer))
            {
                StoreInternal(xmlFormWriter);
            }
        }

        public void SaveToFile(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if ((extension != null ? extension.ToLower() : null) == ".xml")
                using (var xmlFormWriter = new XmlFormWriter(fileName))
                {
                    StoreInternal(xmlFormWriter);
                    return;
                }

            using (var textFormWriter = new TextFormWriter(fileName))
            {
                StoreInternal(textFormWriter);
            }
        }

        private bool StoreInternal(IWriter writer)
        {
            if (_designerLoader == null)
            {
                NoDesignerLoaderControl();
                return false;
            }

            var list = new List<IComponent>();
            var list2 = list;
            IComponent item;
            if (!Active || DesignContainer == null)
            {
                IComponent designedForm = _designerHost.DesignedForm;
                item = designedForm;
            }
            else
            {
                item = _designerHost.RootComponent;
            }

            list2.Add(item);
            if (_designedComponents != null) list.AddRange(_designedComponents.Values);
            foreach (var obj in _designerHost.Components)
            {
                var component = (IComponent) obj;
                if (!(component is Control))
                {
                    var designedComponents = _designedComponents;
                    if (designedComponents == null || !designedComponents.Contains(component)) list.Add(component);
                }
            }

            _designerLoader.Store(list.ToArray(), writer);
            ClearDirty();
            return true;
        }

        public void SelectAll()
        {
            var selectionService = (ISelectionService) _designerHost.GetService(typeof(ISelectionService));
            var control = _designerHost.RootComponent as Control;
            selectionService.SetSelectedComponents(control.Controls, SelectionTypes.Replace);
        }

        public void CopyControls()
        {
            if (_designerLoader == null)
            {
                NoDesignerLoaderControl();
                return;
            }

            var selectedComponents = ((ISelectionService) _designerHost.GetService(typeof(ISelectionService))).GetSelectedComponents();
            if (selectedComponents.Count == 0) return;
            if (selectedComponents.Count == 1)
            {
                var enumerator = selectedComponents.GetEnumerator();
                while (enumerator.MoveNext())
                    if ((IComponent) enumerator.Current == _designerHost.RootComponent)
                        return;
            }

            var formComponents = (FormComponents) _designerHost.GetService(typeof(FormComponents));
            IDictionary<string, IComponent> dictionary = null;
            if (formComponents != null)
            {
                dictionary = formComponents.Components;
                formComponents.Clear();
            }

            using (var stringWriter = new StringWriter())
            {
                IWriter writer = new TextFormWriter(stringWriter);
                var array = new Control[selectedComponents.Count];
                var num = 0;
                foreach (var obj in selectedComponents)
                {
                    var component = (IComponent) obj;
                    array[num++] = component as Control;
                }

                var designerLoader = _designerLoader;
                IComponent[] parents = array;
                designerLoader.Store(parents, writer);
                Clipboard.SetData("_SmartFormDesigner_Controls", stringWriter.ToString());
            }

            if (formComponents != null)
                foreach (var keyValuePair in dictionary)
                    formComponents.Add(keyValuePair.Key, keyValuePair.Value);
            _pasteShift = GridSize;
        }

        public void PasteControls()
        {
            _pastedControls.Clear();
            if (_designerLoader == null)
            {
                NoDesignerLoaderControl();
                return;
            }

            var data = Clipboard.GetData("_SmartFormDesigner_Controls");
            if (data == null) return;
            var loadMode = _designerLoader.LoadMode;
            _designerLoader.LoadMode = LoadModes.Duplicate;
            var selectionService = (ISelectionService) _designerHost.GetService(typeof(ISelectionService));
            var control = _designerHost.RootComponent as Control;
            if (selectionService.SelectionCount == 1)
            {
                var enumerator = selectionService.GetSelectedComponents().GetEnumerator();
                enumerator.MoveNext();
                Control control2;
                if ((control2 = enumerator.Current as Control) != null)
                {
                    var designer = _designerHost.GetDesigner(control2);
                    if (typeof(ParentControlDesigner).IsAssignableFrom(designer.GetType())) control = control2;
                }
            }

            using (var designerTransaction = _designerHost.CreateTransaction("Paste controls"))
            {
                using (var stringReader = new StringReader(data as string))
                {
                    var reader = new TextFormReader(stringReader);
                    _designerLoader.ComponentLoaded += PasteControlHandler;
                    _designerLoader.Load(control != null ? control : _designerHost.DesignedForm, reader, null, true);
                    _designerLoader.ComponentLoaded -= PasteControlHandler;
                }

                var control3 = _designerHost.RootComponent as Control;
                var componentChangeService = (IComponentChangeService) _designerHost.GetService(typeof(IComponentChangeService));
                foreach (var control4 in _pastedControls)
                {
                    componentChangeService.OnComponentChanging(control4, null);
                    if (control4.Parent == control3) control4.Location += _pasteShift;
                    if (_designerHost.Components[control4.Text] != null) control4.Text = control4.Name;
                    control4.BringToFront();
                    componentChangeService.OnComponentChanged(control4, null, null, null);
                }

                designerTransaction.Commit();
                _pasteShift += GridSize;
            }

            selectionService.SetSelectedComponents(_pastedControls.ToArray(), SelectionTypes.Replace);
            SetDirty();
            _designerLoader.LoadMode = loadMode;
        }

        private void PasteControlHandler(object sender, ComponentEventArgs e)
        {
            if (_designerLoader == null)
            {
                NoDesignerLoaderControl();
                return;
            }

            Control item;
            if ((item = e.Component as Control) != null) _pastedControls.Add(item);
        }

        public void DeleteSelected()
        {
            if (!_active) return;
            var selectionService = (ISelectionService) _designerHost.GetService(typeof(ISelectionService));
            if (selectionService.SelectionCount == 0) return;
            var selectedComponents = selectionService.GetSelectedComponents();
            using (var designerTransaction = _designerHost.CreateTransaction("Delete selected"))
            {
                IBaseDataWindow bdw = DesignedForm as IBaseDataWindow;
                foreach (var obj in selectedComponents)
                {
                    var component = (IComponent) obj;
                    try
                    {
                        if (bdw != null)
                        {
                            if (bdw.IsMustControl(component.Site.Name))
                            {
                                if (selectedComponents.Count == 1)
                                {
                                    MessageBox.Show("当前控件不允许删除");
                                    return;
                                }

                                continue;
                            }
                        }

                        if (component != _designerHost.RootComponent)
                        {
                            _designerHost.Remove(component);
                            component.Dispose();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                designerTransaction.Commit();
            }
        }

        public void DeleteAllComponents()
        {
            if (!_active) return;
            using (var designerTransaction = _designerHost.CreateTransaction("Delete selected"))
            {
                foreach (var obj in _designerHost.Components)
                {
                    var component = (IComponent) obj;

                    if (_designerHost.Parents[component] == null)
                    {
                        try
                        {
                            if (component != _designerHost.RootComponent)
                            {
                                _designerHost.Remove(component);
                                component.Dispose();
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                designerTransaction.Commit();
            }
        }

        public void ReloadLayoutXml(string xml)
        {
            Active = false;
            DeleteAllComponents();
            LayoutXml = xml;
            Active = true;
        }

        public void MakeSameSize(ResizeType resize)
        {
            if (!_active) return;
            var selectionService = (ISelectionService) _designerHost.GetService(typeof(ISelectionService));
            if (selectionService.SelectionCount < 2) return;
            var componentChangeService = (IComponentChangeService) _designerHost.GetService(typeof(IComponentChangeService));
            var selectedComponents = selectionService.GetSelectedComponents();
            using (var designerTransaction = _designerHost.CreateTransaction("Make Same Size"))
            {
                var control = (Control) selectionService.PrimarySelection;
                var size = control.Size;
                foreach (var obj in selectedComponents)
                {
                    var control2 = (Control) obj;
                    if (control2 != control)
                    {
                        var size2 = control2.Size;
                        if ((resize & ResizeType.SameWidth) != 0) size2.Width = size.Width;
                        if ((resize & ResizeType.SameHeight) != 0) size2.Height = size.Height;
                        var member = TypeDescriptor.GetProperties(control2)["Size"];
                        componentChangeService.OnComponentChanging(control2, member);
                        object oldValue = control2.Size;
                        control2.Size = size2;
                        componentChangeService.OnComponentChanged(control2, member, oldValue, size2);
                    }
                }

                designerTransaction.Commit();
            }
        }

        public void Align(AlignType align)
        {
            if (!_active) return;
            var selectionService = (ISelectionService) _designerHost.GetService(typeof(ISelectionService));
            if (selectionService.SelectionCount < 2) return;
            var componentChangeService = (IComponentChangeService) _designerHost.GetService(typeof(IComponentChangeService));
            using (var designerTransaction = _designerHost.CreateTransaction("Align"))
            {
                var selectedComponents = selectionService.GetSelectedComponents();
                var control = (Control) selectionService.PrimarySelection;
                var location = control.Location;
                var size = control.Size;
                foreach (var obj in selectedComponents)
                {
                    var control2 = (Control) obj;
                    if (control2 != control)
                    {
                        var location2 = control2.Location;
                        if ((align & AlignType.Left) != 0)
                            location2.X = location.X;
                        else if ((align & AlignType.Right) != 0)
                            location2.X = location.X + size.Width - control2.Size.Width;
                        else if ((align & AlignType.Center) != 0) location2.X = location.X + (size.Width - control2.Size.Width) / 2;
                        if ((align & AlignType.Top) != 0)
                            location2.Y = location.Y;
                        else if ((align & AlignType.Bottom) != 0)
                            location2.Y = location.Y + size.Height - control2.Size.Height;
                        else if ((align & AlignType.Middle) != 0) location2.Y = location.Y + (size.Height - control2.Size.Height) / 2;
                        var member = TypeDescriptor.GetProperties(control2)["Location"];
                        componentChangeService.OnComponentChanging(control2, member);
                        object oldValue = control2.Location;
                        control2.Location = location2;
                        componentChangeService.OnComponentChanged(control2, member, oldValue, location2);
                    }
                }

                designerTransaction.Commit();
            }
        }

        public void Lock()
        {
            if (!_active) return;
            var selectionService = (ISelectionService) _designerHost.GetService(typeof(ISelectionService));
            if (selectionService.SelectionCount < 1) return;
            var selectedComponents = selectionService.GetSelectedComponents();
            var componentChangeService = (IComponentChangeService) _designerHost.GetService(typeof(IComponentChangeService));

            using (var designerTransaction = _designerHost.CreateTransaction("Locked"))
            {
                foreach (var component in selectedComponents)
                {
                    PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(component, true);
                    var pd = baseProps.Find("Locked", true);
                    if (pd == null)
                    {
                        continue;
                    }

                    var locked = (bool) pd.GetValue(component);

                    componentChangeService.OnComponentChanging(component, pd);
                    pd.SetValue(component, !locked);
                    componentChangeService.OnComponentChanged(component, pd, locked, !locked);
                }

                designerTransaction.Commit();
            }
        }

        public bool Undo()
        {
            return _designerHost.Undo();
        }

        public bool Redo()
        {
            return _designerHost.Redo();
        }

        public void ResetUndo()
        {
            _designerHost.ResetUndo();
        }

        public void AddDefaultValue(IComponent component, string propName, object value)
        {
            AddDefault(component, propName, value);
        }

        public void AddDefaultValue(Type componentType, string propName, object value)
        {
            AddDefault(componentType, propName, value);
        }

        public void RemoveDefaultValues(IComponent component)
        {
            RemoveDefaults(component);
        }

        public void RemoveDefaultValues(Type componentType)
        {
            RemoveDefaults(componentType);
        }

        private void AddDefault(object component, string propName, object value)
        {
            if (_changedDefaults == null)
            {
                _changedDefaults = new Dictionary<object, Dictionary<string, object>>();
                IDesignEvents designEvents;
                if ((designEvents = _designerHost.GetService(typeof(IDesignEvents)) as IDesignEvents) != null) designEvents.FilterProperties += FilterProperties;
            }

            var dictionary = _changedDefaults[component];
            if (dictionary == null)
            {
                dictionary = new Dictionary<string, object>();
                _changedDefaults[component] = dictionary;
            }

            dictionary[propName] = value;
        }

        private void RemoveDefaults(object component)
        {
            if (_changedDefaults != null)
            {
                _changedDefaults.Remove(component);
                if (_changedDefaults.Count == 0)
                {
                    _changedDefaults = null;
                    (_designerHost.GetService(typeof(IDesignEvents)) as IDesignEvents).FilterProperties -= FilterProperties;
                }
            }
        }

        private void FilterProperties(object sender, FilterEventArgs e)
        {
            Dictionary<string, object> dictionary = null;
            var component = sender as IComponent;
            if (!_changedDefaults.TryGetValue(component, out dictionary)) _changedDefaults.TryGetValue(component.GetType(), out dictionary);
            if (dictionary != null)
                foreach (var keyValuePair in dictionary)
                {
                    var propertyDescriptor = e.Data[keyValuePair.Key] as PropertyDescriptor;
                    if (propertyDescriptor != null)
                    {
                        var defaultValueAttribute = new DefaultValueAttribute(keyValuePair.Value);
                        var value = TypeDescriptor.CreateProperty(component.GetType(), propertyDescriptor, defaultValueAttribute);
                        e.Data[keyValuePair.Key] = value;
                    }
                }
        }

        private void RegisterListners()
        {
            var componentChangeService = (IComponentChangeService) _designerHost.GetService(typeof(IComponentChangeService));
            componentChangeService.ComponentAdded += ComponentAddedOrRemoved;
            componentChangeService.ComponentRemoved += ComponentAddedOrRemoved;
            componentChangeService.ComponentChanged += ComponentChanged;
        }

        private void UnregisterListners()
        {
            var componentChangeService = (IComponentChangeService) _designerHost.GetService(typeof(IComponentChangeService));
            componentChangeService.ComponentAdded -= ComponentAddedOrRemoved;
            componentChangeService.ComponentRemoved -= ComponentAddedOrRemoved;
            componentChangeService.ComponentChanged -= ComponentChanged;
        }

        private void ComponentAddedOrRemoved(object sender, ComponentEventArgs e)
        {
            SetDirty();
        }

        private void ComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            SetDirty();
        }

        public void SetDirty()
        {
            if (!IsDirty)
            {
                IsDirty = true;
                var dirtyChanged = DirtyChanged;
                if (dirtyChanged == null) return;
                dirtyChanged(this, EventArgs.Empty);
            }
        }

        public void ClearDirty()
        {
            if (IsDirty)
            {
                IsDirty = false;
                var dirtyChanged = DirtyChanged;
                if (dirtyChanged == null) return;
                dirtyChanged(this, EventArgs.Empty);
            }
        }

        protected void EnableEdit(bool enable)
        {
            if (enable)
            {
                if (_designerLoader != null) _designerLoader.UnbindEvents(_designerHost.DesignedForm.FindForm());
                _designerHost.StartDesign();
                RegisterListners();
                if (_designedComponents != null)
                    foreach (var keyValuePair in _designedComponents)
                        _designerHost.Container.Add(keyValuePair.Value);
                var formComponents = (FormComponents) _designerHost.GetService(typeof(FormComponents));
                if (formComponents != null)
                {
                    var array = new KeyValuePair<string, IComponent>[formComponents.Components.Count];
                    formComponents.Components.CopyTo(array, 0);
                    foreach (var keyValuePair2 in array)
                    {
                        var value = keyValuePair2.Value;
                        if (value.Site == null) _designerHost.Container.Add(value, keyValuePair2.Key);
                    }
                }

                _designerHost.EndLoad();
                ClearDirty();
                if (_designerLoader != null) _designerLoader.RefreshEventData();
            }
            else
            {
                _designerHost.StopDesign();
                ClearDirty();
                UnregisterListners();
                var designerLoader = _designerLoader;
                if (designerLoader == null) return;
                designerLoader.BindEvents(_designerHost.DesignedForm.FindForm());
            }
        }

        public bool RegisterDragAndDropHandler(string format, ComponentsCreatorCallback creator)
        {
            IToolboxService toolboxService;
            if ((toolboxService = _designerHost.GetService(typeof(IToolboxService)) as IToolboxService) != null)
            {
                ToolboxItemCreatorCallback creator2 = CreateToolboxItem;
                toolboxService.AddCreator(creator2, format);
                _creatorCallbacks[format] = creator;
                return true;
            }

            return false;
        }

        public void UnregisterDragAndDropHandler(string format)
        {
            IToolboxService toolboxService;
            if ((toolboxService = _designerHost.GetService(typeof(IToolboxService)) as IToolboxService) != null)
            {
                toolboxService.RemoveCreator(format);
                _creatorCallbacks.Remove(format);
            }
        }

        private ToolboxItem CreateToolboxItem(object serializedObject, string format)
        {
            ComponentsCreatorCallback callback;
            if (_creatorCallbacks.TryGetValue(format, out callback)) return new ToolboxItemHelper(format, serializedObject, callback);
            return null;
        }

        private void NoDesignerLoaderControl()
        {
            MessageBox.Show("设计器没有链接到 DesignerLoader 组件。所有存储和加载功能都不可用。");
        }
    }
}