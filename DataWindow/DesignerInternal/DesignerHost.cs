using DataWindow.DesignerInternal.Event;
using DataWindow.DesignerInternal.Interface;
using DataWindow.DesignLayer;
using DataWindow.Services;
using DataWindow.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DataWindow.Core;
using DataWindow.Serialization.Components;

namespace DataWindow.DesignerInternal
{
    public class DesignerHost : IDesignerHost, IServiceContainer, IServiceProvider, IContainer, IDisposable, IDesignEvents, IComponentChangeService, IDesignerEventService, IExtenderProviderService, IExtenderListService, IUIService
    {
        private readonly List<MegaAction> _actions;

        private readonly List<IExtenderProvider> _extenderProviders;

        private readonly Dictionary<IComponent, int> _selectedTab;

        private readonly ServiceContainer _serviceContainer;

        private readonly Dictionary<IComponent, DesignerSite> _sites;

        private readonly List<DesignerTransaction> _transactions;

        private int _actionIndex;

        private bool _active;

        private Control _designedForm;

        private Point _designedFormLocation;

        private bool _designStopped;

        private Control _designSurface;

        private bool _doingAction;

        private Control _rootView;

        internal List<ExtInvokeParam> extInvoke = new List<ExtInvokeParam>();

        private bool inRename;

        private string memberName;

        public bool ShowErrorMessage;

        public DesignerHost()
        {
            DesignedContainers = new SortedList<string, string>();
            Parents = new Hashtable();
            Styles = new Hashtable();
            _active = false;
            _actions = new List<MegaAction>();
            _designStopped = false;
            _doingAction = false;
            _extenderProviders = new List<IExtenderProvider>();
            _selectedTab = new Dictionary<IComponent, int>();
            _serviceContainer = new ServiceContainer();
            _sites = new Dictionary<IComponent, DesignerSite>();
            _transactions = new List<DesignerTransaction>();
            _serviceContainer.AddService(typeof(FormComponents), new FormComponents());
            _serviceContainer.AddService(typeof(IDesignerHost), this);
            _serviceContainer.AddService(typeof(IContainer), this);
            _serviceContainer.AddService(typeof(IComponentChangeService), this);
            var selectionServiceImpl = new ISelectionServiceImpl(this);
            selectionServiceImpl.SelectionChanged += SelectionComponentChanged;
            _serviceContainer.AddService(typeof(ISelectionService), selectionServiceImpl);
            _serviceContainer.AddService(typeof(IDesignerEventService), this);
            _serviceContainer.AddService(typeof(IExtenderProviderService), this);
            _serviceContainer.AddService(typeof(IExtenderListService), this);
            var typeDescriptorFilterServiceImpl = new ITypeDescriptorFilterServiceImpl(this);
            typeDescriptorFilterServiceImpl.FilterAtt += FilterAtt;
            typeDescriptorFilterServiceImpl.FilterEvnts += FilterEvnts;
            typeDescriptorFilterServiceImpl.FilterProps += FilterProps;
            _serviceContainer.AddService(typeof(ITypeDescriptorFilterService), typeDescriptorFilterServiceImpl);
            _serviceContainer.AddService(typeof(INameCreationService), new INameCreationServiceImpl());
            var menuCommandServiceImpl = new IMenuCommandServiceImpl(this);
            menuCommandServiceImpl.AddingVerb += LocalMenuAddingVerb;
            _serviceContainer.AddService(typeof(IMenuCommandService), menuCommandServiceImpl);
            _serviceContainer.AddService(typeof(IUIService), this);
            _serviceContainer.AddService(typeof(IDesignEvents), this);
            _serviceContainer.AddService(typeof(IDesignerOptionService), new IDesignerOptionServiceImpl());
            _serviceContainer.AddService(typeof(DesignerOptionService), new DesignerOptionServiceImpl());
            var serviceInstance = new TypeResolutionService();
            _serviceContainer.AddService(typeof(ITypeResolutionService), serviceInstance);
            _serviceContainer.AddService(typeof(ITypeDiscoveryService), serviceInstance);
            _serviceContainer.AddService(typeof(EventFilter), new EventFilter(this));
        }

        internal Control DesignedForm
        {
            get => _designedForm;
            set
            {
                if (_designedForm != null)
                {
                    var form = _designedForm is Form ? (Form) _designedForm : _designedForm.FindForm();
                    if (form != null) form.Closed -= DesginedFormClosed;
                }

                if (value != null)
                {
                    var form2 = value is Form ? (Form) value : value.FindForm();
                    if (form2 != null) form2.Closed += DesginedFormClosed;
                    _designedFormLocation = value.Location;
                }

                _designedForm = value;
            }
        }

        internal Control DesignContainer { get; set; }

        internal string LogName { get; set; }

