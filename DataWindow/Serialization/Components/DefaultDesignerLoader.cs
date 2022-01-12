using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using DataWindow.Core;
using DataWindow.DesignerInternal;
using DataWindow.Utility;

namespace DataWindow.Serialization.Components
{
    public class DefaultDesignerLoader : Component, IDesignerLoader, IDesignerSerializationService
    {
        private const string ATTR_COLLECTION = "collection";

        private const string ATTR_CONTENT = "content";

        private const string ATTR_CONTROL = "control";

        private const string ATTR_MODE = "mode";

        private const string ATTR_MODE_BINARY = "binary";

        private const string ATTR_MODE_CONSTRUCTOR = "constructor";

        private const string ATTR_MODE_INSTANCE_DESCRIPTOR = "instance_descriptor";

        private const string ATTR_MODE_REFERENCE = "reference";

        private const string ATTR_NAME = "name";

        private const string ATTR_EVENT_NAME = "event_name";

        private const string ATTR_NULL = "null";

        private const string ATTR_PROVIDER = "provider";

        private const string ATTR_PROP_TYPE = "prop_type";

        private const string ATTR_TYPE = "type";

        private const string ATTR_VERSION = "version";

        private const string NODE_DATA = "Data";

        private const string NODE_EVENT = "Event";

        private const string NODE_OBJECT = "Object";

        private const string NODE_OBJECT_COLLECTION = "ObjectCollection";

        private const string NODE_PARAM = "Param";

        private const string DESIGNED_FORM = "DesignedForm";

        private const string VERSION_ID = "1.0";

        private readonly Dictionary<Control, List<Control>> addedControls = new Dictionary<Control, List<Control>>();

        private readonly ArrayList drillList = new ArrayList();

        private readonly IEventBindingServiceImpl eventBinding;

        private readonly Dictionary<string, List<Extender>> extenders = new Dictionary<string, List<Extender>>();

        private readonly ArrayList initedObjects = new ArrayList();

        private readonly Hashtable lazyList = new Hashtable();

        private readonly Dictionary<string, IComponent> loadedComponents = new Dictionary<string, IComponent>();

        private readonly ReferencedCollection referencedComponents = new ReferencedCollection();

        private string currentVersion = "";

        private Control designedForm;

        private IDesignerHost designerHost;

        private string loadedObjectName;

        private Hashtable pointers;

        private bool versionWrited;

        public DefaultDesignerLoader()
        {
            DrillDown += DrillDownDefault;
            eventBinding = new IEventBindingServiceImpl();
            Assembly.GetAssembly(typeof(PropertyDescriptor));
        }

        public event DrillDownHandler DrillDown
        {
            add => drillList.Add(value);
            remove => drillList.Remove(value);
        }

        public event ComponentEventHandler ComponentLoaded;

        [DefaultValue(LoadModes.Default)]
        [Description("获取或设置设计器的加载模式")]
        public LoadModes LoadMode { get; set; }

        [DefaultValue(StoreModes.Default)]
        [Description("获取或设置设计器的序列化模式")]
        public StoreModes StoreMode { get; set; }

        [DefaultValue(false)]
        [Description("启用或禁用显示错误消息框")]
        public bool ShowErrorMessage { get; set; }

        [DefaultValue(null)]
        public IDesignerHost DesignerHost
        {
            get => designerHost;
            set
            {
                if (designerHost != null)
                {
                    designerHost.RemoveService(typeof(IDesignerSerializationService));
                    designerHost.RemoveService(typeof(IEventBindingService));
                    eventBinding.ServiceProvider = null;
                    designerHost.RemoveService(typeof(ComponentSerializationService));
                }

                designerHost = value;
                if (designerHost != null)
                {
                    designerHost.AddService(typeof(IEventBindingService), eventBinding);
                    eventBinding.ServiceProvider = designerHost;
                    designerHost.AddService(typeof(IDesignerSerializationService), this);
                    designerHost.AddService(typeof(ComponentSerializationService), new ComponentSerializationServiceImpl(this));
                }
            }
        }

        [DefaultValue(null)]
        public FormComponents Components { get; set; }

        [DefaultValue(null)]
        public string LogFile { get; set; }

        public void Load(Control parent, IReader reader, Dictionary<string, IComponent> components, bool ignoreParent)
        {
            if (parent != null)
            {
                designedForm = parent;
                initedObjects.Clear();
                Control rootControl = null;
                if (designerHost != null)
                {
                    rootControl = designerHost.RootComponent as Control;
                    WriteToLog("加载设计时");
                }

                reader.Read();
                reader.Attributes.TryGetValue("version", out currentVersion);
                if (reader.Name == "ObjectCollection") reader.Read();
                parent = SubstRoot(rootControl, parent);
                if (!ignoreParent) LoadProperties(parent, reader);
                loadedComponents.Clear();
                referencedComponents.Clear();
                lazyList.Clear();
                PrepareParent(reader, ignoreParent, parent);
                var stack = new Stack<Control>();
                stack.Push(parent);
                LoadControls(stack, reader);
                AddControls();
                LoadComponents(reader, components);
                SetReferences();
                SetExtendProviders();
                InvokeAddRange();
                foreach (var obj in initedObjects) ((ISupportInitialize) obj).EndInit();
                AddBindings();
                if (designerHost == null)
                {
                    var form = parent.FindForm();
                    eventBinding.BindEvents(form ?? parent);
                }
                else
                {
                    eventBinding.RefreshEventData();
                }

                initedObjects.Clear();
            }
        }

        public void Store(IComponent[] _components, IWriter writer)
        {
            if (_components.Length != 0)
            {
                BeforeWriting();
                var list = new List<IComponent>(_components);
                if (Components != null)
                    foreach (var keyValuePair in Components.Components)
                        if (!list.Contains(keyValuePair.Value))
                            list.Add(keyValuePair.Value);
                if (_components[0] is Control) designedForm = (Control) _components[0];
                if (list.Count > 1)
                {
                    var hashtable = new Hashtable();
                    hashtable["version"] = 1.ToString();
                    versionWrited = true;
                    writer.WriteStartElement("ObjectCollection", hashtable);
                }

                Control rootControl = null;
                if (designerHost != null) rootControl = designerHost.RootComponent as Control;
                var list2 = new List<Control>();
                using (var enumerator2 = list.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        Control item;
                        if ((item = enumerator2.Current as Control) != null) list2.Add(item);
                    }
                }

                foreach (var component in list)
                    if (component != null && !(component as Control).HaveParentInList(list2))
                        StoreControl(component, rootControl, writer);
                if (list.Count > 1) writer.WriteEndElement("ObjectCollection");
                writer.Flush();
                AfterWriting();
            }
        }