        internal Designer Owner { get; set; }

        internal int UndoCount => _actionIndex + 1;

        internal int RedoCount => _actions.Count - _actionIndex - 1;

        public SortedList<string, string> DesignedContainers { get; }

        public Hashtable Parents { get; }

        public bool UseNativeType { get; set; }

        public event ComponentEventHandler ComponentAdded;

        public event ComponentEventHandler ComponentAdding;

        public event ComponentChangingEventHandler ComponentChanging;

        public event ComponentChangedEventHandler ComponentChanged;

        public event ComponentEventHandler ComponentRemoved;

        public event ComponentEventHandler ComponentRemoving;

        public event ComponentRenameEventHandler ComponentRename;

        public void OnComponentChanging(object component, MemberDescriptor member)
        {
            if (!Loading)
            {
                memberName = member != null ? member.Name : null;
                if (ComponentChanging != null)
                {
                    var e = new ComponentChangingEventArgs(component, member);
                    ComponentChanging(this, e);
                }
            }
        }

        public void OnComponentChanged(object component, MemberDescriptor member, object oldValue, object newValue)
        {
            if (!Loading)
            {
                if (ComponentChanged != null)
                {
                    var oldValue2 = memberName != null && member != null && memberName == member.Name ? null : oldValue;
                    var e = new ComponentChangedEventArgs(component, member, oldValue2, newValue);
                    ComponentChanged(this, e);
                }

                if (member != null && member.Name == "Name" && ComponentRename != null && !inRename)
                {
                    inRename = true;
                    ComponentRename(this, new ComponentRenameEventArgs(component, (string) oldValue, (string) newValue));
                    inRename = false;
                }
            }
        }

        public ComponentCollection Components =>
            new ComponentCollection((from s in _sites
                                     select s.Value.Component).ToArray());

        public void Add(IComponent component)
        {
            Add(component, null);
        }

        public void Add(IComponent component, string name)
        {
            if (component != null && !_designStopped && (component.Site == null || component.Site.Container != this) && (AllowDesign == null || AllowDesign(component)))
            {
                if (component is Designer || component is DefaultDesignerLoader)
                {
                    return;
                }


                if (component is Control) Parents[component] = ((Control) component).Parent;
                WriteToLog("添加组件 {0}", name == null ? "" : name);
                var designerTransaction = CreateTransaction("Add component");
                using (designerTransaction)
                {
                    var designerSite = new DesignerSite(this, component);
                    if (name == null || Components[name] != null) name = ((INameCreationService) GetService(typeof(INameCreationService))).CreateName(this, component.GetType());
                    var control = component as Control;
                    if (control != null && control.Name != name) control.Name = name;
                    designerSite.Name = name;
                    component.GetType().ToString().ToUpper();
                    component.Site = designerSite;

                    TabControl tabControl;
                    if ((tabControl = component as TabControl) != null)
                    {
                        var selectedTab = tabControl.SelectedTab;
                        var tabPages = tabControl.TabPages;
                        var num = 0;
                        var enumerator = tabPages.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if ((TabPage) enumerator.Current == selectedTab)
                            {
                                _selectedTab.TryGetValue(component, out num);
                                break;
                            }

                            num++;
                        }
                    }

                    var designer = CreateComponentDesigner(component);
                    if (designer == null)
                    {
                        WriteToLog("不能为组件{0}加载设计器", name == null ? "" : name);
                    }
                    else
                    {
                        var componentAdding = ComponentAdding;
                        if (componentAdding != null) componentAdding(this, new ComponentEventArgs(component));
                        designerSite.Designer = designer;

                        _sites.Add(component, designerSite);
                        try
                        {
                            designer.Initialize(component);
                        }
                        catch (Exception ex)
                        {
                            WriteToLog("初始化设计器时异常: {0}", ex.Message);
                        }

                        var extenderProviderService = (IExtenderProviderService) GetService(typeof(IExtenderProviderService));
                        var extenderProvider = designer as IExtenderProvider;
                        if (extenderProvider != null) extenderProviderService.AddExtenderProvider(extenderProvider);
                        extenderProvider = component as IExtenderProvider;
                        if (extenderProvider != null) extenderProviderService.AddExtenderProvider(extenderProvider);
                        var formComponents = (FormComponents) GetService(typeof(FormComponents));
                        if (!(component is Control)) formComponents.Add(name, component);
                        var componentAdded = ComponentAdded;
                        if (componentAdded != null) componentAdded(this, new ComponentEventArgs(component));
                        designerTransaction.Commit();
                        WriteToLog("组件添加完成");
                        CheckContainedComponents(component);
                    }
                }
            }
        }

        public void Remove(IComponent component)
        {
            Remove(component, true);
        }

        public void Dispose()
        {
            _designedForm = null;
            foreach (var designerSite in _sites.Values)
            {
                designerSite.Component.Site = null;
                designerSite.Designer.Dispose();
            }

            _sites.Clear();
            _serviceContainer.RemoveService(typeof(IDesignerHost));
            _serviceContainer.RemoveService(typeof(IContainer));
            _serviceContainer.RemoveService(typeof(IComponentChangeService));
            _serviceContainer.RemoveService(typeof(ISelectionService));
            _serviceContainer.RemoveService(typeof(IDesignerEventService));
            _serviceContainer.RemoveService(typeof(IExtenderProviderService));
            _serviceContainer.RemoveService(typeof(IExtenderListService));
            _serviceContainer.RemoveService(typeof(ITypeDescriptorFilterService));
            _serviceContainer.RemoveService(typeof(INameCreationService));
            _serviceContainer.RemoveService(typeof(IMenuCommandService));
            _serviceContainer.RemoveService(typeof(IUIService));
            _serviceContainer.RemoveService(typeof(ITypeResolutionService));
            _serviceContainer.RemoveService(typeof(IDesignEvents));
            _serviceContainer.RemoveService(typeof(IDesignerOptionService));
            _serviceContainer.RemoveService(typeof(DesignerOptionService));
            _serviceContainer.RemoveService(typeof(EventFilter));
        }

        public IDesignerHost ActiveDesigner => this;

        public DesignerCollection Designers
        {
            get
            {
                return new DesignerCollection(new IDesignerHost[]
                {
                    this
                });
            }
        }

        public event ActiveDesignerEventHandler ActiveDesignerChanged;

        public event DesignerEventHandler DesignerDisposed;

        public event DesignerEventHandler DesignerCreated;

        public event EventHandler SelectionChanged;

        public IContainer Container => this;

        public string TransactionDescription
        {
            get
            {
                if (InTransaction) return _transactions[_transactions.Count - 1].Description;
                return null;
            }
        }

        public string RootComponentClassName => RootComponent.GetType().ToString();

        public bool Loading { get; set; }

        public bool InTransaction => _transactions.Count > 0;

        public IComponent RootComponent => _designSurface;

        public event EventHandler Activated;

        public event EventHandler Deactivated;

        public event EventHandler LoadComplete;

        public event DesignerTransactionCloseEventHandler TransactionClosed;

        public event DesignerTransactionCloseEventHandler TransactionClosing;

        public event EventHandler TransactionOpened;

        public event EventHandler TransactionOpening;

        public void Activate()
        {
            _active = true;
            DesignedForm.Focus();
        }

        public DesignerTransaction CreateTransaction()
        {
            return CreateTransaction(null);
        }

        public DesignerTransaction CreateTransaction(string desc)
        {
            if (!Loading && _transactions.Count == 0 && TransactionOpening != null) TransactionOpening(this, EventArgs.Empty);
            DesignerTransaction designerTransaction;
            if (desc == null)
                designerTransaction = new DesignerTransactionImpl(this);
            else
                designerTransaction = new DesignerTransactionImpl(this, desc);
            _transactions.Add(designerTransaction);
            if (!Loading && _transactions.Count == 1)
            {
                var transactionOpened = TransactionOpened;
                if (transactionOpened != null) transactionOpened(this, EventArgs.Empty);
                if (!_doingAction)
                {
                    while (RedoCount > 0) _actions.RemoveAt(_actions.Count - 1);
                    var megaAction = new MegaAction(this);
                    _actions.Add(megaAction);
                    _actionIndex = _actions.Count - 1;
                    megaAction.StartActions();
                }
            }

            return designerTransaction;
        }

        public IComponent CreateComponent(Type componentType)
        {
            return CreateComponent(componentType, null);
        }

        public IComponent CreateComponent(Type componentType, string name)
        {
            IComponent component = null;
            WriteToLog("创建组件 ", componentType.ToString());

            if (!string.IsNullOrWhiteSpace(name))
            {
                component = Components.Cast<IComponent>().FirstOrDefault(s => s.Site.Name.Equals(name));
                if (component != null)
                {
                    return component;
                }
            }

            try
            {
                component = (IComponent) Activator.CreateInstance(componentType);
            }
            catch (Exception ex)
            {
                if (ShowErrorMessage) MessageBox.Show("创建组件(" + componentType + ")异常，" + ex.Message);
                WriteToLog("创建组件失败: {0}", ex.Message);
                return null;
            }

            if (name == null)
            {
                INameCreationService nameCreationService;
                if ((nameCreationService = GetService(typeof(INameCreationService)) as INameCreationService) != null)
                    name = nameCreationService.CreateName(this, componentType);
                else
                    name = componentType.Name;
            }

            if (componentType == typeof(SplitContainer))
            {
                Add((component as SplitContainer).Panel1, null);
                Add((component as SplitContainer).Panel2, null);
            }

            Add(component, name);
            return component;
        }