        public void SetEventSource(object eventSource)
        {
            eventBinding.Source = eventSource;
        }

        public void BindEvents(object eventSource)
        {
            eventBinding.BindEvents(eventSource);
        }

        public void UnbindEvents(object eventSource)
        {
            eventBinding.UnbindEvents(eventSource);
        }

        public void RefreshEventData()
        {
            eventBinding.RefreshEventData();
        }

        public object Serialize(ICollection objects)
        {
            string result;
            using (var stringWriter = new StringWriter())
            {
                IWriter writer = new TextFormWriter(stringWriter);
                BeforeWriting();
                foreach (var obj in objects)
                {
                    string[] array;
                    if ((array = obj as string[]) != null)
                    {
                        var hashtable = new Hashtable();
                        hashtable["name"] = "";
                        hashtable["type"] = array.GetType().AssemblyQualifiedName;
                        hashtable["Length"] = array.Length;
                        if (!versionWrited)
                        {
                            hashtable["version"] = "1.0";
                            versionWrited = true;
                        }

                        writer.WriteStartElement("Object", hashtable);
                        foreach (var value in array) writer.WriteValue("Data", value, null);
                        writer.WriteEndElement("Object");
                    }
                    else
                    {
                        var component = obj as IComponent;
                        if (component == null)
                        {
                            Hashtable hashtable2;
                            if ((hashtable2 = obj as Hashtable) != null) pointers = (Hashtable) hashtable2.Clone();
                        }
                        else
                        {
                            var site = component.Site;
                            if (designerHost == null && site != null) designerHost = (IDesignerHost) site.GetService(typeof(IDesignerHost));
                            component.Site = null;
                            StoreControl(component, null, writer);
                            component.Site = site;
                        }
                    }
                }

                result = stringWriter.ToString();
            }

            AfterWriting();
            return result;
        }