        public bool HasComponent(string name)
        {
            return Components.Cast<IComponent>().Any(s => s.Site != null && s.Site.Name.Equals(name));
        }

        public void DestroyComponent(IComponent component)
        {
            if (component.Site != null && component.Site.Container == this)
            {
                Remove(component);
                component.Dispose();
            }
        }

        public IDesigner GetDesigner(IComponent component)
        {
            if ((component != null ? component.Site : null) == null || component.Site.Container != this) return null;
            return (component.Site as DesignerSite).Designer;
        }

        public Type GetType(string typeName)
        {
            ITypeResolutionService typeResolutionService;
            if ((typeResolutionService = GetService(typeof(ITypeResolutionService)) as ITypeResolutionService) != null) return typeResolutionService.GetType(typeName);
            return Type.GetType(typeName);
        }

        public object GetService(Type serviceType)
        {
            return _serviceContainer.GetService(serviceType);
        }

        public void AddService(Type serviceType, object serviceInstance)
        {
            _serviceContainer.AddService(serviceType, serviceInstance);
        }

        public void AddService(Type serviceType, object serviceInstance, bool promote)
        {
            _serviceContainer.AddService(serviceType, serviceInstance, promote);
        }

        public void AddService(Type serviceType, ServiceCreatorCallback callback)
        {
            _serviceContainer.AddService(serviceType, callback);
        }

        public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
        {
            _serviceContainer.AddService(serviceType, callback);
        }

        public void RemoveService(Type serviceType)
        {
            _serviceContainer.RemoveService(serviceType);
        }

        public void RemoveService(Type serviceType, bool promote)
        {
            _serviceContainer.RemoveService(serviceType, promote);
        }

        public event EventHandler<FilterEventArgs> FilterAttributes;

        public event EventHandler<FilterEventArgs> FilterEvents;

        public event EventHandler<FilterEventArgs> FilterProperties;

        public event AddingVerbHandler AddingVerb;

        public event AllowDesignHandler AllowDesign;

        public IExtenderProvider[] GetExtenderProviders()
        {
            return _extenderProviders.ToArray();
        }

        public void AddExtenderProvider(IExtenderProvider provider)
        {
            if (!_extenderProviders.Contains(provider)) _extenderProviders.Add(provider);
        }

        public void RemoveExtenderProvider(IExtenderProvider provider)
        {
            if (_extenderProviders.Contains(provider)) _extenderProviders.Remove(provider);
        }

        public IDictionary Styles { get; }

        public bool CanShowComponentEditor(object component)
        {
            return false;
        }

        public IWin32Window GetDialogOwnerWindow()
        {
            return RootComponent as IWin32Window;
        }

        public void SetUIDirty()
        {
            Owner.SetDirty();
        }

        public bool ShowComponentEditor(object component, IWin32Window parent)
        {
            return false;
        }

        public DialogResult ShowDialog(Form form)
        {
            DialogResult result;
            try
            {
                result = form.ShowDialog(GetDialogOwnerWindow());
            }
            catch
            {
                result = DialogResult.Cancel;
            }

            return result;
        }

        public void ShowError(Exception ex, string message)
        {
            MessageBox.Show(GetDialogOwnerWindow(), ex.Message, message);
        }

        void IUIService.ShowError(Exception ex)
        {
            MessageBox.Show(GetDialogOwnerWindow(), ex.Message);
        }

        void IUIService.ShowError(string message)
        {
            MessageBox.Show(GetDialogOwnerWindow(), message);
        }

        void IUIService.ShowMessage(string message, string caption)
        {
            MessageBox.Show(GetDialogOwnerWindow(), message, caption);
        }

        void IUIService.ShowMessage(string message)
        {
            MessageBox.Show(GetDialogOwnerWindow(), message);
        }

        public DialogResult ShowMessage(string message, string caption, MessageBoxButtons buttons)
        {
            return MessageBox.Show(GetDialogOwnerWindow(), message, caption, buttons);
        }

        public bool ShowToolWindow(Guid toolWindow)
        {
            return false;
        }

        private void WriteToLog(string str, params object[] args)
        {
            if (LogName != null)
                using (var streamWriter = File.AppendText(LogName))
                {
                    streamWriter.WriteLine(args != null && args.Length != 0 ? string.Format(str, args) : str);
                }
        }

        private void DesginedFormClosed(object sendr, EventArgs a)
        {
            if (_active) StopDesign();
            DesignedForm = null;
        }

        private void SelectionComponentChanged(object sender, EventArgs e)
        {
            if (!Loading)
            {
                var selectionChanged = SelectionChanged;
                if (selectionChanged == null) return;
                selectionChanged(this, EventArgs.Empty);
            }
        }