        public ICollection Deserialize(object serializationData)
        {
            var text = serializationData as string;
            ICollection result;
            if (text == null)
            {
                result = null;
            }
            else
            {
                ICollection collection = null;
                using (var stringReader = new StringReader(text))
                {
                    IReader reader = new TextFormReader(stringReader);
                    collection = Deserialize(reader, true);
                }

                pointers = null;
                if (designerHost != null) ((ISelectionService) designerHost.GetService(typeof(ISelectionService))).SetSelectedComponents(collection, SelectionTypes.Replace);
                result = collection;
            }

            return result;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        internal ICollection Deserialize(IReader reader, bool setParent)
        {
            var list = new List<IComponent>();
            reader.Read();
            reader.Attributes.TryGetValue("version", out currentVersion);
            var control = designerHost.RootComponent as Control;
            var point = control.PointToClient(Cursor.Position);
            do
            {
                if (reader.State == ReaderState.StartElement)
                {
                    var obj = LoadObject(reader, null, false);
                    IComponent item;
                    if ((item = obj as IComponent) != null)
                    {
                        list.Add(item);
                        Control control2;
                        if (designerHost != null && (control2 = obj as Control) != null)
                        {
                            if (pointers != null && pointers.ContainsKey(loadedObjectName))
                            {
                                var point2 = (Point) pointers[loadedObjectName];
                                var location = new Point(point.X - point2.X, point.Y - point2.Y);
                                control2.Location = location;
                            }

                            control2.Parent = setParent ? control : null;
                            control2.Text = control2.Name;
                        }
                    }
                }
            } while (reader.Read());

            return list;
        }

        public void Load(Control parent, IReader reader)
        {
            Load(parent, reader, null, false);
        }

        public void Load(Control parent, string layout)
        {
            if (layout.Length == 0) throw new ArgumentException();
            using (var xmlFormReader = new XmlFormReader(new MemoryStream(Encoding.UTF8.GetBytes(layout))))
            {
                Load(parent, xmlFormReader, null, false);
            }
        }

        public void LoadRoot(Control parent, IReader reader)
        {
            Load(parent, reader, null, true);
        }

        public void LoadRoot(Control parent, string layout)
        {
            if (layout.Length == 0) throw new ArgumentException();
            using (var xmlFormReader = new XmlFormReader(new MemoryStream(Encoding.UTF8.GetBytes(layout))))
            {
                Load(parent, xmlFormReader, null, true);
            }
        }

        private void LoadControls(Stack<Control> owners, IReader reader)
        {
            for (;;)
            {
                if (reader.State == ReaderState.StartElement)
                {
                    var control = owners.Peek();
                    if (!addedControls.ContainsKey(control)) addedControls[control] = new List<Control>();
                    var flag = false;
                    Control control2;
                    if ((control2 = GetOrCreateObject(control, reader, ref flag) as Control) != null)
                    {
                        LoadProperties(control2, reader);
                        if (!flag)
                        {
                            addedControls[control].Add(control2);
                            TableLayoutPanel tableLayoutPanel;
                            if ((tableLayoutPanel = control as TableLayoutPanel) != null)
                            {
                                tableLayoutPanel.ColumnCount = tableLayoutPanel.ColumnStyles.Count;
                                tableLayoutPanel.RowCount = tableLayoutPanel.RowStyles.Count;
                            }
                        }

                        owners.Push(control2);
                    }
                    else if (!SkipControl(reader, true))
                    {
                        break;
                    }
                }
                else if (reader.State == ReaderState.EndElement)
                {
                    owners.Pop();
                    if (!reader.Read()) return;
                    if (owners.Count == 0) return;
                }
                else if (!reader.Read())
                {
                    return;
                }
            }
        }

        private void LoadComponents(IReader reader, Dictionary<string, IComponent> components)
        {
            while (reader.State != ReaderState.EOF)
            {
                while (reader.State != ReaderState.StartElement && reader.State != ReaderState.Value && reader.Read())
                {
                }

                if (reader.State != ReaderState.StartElement && reader.State != ReaderState.Value) break;
                string text;
                reader.Attributes.TryGetValue("name", out text);
                if (text != null)
                {
                    IComponent obj = null;
                    if (components != null) components.TryGetValue(text, out obj);
                    var obj2 = LoadObject(reader, obj, true);
                    if (obj2 != null && obj2 is IComponent && Components != null) Components.Add(text, obj2 as IComponent);
                }
            }
        }

        private void LoadProperties(object obj, IReader reader)
        {
            var properties = TypeDescriptor.GetProperties(obj);
            while (reader.Read() && reader.State != ReaderState.EndElement && !reader.Attributes.ContainsKey("name"))
            {
                var name = reader.Name;
                if (!(name == "Controls"))
                {
                    var control = obj as Control;
                    if (name != "Name" || control == null || control.Name == "")
                    {
                        if (reader.Name == "Event")
                        {
                            string eventName;
                            reader.Attributes.TryGetValue("event_name", out eventName);
                            eventBinding.AddEvent(obj, eventName, reader.Value);
                        }
                        else
                        {
                            var propertyDescriptor = properties.Find(name, false);
                            if (propertyDescriptor != null)
                            {
                                var obj2 = LoadValue(propertyDescriptor.PropertyType, propertyDescriptor, obj, reader, false);
                                if (obj2 == null) continue;
                                try
                                {
                                    propertyDescriptor.SetValue(obj, obj2);
                                    continue;
                                }
                                catch (Exception ex)
                                {
                                    WriteToLog(ex.Message);
                                    continue;
                                }
                            }

                            var flag = false;
                            string text;
                            reader.Attributes.TryGetValue("provider", out text);
                            string text2;
                            reader.Attributes.TryGetValue("prop_type", out text2);
                            var property = "Set" + reader.Name;
                            if (text != null && text2 != null)
                            {
                                var type = CreateType(text2);
                                if (type != null)
                                {
                                    flag = true;
                                    var obj3 = LoadValue(type, null, obj, reader, false);
                                    if (obj3 != null)
                                    {
                                        List<Extender> list;
                                        extenders.TryGetValue(text, out list);
                                        if (list == null) list = new List<Extender>();
                                        list.Add(new Extender
                                        {
                                            Control = obj,
                                            Value = obj3,
                                            Property = property
                                        });
                                        extenders[text] = list;
                                    }
                                }
                            }

                            if (!flag && reader.State == ReaderState.StartElement) SkipControl(reader, false);
                        }
                    }
                }
            }

            IComponent component;
            if (ComponentLoaded != null && (component = obj as IComponent) != null) ComponentLoaded(this, new ComponentEventArgs(component));
        }

        private object LoadObject(IReader reader, object obj, bool readName)
        {
            string text;
            reader.Attributes.TryGetValue("type", out text);
            string name;
            if (readName)
            {
                reader.Attributes.TryGetValue("name", out name);
                if (name == null) name = reader.Name;
            }
            else
            {
                obj = CreateObject(text, null, true);
                name = (obj as IComponent).Site.Name;
            }

            loadedObjectName = name;
            if (text.IndexOf("String[]") != -1)
            {
                string text2;
                reader.Attributes.TryGetValue("Length", out text2);
                if (text2 != null) return LoadStrings(reader, Convert.ToInt32(text2));
            }

            if (obj == null)
            {
                if (name != "" && !NeedCreateNew(text)) obj = FindComponent(name);
                if (obj == null) obj = CreateObject(text, name, true);
            }

            IComponent value;
            if (!string.IsNullOrEmpty(name) && (value = obj as IComponent) != null) loadedComponents[name] = value;
            ISupportInitialize supportInitialize;
            if ((supportInitialize = obj as ISupportInitialize) != null && !initedObjects.Contains(obj))
            {
                initedObjects.Add(obj);
                supportInitialize.BeginInit();
            }

            LoadProperties(obj, reader);
            return obj;
        }

        private object LoadStrings(IReader reader, int length)
        {
            var array = new string[length];
            for (var i = 0; i < length; i++)
            {
                reader.Read();
                array.SetValue(reader.Value, i);
            }

            while (reader.State != ReaderState.EndElement && reader.Read())
            {
            }

            return array;
        }

        private object LoadBinary(TypeConverter converter, IReader reader)
        {
            var array = Convert.FromBase64String(reader.Value);
            object result;
            if (converter.CanConvert(typeof(byte[])))
            {
                result = converter.ConvertFrom(null, CultureInfo.InvariantCulture, array);
            }
            else
            {
                var binaryFormatter = new BinaryFormatter();
                var serializationStream = new MemoryStream(array);
                result = binaryFormatter.Deserialize(serializationStream);
            }

            return result;
        }

        private object LoadConstructor(IReader reader)
        {
            while (reader.Read())
                if (reader.State == ReaderState.Value && reader.Name == "type")
                {
                    var obj = CreateObject(reader.Value, null, false);
                    LoadProperties(obj, reader);
                    return obj;
                }

            return null;
        }

        private object LoadInstanceDescriptor(IReader reader, bool needLazyLoad)
        {
            while (reader.Read())
                if (reader.State == ReaderState.Value && reader.Name == "Data")
                {
                    var buffer = Convert.FromBase64String(reader.Value);
                    var binaryFormatter = new BinaryFormatter();
                    var serializationStream = new MemoryStream(buffer);
                    var memberInfo = (MemberInfo) binaryFormatter.Deserialize(serializationStream);
                    object[] array = null;
                    var flag = false;
                    if (memberInfo is MethodBase)
                    {
                        var parameters = ((MethodBase) memberInfo).GetParameters();
                        array = new object[parameters.Length];
                        var i = 0;
                        while (i < parameters.Length)
                        {
                            if (!reader.Read()) return null;
                            if ((reader.State == ReaderState.Value || reader.State == ReaderState.StartElement) && reader.Name == "Param")
                            {
                                if (reader.Attributes.Count != 0)
                                {
                                    string text;
                                    reader.Attributes.TryGetValue("mode", out text);
                                    if (text != null && text == "reference")
                                    {
                                        flag = true;
                                        var lazyParam = new LazyParam();
                                        lazyParam.Name = reader.Value;
                                        array[i++] = lazyParam;
                                        continue;
                                    }
                                }

                                var obj = reader.Attributes.ContainsKey("null") ? null : LoadValue(parameters[i].ParameterType, null, null, reader, false);
                                array[i++] = obj;
                            }

                            if (i == parameters.Length) break;
                        }
                    }

                    if (flag || needLazyLoad)
                    {
                        var result = new InstanceDescriptorLoader(memberInfo, array);
                        var num = 0;
                        while (reader.Read())
                            if (reader.State == ReaderState.StartElement)
                            {
                                num++;
                            }
                            else if (reader.State == ReaderState.EndElement)
                            {
                                if (num == 0) break;
                                num--;
                            }

                        return result;
                    }

                    var obj2 = new InstanceDescriptor(memberInfo, array).Invoke();
                    LoadProperties(obj2, reader);
                    return obj2;
                }

            return null;
        }

        private void LoadList(IList list, string typeName, object control, IReader reader, PropertyDescriptor pd)
        {
            var flag = false;
            while (reader.Read() && reader.State != ReaderState.EndElement)
            {
                Type type;
                if (typeName != null)
                {
                    type = Type.GetType(typeName);
                }
                else
                {
                    if (reader.Attributes.TryGetValue("type", out typeName) && typeName == null) continue;
                    type = Type.GetType(typeName);
                }

                object obj;
                if (typeof(IList).IsAssignableFrom(type))
                {
                    obj = Activator.CreateInstance(type);
                    reader.Attributes.TryGetValue("type", out typeName);
                    LoadList((IList) obj, typeName, null, reader, null);
                }
                else
                {
                    obj = LoadValue(type, pd, control, reader, true);
                }

                if (obj != null)
                {
                    InstanceDescriptorLoader value;
                    if (pd != null && (value = obj as InstanceDescriptorLoader) != null)
                    {
                        var value2 = pd.GetValue(control);
                        if (value2.GetType().IsDataCollection())
                        {
                            if (!lazyList.ContainsKey(value2)) lazyList.Add(value2, new ArrayList());
                            ((ArrayList) lazyList[value2]).Add(value);
                        }
                    }
                    else
                    {
                        try
                        {
                            list.Add(obj);
                            flag = true;
                        }
                        catch (Exception ex)
                        {
                            WriteToLog(ex.Message);
                        }
                    }
                }
            }

            if (flag && designerHost != null && pd != null)
            {
                var componentChangeService = (IComponentChangeService) designerHost.GetService(typeof(IComponentChangeService));
                componentChangeService.OnComponentChanging(control, pd);
                componentChangeService.OnComponentChanged(control, pd, null, list);
            }
        }

        private object LoadCollectionItem(IReader reader)
        {
            var flag = reader.Name.StartsWith("Item");
            if (flag) reader.Read();
            var result = LoadObject(reader, null, true);
            if (flag) reader.Read();
            return result;
        }

        private object LoadArray(Type arrayType, PropertyDescriptor p, object control, IReader reader)
        {
            var arrayList = new ArrayList();
            string typeName;
            reader.Attributes.TryGetValue("type", out typeName);
            LoadList(arrayList, typeName, control, reader, p);
            var array = (Array) Activator.CreateInstance(arrayType, arrayList.Count);
            arrayList.CopyTo(array);
            return array;
        }

        private object LoadValue(Type propType, PropertyDescriptor p, object control, IReader reader, bool loadListInvoke)
        {
            var name = reader.Name;
            object obj = null;
            var num = reader.Attributes.Count;
            if (num != 0 && reader.Attributes.ContainsKey("provider")) num--;
            if (num != 0 && reader.Attributes.ContainsKey("prop_type")) num--;
            if (num != 0)
            {
                string text;
                reader.Attributes.TryGetValue("mode", out text);
                if (text != null)
                {
                    if (text == "binary")
                    {
                        obj = LoadBinary(TypeDescriptor.GetConverter(propType), reader);
                    }
                    else if (text == "instance_descriptor")
                    {
                        obj = LoadInstanceDescriptor(reader, LazyLoadInstance(propType));
                    }
                    else if (text == "constructor")
                    {
                        obj = LoadConstructor(reader);
                    }
                    else if (text == "reference" && control != null)
                    {
                        if (p.PropertyType.IsDataCollection())
                        {
                            var value = p.GetValue(control);
                            if (!lazyList.ContainsKey(value)) lazyList.Add(value, new ArrayList());
                            ((ArrayList) lazyList[value]).Add(reader.Value);
                        }
                        else
                        {
                            var property = p.PropertyType.IsArray ? new ArrayProperty(control, p) : typeof(IList).IsAssignableFrom(p.PropertyType) ? new ListProperty(control, p) : new ComponentProperty(control, p);
                            referencedComponents.Add(reader.Value, property);
                        }
                    }
                }
                else if (reader.Attributes.ContainsKey("content"))
                {
                    obj = TypeDescriptor.GetProperties(control).Find(name, false).GetValue(control);
                    LoadProperties(obj, reader);
                }
                else if (reader.Attributes.ContainsKey("collection"))
                {
                    if (control == null || p == null || reader.State != ReaderState.StartElement || propType.IsArray)
                    {
                        if (propType.IsArray) return LoadArray(propType, p, control, reader);
                        return null;
                    }

                    string typeName;
                    reader.Attributes.TryGetValue("type", out typeName);
                    var value2 = p.GetValue(control);
                    var list = value2 as IList;
                    if (list == null)
                    {
                        var property2 = value2.GetType().GetProperty("List", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty);
                        if (property2 != null) list = (IList) property2.GetValue(value2, new object[0]);
                    }

                    if (list != null)
                    {
                        ClearList(list);
                        LoadList(list, typeName, control, reader, p);
                    }
                }
                else if (reader.Attributes.ContainsKey("control"))
                {
                    var stack = new Stack<Control>();
                    var flag = false;
                    if (reader.State == ReaderState.Value) return null;
                    if (loadListInvoke) return LoadCollectionItem(reader);
                    if (p != null) obj = p.GetValue(control);
                    if (obj != null)
                    {
                        reader.Read();
                        if (reader.State != ReaderState.Value) LoadProperties(obj, reader);
                    }
                    else
                    {
                        if (!reader.Attributes.ContainsKey("type")) reader.Read();
                        var flag2 = false;
                        obj = GetOrCreateObject(null, reader, ref flag2);
                        if (obj != null)
                        {
                            LoadProperties(obj, reader);
                        }
                        else
                        {
                            var num2 = 0;
                            while (reader.Read())
                                if (reader.State == ReaderState.EndElement)
                                {
                                    if (num2 <= 0) break;
                                    num2--;
                                }
                                else if (reader.State == ReaderState.StartElement)
                                {
                                    num2++;
                                }
                        }

                        flag = true;
                    }

                    Control item;
                    if ((item = obj as Control) != null)
                    {
                        stack.Push(item);
                        LoadControls(stack, reader);
                    }
                    else
                    {
                        reader.Read();
                    }

                    if (p == null) return obj;
                    if (flag) p.SetValue(control, obj);
                    return null;
                }
            }
            else
            {
                var converter = TypeDescriptor.GetConverter(propType);
                if (converter.CanConvertFrom(typeof(string)))
                {
                    if (currentVersion == null)
                        obj = converter.ConvertFrom(null, CultureInfo.CurrentCulture, reader.Value);
                    else
                        obj = converter.ConvertFrom(null, CultureInfo.InvariantCulture, reader.Value);
                }

                if (obj == null && propType == typeof(object)) obj = reader.Value;
            }

            return obj;
        }

        private void AddControls()
        {
            foreach (var keyValuePair in addedControls)
            {
                var key = keyValuePair.Key;
                foreach (var value in keyValuePair.Value) key.Controls.Add(value);
            }

            addedControls.Clear();
        }

        private void AddBindings()
        {
            foreach (var obj in lazyList)
            {
                var dictionaryEntry = (DictionaryEntry) obj;
                try
                {
                    ControlBindingsCollection controlBindingsCollection;
                    if ((controlBindingsCollection = dictionaryEntry.Key as ControlBindingsCollection) != null)
                        foreach (var obj2 in dictionaryEntry.Value as ArrayList)
                        {
                            var idl = (InstanceDescriptorLoader) obj2;
                            Binding binding;
                            if ((binding = CreateInstance(idl) as Binding) != null) controlBindingsCollection.Add(binding);
                        }
                }
                catch (Exception ex)
                {
                    WriteToLog(ex.Message);
                }
            }
        }

        private void ClearList(IList list)
        {
            if (designerHost == null)
            {
                list.Clear();
                return;
            }

            for (var i = 0; i < list.Count; i++)
            {
                var obj = list[i];
                IComponent component;
                if ((component = obj as IComponent) == null || component.Site == null || component.Site.Container != designerHost)
                {
                    list.Remove(obj);
                    i--;
                }
            }
        }

        private bool SkipControl(IReader reader, bool readNext)
        {
            var num = 1;
            while (reader.Read())
                if (reader.State == ReaderState.EndElement)
                {
                    if (num == 1) return !readNext || reader.Read();
                    num--;
                }
                else if (reader.State == ReaderState.StartElement)
                {
                    num++;
                }

            return false;
        }

        public string Store(Control[] parents)
        {
            var memoryStream = new MemoryStream();
            string @string;
            using (var xmlFormWriter = new XmlFormWriter(memoryStream))
            {
                Store(parents, xmlFormWriter);
                @string = Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return @string;
        }

        private void StoreHead(object control, IWriter writer)
        {
            var hashtable = new Hashtable();
            var text = "";
            IComponent component;
            if ((component = control as IComponent) != null && component.Site != null) text = component.Site.Name;
            if (text == "") text = GetObjectName(control);
            hashtable["name"] = text;
            hashtable["type"] = control.GetType().AssemblyQualifiedName;
            if (!versionWrited)
            {
                hashtable["version"] = 1.ToString();
                versionWrited = true;
            }

            writer.WriteStartElement("Object", hashtable);
        }

        private void StoreBinary(string name, byte[] value, IWriter writer, IComponent provider)
        {
            var hashtable = new Hashtable();
            hashtable["mode"] = "binary";
            if (provider != null)
            {
                hashtable["provider"] = provider.Site.Name;
                hashtable["prop_type"] = value.GetType().AssemblyQualifiedName;
            }

            writer.WriteValue(name, Convert.ToBase64String(value), hashtable);
        }

        private void StoreInstanceDescriptor(string name, InstanceDescriptor id, object value, Control rootControl, IWriter writer, IComponent provider)
        {
            var hashtable = new Hashtable();
            if (provider != null)
            {
                hashtable["provider"] = provider.Site.Name;
                hashtable["prop_type"] = value.GetType().AssemblyQualifiedName;
            }

            if (id.Arguments.Count == 0 && id.MemberInfo.Name == ".ctor")
            {
                hashtable["mode"] = "constructor";
                writer.WriteStartElement(name, hashtable);
                writer.WriteValue("type", value.GetType().AssemblyQualifiedName, null);
            }
            else
            {
                var binaryFormatter = new BinaryFormatter();
                var memoryStream = new MemoryStream();
                binaryFormatter.Serialize(memoryStream, id.MemberInfo);
                var value2 = Convert.ToBase64String(memoryStream.ToArray());
                hashtable["mode"] = "instance_descriptor";
                writer.WriteStartElement(name, hashtable);
                writer.WriteValue("Data", value2, null);
                foreach (var obj in id.Arguments)
                    if (obj == null)
                    {
                        hashtable.Clear();
                        hashtable["null"] = "1";
                        writer.WriteValue("Param", "", hashtable);
                    }
                    else
                    {
                        StoreValue("Param", obj, rootControl, writer);
                    }
            }

            if (!id.IsComplete) StoreProperties(value, rootControl, writer);
            writer.WriteEndElement(name);
        }

        private int StoreValue(string name, object value, Control rootControl, IWriter writer)
        {
            return StoreValue(name, value, rootControl, writer, null);
        }

        private int StoreValue(string name, object value, Control rootControl, IWriter writer, IComponent provider)
        {
            int result;
            if (value == null)
            {
                result = 0;
            }
            else if (value.GetType().GetCustomAttributes(typeof(BinarySerializationAttribute), false).Length != 0)
            {
                var binaryFormatter = new BinaryFormatter();
                var memoryStream = new MemoryStream();
                binaryFormatter.Serialize(memoryStream, value);
                StoreBinary(name, memoryStream.ToArray(), writer, provider);
                result = 1;
            }
            else
            {
                var converter = TypeDescriptor.GetConverter(value);
                if (value is IComponent)
                {
                    var text = ComponentName(value as IComponent);
                    if (text != null)
                    {
                        var hashtable = new Hashtable();
                        hashtable["mode"] = "reference";
                        writer.WriteValue(name, text, hashtable);
                    }
                    else
                    {
                        StoreObjectAsProperty(name, value, rootControl, writer, provider);
                    }
                }
                else if (converter.CanConvert(typeof(string)))
                {
                    var value2 = (string) converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(string));
                    Hashtable hashtable2 = null;
                    if (provider != null)
                    {
                        hashtable2 = new Hashtable();
                        hashtable2["provider"] = provider.Site.Name;
                        hashtable2["prop_type"] = value.GetType().AssemblyQualifiedName;
                    }

                    writer.WriteValue(name, value2, hashtable2);
                }
                else if (converter.CanConvert(typeof(InstanceDescriptor)))
                {
                    var id = (InstanceDescriptor) converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(InstanceDescriptor));
                    StoreInstanceDescriptor(name, id, value, rootControl, writer, provider);
                }
                else
                {
                    if (converter.CanConvert(typeof(byte[])))
                        try
                        {
                            var value3 = (byte[]) converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(byte[]));
                            StoreBinary(name, value3, writer, provider);
                            goto IL_217;
                        }
                        catch (Exception ex)
                        {
                            WriteToLog(ex.Message);
                            goto IL_217;
                        }

                    if (value is IList) return StoreList(name, (IList) value, rootControl, writer);
                    if (!value.GetType().IsSerializable) return StoreObjectAsProperty(name, value, rootControl, writer, provider);
                    var binaryFormatter2 = new BinaryFormatter();
                    var memoryStream2 = new MemoryStream();
                    binaryFormatter2.Serialize(memoryStream2, value);
                    StoreBinary(name, memoryStream2.ToArray(), writer, provider);
                }

                IL_217:
                result = 1;
            }

            return result;
        }

        private int StoreList(string name, IList value, Control rootControl, IWriter writer)
        {
            int result;
            if (value.Count == 0)
            {
                result = 0;
            }
            else
            {
                var hashtable = new Hashtable();
                hashtable["collection"] = "true";
                hashtable["type"] = value[0].GetType().AssemblyQualifiedName;
                var lazyWrite = writer as ILazyWrite;
                if (lazyWrite != null) lazyWrite.Begin();
                writer.WriteStartElement(name, hashtable);
                var num = 0;
                var num2 = 0;
                foreach (var value2 in value)
                {
                    num2 += StoreValue("Item" + num, value2, rootControl, writer);
                    num++;
                }

                writer.WriteEndElement(name);
                if (lazyWrite != null) lazyWrite.End(num2 == 0);
                result = num2;
            }

            return result;
        }

        private int StoreObjectAsProperty(string propName, object value, Control rootControl, IWriter writer, IComponent provider)
        {
            Control control;
            if ((control = value as Control) != null && designerHost != null)
            {
                var control2 = designerHost.RootComponent as Control;
                while (control != null && control != control2) control = control.Parent;
                if (control == null) return 0;
            }

            var hashtable = new Hashtable();
            hashtable["control"] = "true";
            if (provider != null)
            {
                hashtable["provider"] = provider.Site.Name;
                hashtable["prop_type"] = value.GetType().AssemblyQualifiedName;
            }

            var lazyWrite = writer as ILazyWrite;
            if (lazyWrite != null) lazyWrite.Begin();
            writer.WriteStartElement(propName, hashtable);
            var num = StoreControl(value, rootControl, writer);
            writer.WriteEndElement(propName);
            if (lazyWrite != null) lazyWrite.End(num == 0);
            return num;
        }

        internal int StoreMember(object control, PropertyDescriptor prop, Control rootControl, IWriter writer)
        {
            var num = 0;
            var attributes = prop.Attributes;
            var designerSerializationVisibility = ((DesignerSerializationVisibilityAttribute) attributes[typeof(DesignerSerializationVisibilityAttribute)]).Visibility;
            if (designerSerializationVisibility == DesignerSerializationVisibility.Hidden && attributes[typeof(ExtenderProvidedPropertyAttribute)] != null && (prop.Name == "Row" || prop.Name == "Column")) designerSerializationVisibility = DesignerSerializationVisibility.Visible;
            if (designerSerializationVisibility != DesignerSerializationVisibility.Visible)
            {
                if (designerSerializationVisibility == DesignerSerializationVisibility.Content)
                {
                    var value = prop.GetValue(control);
                    if (typeof(IList).IsAssignableFrom(prop.PropertyType))
                    {
                        num += StoreList(prop.Name, (IList) value, rootControl, writer);
                    }
                    else if (prop.PropertyType.IsDataCollection())
                    {
                        var value2 = (IList) value.GetType().GetProperty("List", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty).GetValue(value, new object[0]);
                        num += StoreList(prop.Name, value2, rootControl, writer);
                    }
                    else
                    {
                        num += StoreObjectAsProperty(prop.Name, value, rootControl, writer, null);
                    }
                }
            }
            else
            {
                IComponent provider = null;
                var control2 = control as Control;
                object value3;
                if (prop.Name == "Visible" && control2 != null && control2.Parent != null && !control2.Parent.Visible)
                {
                    value3 = prop.GetValue(control);
                }
                else
                {
                    value3 = prop.GetValue(control);
                    var extenderProvidedPropertyAttribute = attributes[typeof(ExtenderProvidedPropertyAttribute)] as ExtenderProvidedPropertyAttribute;
                    IComponent component;
                    if (extenderProvidedPropertyAttribute != null && extenderProvidedPropertyAttribute.Provider != null && value3 != null && (component = extenderProvidedPropertyAttribute.Provider as IComponent) != null && component.Site != null) provider = component;
                }

                if (!prop.IsReadOnly) num += StoreValue(prop.Name, value3, rootControl, writer, provider);
            }

            return num;
        }

        private int StoreProperties(object control, Control rootControl, IWriter writer)
        {
            var num = 0;
            foreach (var obj in TypeDescriptor.GetProperties(control))
            {
                var propertyDescriptor = (PropertyDescriptor) obj;
                try
                {
                    if ((StoreMode == StoreModes.AllProperties || propertyDescriptor.ShouldSerializeValue(control)) && propertyDescriptor.Name != "Controls") num += StoreMember(control, propertyDescriptor, rootControl, writer);
                }
                catch (Exception ex)
                {
                    var text = "序列化属性" + propertyDescriptor.Name + "发生异常，" + ex.Message;
                    if (ShowErrorMessage) MessageBox.Show(text);
                    WriteToLog(text);
                }
            }

            var eventDatas = eventBinding.GetEventDatas(control);
            if (eventDatas != null)
            {
                var hashtable = new Hashtable();
                foreach (var eventData in eventDatas)
                {
                    hashtable["event_name"] = eventData.EventName;
                    writer.WriteValue("Event", eventData.MethodName, hashtable);
                }
            }

            return num;
        }

        private void StoreTail(object control, IWriter writer)
        {
            writer.WriteEndElement("Object");
        }

        internal int StoreControl(object obj, Control rootControl, IWriter writer)
        {
            var num = 0;
            StoreHead(obj, writer);
            num += StoreProperties(obj, rootControl, writer);
            Control control;
            if ((control = obj as Control) != null)
            {
                var control2 = SubstRoot(rootControl, control);
                var flag = false;
                var enumerator = drillList.GetEnumerator();
                while (enumerator.MoveNext())
                    if (((DrillDownHandler) enumerator.Current)(control2))
                    {
                        flag = true;
                        break;
                    }

                if (flag)
                    foreach (var control3 in control2.Controls.ToArray())
                        if (control3 != null && (designerHost == null || control3.Site != null && control3.Site.GetService(typeof(IDesignerHost)) == designerHost))
                            num += StoreControl(control3, rootControl, writer);
            }

            StoreTail(obj, writer);
            return num;
        }

        internal void BeforeWriting()
        {
            versionWrited = false;
        }

        internal void AfterWriting()
        {
            versionWrited = false;
        }

        private Type CreateType(string type)
        {
            var num = type.IndexOf(",");
            var name = type.Substring(0, num);
            var text = type.Substring(num + 1).Trim();
            ITypeResolutionService typeResolutionService;
            Type type2;
            if (designerHost != null && (typeResolutionService = designerHost.GetService(typeof(ITypeResolutionService)) as ITypeResolutionService) != null)
            {
                type2 = typeResolutionService.GetType(name, false, false);
                if (type2 != null) return type2;
            }

            WriteToLog("加载程序集 " + text);
            Assembly assembly;
            try
            {
                assembly = Assembly.Load(text);
            }
            catch (Exception ex)
            {
                WriteToLog("加载程序集失败: " + ex.Message);
                assembly = null;
            }

            if (assembly == null)
            {
                if (ShowErrorMessage) MessageBox.Show(string.Concat("从程序集 ", text, " 创建类型 ", type, " 失败"));
                type2 = null;
            }
            else
            {
                type2 = assembly.GetType(name);
            }

            return type2;
        }

        private object CreateObject(string type, string name, bool createComponent)
        {
            var type2 = CreateType(type);
            if (type2 == null) return null;
            object obj = null;
            if (designerHost != null && createComponent && typeof(IComponent).IsAssignableFrom(type2)) obj = designerHost.CreateComponent(type2, name);
            if (obj == null)
            {
                WriteToLog("创建组件 " + type2);
                try
                {
                    obj = Activator.CreateInstance(type2);
                    Control control;
                    if ((control = obj as Control) != null) control.Name = name;
                }
                catch (Exception ex)
                {
                    var text = "创建类型 " + type2 + " 失败，" + ex.Message;
                    if (ShowErrorMessage) MessageBox.Show(text);
                    WriteToLog(text);
                    obj = null;
                }

                if (obj != null) WriteToLog("创建组件完成。");
            }

            IComponent value;
            if (obj != null && name != null && (value = obj as IComponent) != null && !loadedComponents.ContainsKey(name)) loadedComponents.Add(name, value);
            ISupportInitialize supportInitialize;
            if (obj != null && (supportInitialize = obj as ISupportInitialize) != null)
            {
                initedObjects.Add(obj);
                supportInitialize.BeginInit();
            }

            return obj;
        }

        private object CreateInstance(InstanceDescriptorLoader idl)
        {
            for (var i = 0; i < idl.Arguments.Length; i++)
            {
                var lazyParam = idl.Arguments[i] as LazyParam;
                if (lazyParam != null)
                {
                    if (lazyParam.Name == "DesignedForm")
                    {
                        idl.Arguments[i] = designedForm;
                    }
                    else
                    {
                        IComponent component;
                        loadedComponents.TryGetValue(lazyParam.Name, out component);
                        idl.Arguments[i] = component;
                    }
                }
            }

            return new InstanceDescriptor(idl.MemberInfo, idl.Arguments).Invoke();
        }

        private string ComponentName(IComponent component)
        {
            if (component.Site != null && component.Site.Container == designerHost) return component.Site.Name;
            if (Components != null)
                foreach (var keyValuePair in Components.Components)
                    if (keyValuePair.Value == component)
                        return keyValuePair.Key;
            if (component != designedForm) return null;
            return "DesignedForm";
        }

        private bool DrillDownDefault(IComponent control)
        {
            return !(control is BindingNavigator) && (!(control is ContainerControl) || control is Form || designerHost != null && control == designerHost.RootComponent || control.GetType().ToString().IndexOf("DesignSurface") != -1);
        }

        private IComponent FindComponent(string name)
        {
            if (designerHost == null || name == null) return null;
            foreach (var obj in ((IContainer) designerHost.GetService(typeof(IContainer))).Components)
            {
                var component = (IComponent) obj;
                if (component.Site.Name == name) return component;
            }

            if (Components != null)
                foreach (var keyValuePair in Components.Components)
                    if (keyValuePair.Key == name)
                        return keyValuePair.Value;
            return null;
        }

        private object GetOrCreateObject(Control parent, IReader reader, ref bool finded)
        {
            string type;
            reader.Attributes.TryGetValue("type", out type);
            string text;
            reader.Attributes.TryGetValue("name", out text);
            if (text == null) text = reader.Name;
            finded = false;
            if (LoadMode == LoadModes.Duplicate)
            {
                if (designerHost != null && parent.FindControl(text) != null) text = null;
                return CreateObject(type, text, true);
            }

            object obj = null;
            if (parent != null && text != null && text != "") obj = parent.FindControl(text);
            if (obj != null)
            {
                finded = true;
                return obj;
            }

            if (parent is IBaseDataWindow bdw)
            {
                obj = bdw.GetInherentControl(text);
                if (obj != null)
                {
                    finded = true;
                    return obj;
                }
            }

            var flag = LoadMode != LoadModes.ModifyExisting;
            if (LoadMode == LoadModes.ModifyExisting)
            {
                var type2 = CreateType(type);
                if (type2 != null) flag = !typeof(IComponent).IsAssignableFrom(type2);
            }

           
            if (flag) obj = CreateObject(type, text, true);
            return obj;
        }

        private string GetObjectName(object control)
        {
            Control control2;
            string result;
            if ((control2 = control as Control) != null)
            {
                result = control2.Name;
            }
            else
            {
                if (Components != null)
                    foreach (var keyValuePair in Components.Components)
                        if (keyValuePair.Value == control)
                            return keyValuePair.Key;
                if (designerHost != null)
                {
                    var nameCreationService = (INameCreationService) designerHost.GetService(typeof(INameCreationService));
                    var container = (IContainer) designerHost.GetService(typeof(IContainer));
                    result = nameCreationService.CreateName(container, control.GetType());
                }
                else if (control is DataTable)
                {
                    result = ((DataTable) control).TableName;
                }
                else if (control is DataColumn)
                {
                    result = ((DataColumn) control).ColumnName;
                }
                else
                {
                    result = "";
                }
            }

            return result;
        }

        private void InvokeAddRange()
        {
            foreach (var obj in lazyList)
            {
                var dictionaryEntry = (DictionaryEntry) obj;
                var key = dictionaryEntry.Key;
                var method = key.GetType().GetMethod("AddRange");
                if (!(method == null))
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType.IsArray)
                    {
                        var arrayList = dictionaryEntry.Value as ArrayList;
                        var arrayList2 = new ArrayList();
                        foreach (var obj2 in arrayList)
                        {
                            var obj3 = obj2;
                            if (obj2 is string)
                            {
                                IComponent component;
                                loadedComponents.TryGetValue((string) obj2, out component);
                                obj3 = component;
                            }
                            else if (obj2 is InstanceDescriptorLoader)
                            {
                                obj3 = CreateInstance((InstanceDescriptorLoader) obj2);
                            }

                            if (obj3 != null && !(key as IEnumerable).Contains(obj3)) arrayList2.Add(obj3);
                        }

                        var array = (Array) Activator.CreateInstance(parameters[0].ParameterType, arrayList2.Count);
                        arrayList2.CopyTo(array);
                        try
                        {
                            method.Invoke(key, new object[]
                            {
                                array
                            });
                        }
                        catch (Exception ex)
                        {
                            WriteToLog(ex.Message);
                        }
                    }
                }
            }
        }

        private bool LazyLoadInstance(Type type)
        {
            return typeof(DataRelation).IsAssignableFrom(type) || typeof(Constraint).IsAssignableFrom(type);
        }

        private bool NeedCreateNew(string typeName)
        {
            var result = false;
            var type = CreateType(typeName);
            if (type != null) result = typeof(DataTable).IsAssignableFrom(type) || typeof(DataColumn).IsAssignableFrom(type);
            return result;
        }

        private void PrepareParent(IReader reader, bool ignoreParent, Control parent)
        {
            if (LoadMode == LoadModes.EraseForm)
            {
                if (designerHost != null)
                {
                    var container = (IContainer) designerHost.GetService(typeof(IContainer));
                    ((ISelectionService) designerHost.GetService(typeof(ISelectionService))).SetSelectedComponents(null);
                    foreach (var obj in container.Components)
                    {
                        var component = (IComponent) obj;
                        if (component != designerHost.RootComponent)
                        {
                            try
                            {
                                container.Remove(component);
                            }
                            catch (Exception ex)
                            {
                                WriteToLog(ex.Message);
                            }

                            if (component != null) component.Dispose();
                        }
                    }

                    if (Components != null) Components.Clear();
                }
                else
                {
                    foreach (var obj2 in parent.Controls) ((Control) obj2).Dispose();
                }
            }
        }

        private void SetExtendProviders()
        {
            foreach (var keyValuePair in extenders)
            {
                IComponent component;
                if (designerHost != null)
                    component = ((IContainer) designerHost.GetService(typeof(IContainer))).Components[keyValuePair.Key];
                else
                    loadedComponents.TryGetValue(keyValuePair.Key, out component);
                if (component != null)
                    foreach (var extender in keyValuePair.Value)
                    {
                        var method = component.GetType().GetMethod(extender.Property);
                        if (method != null)
                            method.Invoke(component, new[]
                            {
                                extender.Control,
                                extender.Value
                            });
                    }
            }
        }

        private void SetReferences()
        {
            foreach (var referencedItem in referencedComponents)
            {
                IComponent component;
                if (loadedComponents.TryGetValue(referencedItem.Key, out component))
                {
                    if (referencedItem.Key == "DesignedForm")
                    {
                        component = designedForm;
                    }
                    else
                    {
                        if (designerHost != null)
                        {
                            var component2 = ((IContainer) designerHost.GetService(typeof(IContainer))).Components[referencedItem.Key];
                            if (component2 != null) component = component2;
                        }

                        if (component == null) continue;
                    }
                }

                foreach (var componentProperty in referencedItem.Properties) componentProperty.SetProperty(component);
            }
        }

        private Control SubstRoot(Control rootControl, Control control)
        {
            if (rootControl == null || rootControl == control) return control;
            for (var parent = control.Parent; parent != null; parent = parent.Parent)
                if (parent == rootControl)
                    return control;
            return rootControl;
        }

        private void WriteToLog(string str)
        {
            if (LogFile != null)
                using (var streamWriter = File.AppendText(LogFile))
                {
                    streamWriter.WriteLine(str);
                }
        }
    }
}