        internal bool Redo()
        {
            if (_actionIndex < _actions.Count - 1)
            {
                _actionIndex++;
                _doingAction = true;
                _actions[_actionIndex].Redo();
                _doingAction = false;
                return true;
            }

            return false;
        }

        internal bool Undo()
        {
            if (_actionIndex >= 0 && _actionIndex < _actions.Count)
            {
                _doingAction = true;
                _actions[_actionIndex].Undo();
                _doingAction = false;
                _actionIndex--;
                return true;
            }

            return false;
        }

        internal void ResetUndo()
        {
            _actions.Clear();
            _actionIndex = -1;
        }

        internal void StartDesign()
        {
            if (_designedForm == null) return;
            Loading = true;
            _designStopped = false;
            Styles.Clear();
            Styles.Add("HighlightColor", SystemColors.HighlightText);
            Styles.Add("DialogFont", Control.DefaultFont);
            if (DesignContainer == null)
            {
                _designSurface = new DesignSurface();
                ((DesignSurface) _designSurface).DesignedControl = _designedForm;
                _designSurface.Name = "DesignSurface1";
                _designSurface.Size = _designedForm.ClientSize;
            }
            else
            {
                _designSurface = CreateDesignSurface(_designedForm.GetType());
                _designSurface.Parent = DesignContainer;
            }

            _designSurface.Controls.Clear();
            Add(_designSurface, _designSurface.Name);
            if (DesignContainer != null) _designedForm.CopyPropertiesTo(_designSurface);
            var rootDesigner = (IRootDesigner) GetDesigner(_designSurface);
            _rootView = (Control) rootDesigner.GetView(ViewTechnology.Default);
            _rootView.Dock = DockStyle.Fill;
            var controls = _designedForm.Controls.ToArray();
            _designSurface.Location = new Point(8, 8);
            _rootView.Size = DesignContainer == null ? _designedForm.ClientSize : DesignContainer.ClientSize;
            AddingControls(controls, _designSurface);
            if (DesignContainer == null)
                _designedForm.Controls.Add(_rootView);
            else
                DesignContainer.Controls.Add(_rootView);
            Activate();
            Loading = false;
            ResetUndo();
            Application.AddMessageFilter((EventFilter) GetService(typeof(EventFilter)));
        }

        internal void StopDesign()
        {
            if (_designedForm != null)
            {
                Loading = true;
                ((ISelectionService) _serviceContainer.GetService(typeof(ISelectionService))).SetSelectedComponents(null);
                Application.RemoveMessageFilter((EventFilter) GetService(typeof(EventFilter)));
                _actions.Clear();
                _transactions.Clear();
                _designStopped = true;
                var controls = _designSurface.Controls.ToArray();
                RestoreControls(controls, _designedForm);
                if (_designSurface.GetType().IsAssignableFrom(_designedForm.GetType()))
                {
                    var site = new DesignerSite(this, _designedForm);
                    _designedForm.Site = site;
                    AddExtProviders(_designedForm, _designSurface);
                    _designedForm.Site = null;
                }

                _selectedTab.Clear();
                if (_sites.Count != 0)
                    foreach (var component in _sites.Keys.ToArray())
                        Remove(component, true);
                if (DesignContainer == null)
                {
                    _designedForm.Controls.Remove(_rootView);
                    ((DesignSurface) _designSurface).DesignedControl = null;
                }
                else
                {
                    DesignContainer.Controls.Remove(_rootView);
                    _designSurface.CopyPropertiesTo(_designedForm);
                }

                _rootView.Dispose();
                _rootView = null;
                DestroyComponent(_designSurface);
                _designSurface = null;
                Deactivate();
                UpdateExtProviders();
                _designedForm.Location = _designedFormLocation;
                Loading = false;
            }
        }

        internal void EndLoad()
        {
            if (DesignContainer != null)
            {
                _designedForm.Site = new DesignerSite(this, _designedForm);
                extInvoke.Clear();
                AddExtProviders(_designSurface, _designedForm);
                UpdateExtProviders();
                _designedForm.Site = null;
            }

            var loadComplete = LoadComplete;
            loadComplete?.Invoke(this, EventArgs.Empty);
        }

        internal Control CreateDesignSurface(Type rootType)
        {
            try
            {
                var obj = Activator.CreateInstance(rootType);
                Form form;
                if ((form = obj as Form) != null) form.TopLevel = false;
                return (Control) obj;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //if (UseNativeType)
            //{
            //    var obj = Activator.CreateInstance(rootType);
            //    Form form;
            //    if ((form = obj as Form) != null) form.TopLevel = false;
            //    return (Control) obj;
            //}

            if (typeof(Form).IsAssignableFrom(rootType))
            {
                var form2 = Activator.CreateInstance(typeof(Form)) as Form;
                form2.TopLevel = false;
                return form2;
            }

            return (Control) Activator.CreateInstance(typeof(UserControl));
        }

        private void AddTableLayout(TableLayoutPanel panel, Control parent)
        {
            var array = MovePanelControls(panel);
            AddingControl(panel, parent);
            panel.Parent = parent;
            foreach (var panelControl in array)
            {
                if (panelControl.Control is TableLayoutPanel)
                    AddTableLayout(panelControl.Control as TableLayoutPanel, null);
                else
                    AddingControl(panelControl.Control, null);
                panel.Controls.Add(panelControl.Control, panelControl.Column, panelControl.Row);
            }
        }

        private void RestoreTableLayout(TableLayoutPanel panel, Control parent)
        {
            var array = MovePanelControls(panel);
            foreach (var panelControl in array)
            {
                TableLayoutPanel panel2;
                if ((panel2 = panelControl.Control as TableLayoutPanel) != null)
                    RestoreTableLayout(panel2, null);
                else
                    RestoreControl(panelControl.Control, null);
            }

            RestoreControl(panel, parent);
            foreach (var panelControl2 in array) panel.Controls.Add(panelControl2.Control, panelControl2.Column, panelControl2.Row);
        }

        private PanelControl[] MovePanelControls(TableLayoutPanel panel)
        {
            var array = new PanelControl[panel.Controls.Count];
            var num = 0;
            foreach (var obj in panel.Controls)
            {
                var control = (Control) obj;
                var panelControl = default(PanelControl);
                panelControl.Control = control;
                panelControl.Row = panel.GetRow(control);
                panelControl.Column = panel.GetColumn(control);
                array[num++] = panelControl;
            }

            panel.Controls.Clear();
            return array;
        }

        private void AddingControl(Control control, Control parent)
        {
            try
            {
                if (control.Site == null)
                {
                    control.Parent = parent;
                    control.Parent = null;
                    Add(control, control.Name);

                    var property = control.GetProperty<string>("Text");
                    ComponentDesigner componentDesigner;
                    if ((componentDesigner = GetDesigner(control) as ComponentDesigner) != null)
                    {
                        if (CanInitializeExisting(control))
                            componentDesigner.InitializeExistingComponent(new Dictionary<object, object>());
                        else
                            componentDesigner.InitializeNewComponent(new Dictionary<object, object>());
                    }

                    control.SetProperty("Text", property);
                    control.Parent = parent;
                }
            }
            catch (Exception ex)
            {
                WriteToLog("AddingControl 错误：{0}", ex.Message);
            }

            if (control.Controls.Count != 0 && GetDesigner(control) is ParentControlDesigner)
            {
                var controls = control.Controls.ToArray();
                AddingControls(controls, control);
            }
        }

        private void AddingControls(Array controls, Control parent)
        {
            foreach (var obj in controls)
            {
                var control = (Control) obj;
                TableLayoutPanel panel;
                if ((panel = control as TableLayoutPanel) != null)
                    AddTableLayout(panel, parent);
                else
                    AddingControl(control, parent);
            }
        }

        private bool CanInitializeExisting(object control)
        {
            var type = control.GetType();
            var fullName = type.FullName;
            return typeof(Panel).IsAssignableFrom(type) || typeof(Button).IsAssignableFrom(type) || control is TabControl || control is DataGridView || fullName.IndexOf("DevExpress.XtraTab.XtraTabControl") >= 0;
        }

        private void AddExtProviders(Control dest, Control src)
        {
            var properties = TypeDescriptor.GetProperties(src);
            var properties2 = TypeDescriptor.GetProperties(dest);
            foreach (var obj in properties)
            {
                var propertyDescriptor = (PropertyDescriptor) obj;
                var extenderProvidedPropertyAttribute = propertyDescriptor.Attributes[typeof(ExtenderProvidedPropertyAttribute)] as ExtenderProvidedPropertyAttribute;
                if (extenderProvidedPropertyAttribute != null && extenderProvidedPropertyAttribute.Provider != null)
                {
                    var value = propertyDescriptor.GetValue(src);
                    if (value != null && properties2[propertyDescriptor.Name] != null)
                    {
                        object provider = extenderProvidedPropertyAttribute.Provider;
                        var method = provider.GetType().GetMethod("Set" + propertyDescriptor.Name);
                        if (method != null)
                        {
                            var extInvokeParam = new ExtInvokeParam();
                            extInvokeParam.MethodInfo = method;
                            extInvokeParam.Provider = provider;
                            extInvokeParam.Params = new[]
                            {
                                dest,
                                value
                            };
                            if (dest != src)
                                try
                                {
                                    var array = new object[2];
                                    array[0] = src;
                                    method.Invoke(provider, array);
                                }
                                catch (Exception)
                                {
                                }

                            extInvoke.Add(extInvokeParam);
                        }
                    }
                }
            }
        }

        private void UpdateExtProviders()
        {
            foreach (var extInvokeParam in extInvoke)
            {
                try
                {
                    MethodBase methodInfo = extInvokeParam.MethodInfo;
                    var provider = extInvokeParam.Provider;
                    var array = new object[2];
                    array[0] = extInvokeParam.Params[0];
                    methodInfo.Invoke(provider, array);
                }
                catch (Exception)
                {
                }

                try
                {
                    extInvokeParam.MethodInfo.Invoke(extInvokeParam.Provider, extInvokeParam.Params);
                }
                catch (Exception)
                {
                }
            }

            extInvoke.Clear();
        }

        private void RestoreControl(Control control, Control parent)
        {
            control.Visible = control.IsVisiable();
            AddExtProviders(control, control);
            if (control.Controls.Count != 0 && GetDesigner(control) is ParentControlDesigner)
            {
                var controls = control.Controls.ToArray();
                RestoreControls(controls, control);
            }

            if (control.Site != null) control.Name = control.Site.Name;
            Remove(control);
            try
            {
                control.Parent = parent;
                if (!control.IsEnabled()) control.Enabled = false;
                var tabControl = control as TabControl;
                if (tabControl != null)
                {
                    int num;
                    _selectedTab.TryGetValue(tabControl, out num);
                    var tabPages = tabControl.TabPages;
                    if (num >= tabPages.Count) num = 0;
                    tabControl.SelectedTab = tabPages[num];
                }
            }
            catch (Exception)
            {
            }
        }

        private void RestoreControls(Array controls, Control parent)
        {
            foreach (var obj in controls)
            {
                var control = (Control) obj;
                if (control != null)
                {
                    if (control is TableLayoutPanel)
                        RestoreTableLayout(control as TableLayoutPanel, parent);
                    else
                        RestoreControl(control, parent);
                }
            }
        }

        private void FilterAtt(object sender, FilterEventArgs e)
        {
            var filterAttributes = FilterAttributes;
            if (filterAttributes == null) return;
            filterAttributes(sender, e);
        }

        private void FilterEvnts(object sender, FilterEventArgs e)
        {
            var filterEvents = FilterEvents;
            if (filterEvents == null) return;
            filterEvents(sender, e);
        }

        private void FilterProps(object sender, FilterEventArgs e)
        {
            var filterProperties = FilterProperties;
            if (filterProperties == null) return;
            filterProperties(sender, e);
        }

        private bool LocalMenuAddingVerb(IComponent primarySelection, DesignerVerb verb)
        {
            return AddingVerb == null || AddingVerb(primarySelection, verb);
        }

        private void Deactivate()
        {
            _active = false;
        }

        internal void ClosingTransaction(DesignerTransaction transaction, bool commiting)
        {
            if (!Loading && _transactions.Count == 1)
            {
                var transactionClosing = TransactionClosing;
                if (transactionClosing != null) transactionClosing(this, new DesignerTransactionCloseEventArgs(commiting, true));
                var transactionClosed = TransactionClosed;
                if (transactionClosed != null) transactionClosed(this, new DesignerTransactionCloseEventArgs(commiting, true));
                if (!_doingAction)
                {
                    var megaAction = _actions[_actions.Count - 1];
                    megaAction.StopActions();
                    if (!commiting)
                    {
                        _doingAction = true;
                        megaAction.Undo();
                        _doingAction = false;
                        _actions.RemoveAt(_actions.Count - 1);
                    }
                }
            }

            _transactions.Remove(transaction);
        }

        internal void TransactionCommiting(DesignerTransaction transaction)
        {
            ClosingTransaction(transaction, true);
        }

        internal void TransactionCanceling(DesignerTransaction transaction)
        {
            ClosingTransaction(transaction, false);
        }

        private void Remove(IComponent component, bool fireEvents)
        {
            if (component.Site != null && component.Site.Container == this)
            {
                var designerTransaction = CreateTransaction("Remove component");
                using (designerTransaction)
                {
                    if (fireEvents && component != _rootView && ComponentRemoving != null) ComponentRemoving(this, new ComponentEventArgs(component));
                    if (!_designStopped) ((FormComponents) GetService(typeof(FormComponents))).Remove(component.Site.Name);
                    var extenderProviderService = (IExtenderProviderService) GetService(typeof(IExtenderProviderService));
                    var extenderProvider = component as IExtenderProvider;
                    if (extenderProvider != null) extenderProviderService.RemoveExtenderProvider(extenderProvider);
                    _sites.Remove(component);
                    var designer = ((DesignerSite) component.Site).Designer;
                    extenderProvider = designer as IExtenderProvider;
                    if (extenderProvider != null) extenderProviderService.RemoveExtenderProvider(extenderProvider);
                    if (designer != null)
                    {
                        try
                        { 
                            designer.Dispose();
                        }
                        catch
                        {
                        }
                    }

                    if (fireEvents && component != _rootView)
                    {
                        var e = new ComponentEventArgs(component);
                        if (ComponentRemoved != null) ComponentRemoved(this, e);
                        var selectionServiceImpl = GetService(typeof(ISelectionService)) as ISelectionServiceImpl;
                        if (selectionServiceImpl != null) selectionServiceImpl.OnComponentRemoved(this, e);
                    }

                    component.Site = null;
                    designerTransaction.Commit();
                }
            }
        }

        private IDesigner CreateComponentDesigner(IComponent component)
        {
            IDesigner result;
            if (_sites.Count == 0)
            {
                result = new RootDesigner();
            }
            else
            {
                ((ITypeResolutionService) GetService(typeof(ITypeResolutionService))).ReferenceAssembly(component.GetType().Assembly.GetName());
                var designer = TypeDescriptor.CreateDesigner(component, typeof(IDesigner));
                if (designer != null)
                {
                    result = designer;
                }
                else
                {
                    var designerAttribute = FindDesignerAttribute(component);
                    if (designerAttribute == null)
                    {
                        if (ShowErrorMessage) MessageBox.Show("未能加载组件");
                        result = null;
                    }
                    else
                    {
                        result = InstantinateDesigner(designerAttribute);
                    }
                }
            }

            return result;
        }

        private DesignerAttribute FindDesignerAttribute(IComponent component)
        {
            foreach (var obj in TypeDescriptor.GetAttributes(component))
            {
                var attribute = (Attribute) obj;
                if (attribute.GetType() == typeof(DesignerAttribute) && attribute.TypeId.ToString().IndexOf("IDesigner") >= 0) return (DesignerAttribute) attribute;
            }

            return null;
        }

        private IDesigner InstantinateDesigner(DesignerAttribute da)
        {
            var text = ",";
            var array = da.DesignerTypeName.Split(text.ToCharArray());
            IDesigner result;
            if (array.Length <= 1)
            {
                result = null;
            }
            else
            {
                var assembly = Assembly.Load(array[1].Trim());
                IDesigner designer = null;
                if (assembly == null)
                {
                    var text2 = "未找到程序集 " + array[1].Trim();
                    WriteToLog(text2);
                    if (ShowErrorMessage) MessageBox.Show(text2);
                }
                else
                {
                    designer = (IDesigner) assembly.CreateInstance(array[0].Trim());
                    WriteToLog("从程序集 {0} 创建对象 {1}", array[1].Trim(), array[0].Trim());
                }

                result = designer;
            }

            return result;
        }

        private void CheckContainedComponents(IComponent control)
        {
            var type = control.GetType();
            PropertyInfo propertyInfo = null;
            foreach (var text in DesignedContainers.Keys)
                if (type.FullName.IndexOf(text) >= 0)
                {
                    propertyInfo = type.GetProperty(DesignedContainers[text]);
                    break;
                }

            if (propertyInfo == null)
            {
                DataSet ds;
                if ((ds = control as DataSet) != null) AddDataSetComponents(ds);
            }
            else
            {
                var value = propertyInfo.GetValue(control, null);
                if (value != null)
                {
                    IList list;
                    if ((list = value as IList) != null)
                    {
                        var enumerator2 = list.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            IComponent component;
                            if ((component = enumerator2.Current as IComponent) != null)
                            {
                                Add(component, null);
                                AddChildControls(component);
                            }
                        }

                        return;
                    }

                    IComponent component2;
                    if ((component2 = value as IComponent) != null) Add(component2, null);
                }
            }
        }

        private void AddDataSetComponents(DataSet ds)
        {
            foreach (var obj in ds.Tables)
            {
                var dataTable = (DataTable) obj;
                Add(dataTable, dataTable.TableName);
                foreach (var obj2 in dataTable.Columns)
                {
                    var dataColumn = (DataColumn) obj2;
                    Add(dataColumn, dataColumn.ColumnName);
                }
            }
        }

        private void AddChildControls(IComponent comp)
        {
            Control control;
            if ((control = comp as Control) != null && control.Controls.Count > 0)
            {
                var designer = GetDesigner(comp);
                if (typeof(ParentControlDesigner).IsAssignableFrom(designer.GetType()))
                    foreach (var obj in control.Controls)
                    {
                        var component = (Control) obj;
                        Add(component, null);
                    }
            }
        }

        private struct PanelControl
        {
            public Control Control;

            public int Row;

            public int Column;
        }

        #region DataWindow

        #endregion
    }
}