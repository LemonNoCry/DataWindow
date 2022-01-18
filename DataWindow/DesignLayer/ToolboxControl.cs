using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using DataWindow.Core;
using DataWindow.Toolbox;
using DataWindow.Utility;

namespace DataWindow.DesignLayer
{
    public class ToolboxControl : UserControl, IToolbox
    {
        private readonly Dictionary<string, List<ToolboxBaseItem>> _toolboxItems;

        private readonly ToolboxService _toolboxService;
        private readonly string DefaultCategory = "常规";

        private ImageList _images;

        private ToolboxList _listbox;

        private IContainer components;

        public ToolboxControl()
        {
            InitializeComponent();
            _toolboxItems = new Dictionary<string, List<ToolboxBaseItem>>();
            _listbox.Click += OnItemClick;
            _listbox.DoubleClick += OnItemDoubleClick;
            _listbox.ItemDrag += OnItemDrag;
            _toolboxService = new ToolboxService(this);
        }

        public Designer Designer
        {
            get => _toolboxService.Designer;
            set => _toolboxService.Designer = value;
        }

        void IToolbox.AddItem(ToolboxItem item, string category)
        {
            if (category == null) category = DefaultCategory;
            if (!_toolboxItems.ContainsKey(category)) AddCategory(category);
            var list = _toolboxItems[category];
            var image = _images.Images.Add(item.Bitmap, item.Bitmap.GetPixel(0, 0));
            list.Add(new ToolboxBaseItem(item.DisplayName, item.DisplayName, image, item));
            var baseGroup = _listbox.Items.Cast<ToolboxBaseItem>().FirstOrDefault(s => s.Text.Equals(category));
            if (baseGroup != null)
            {
                if (baseGroup.Tag is ToolboxCategoryState tcs && tcs == ToolboxCategoryState.Expanded)
                {
                    var num = FindCategoryItem(category);
                    var item2 = new ToolboxBaseItem(item.DisplayName, item.DisplayName, image, item);
                    _listbox.Items.Insert(num + 1 + list.Count, item2);
                }
            }
        }

        void IToolbox.RemoveItem(ToolboxItem item, string category)
        {
            if (category == null) category = DefaultCategory;
            if (!_toolboxItems.ContainsKey(category)) return;
            var list = _toolboxItems[category];
            var num = 0;
            using (var enumerator = list.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Tag as ToolboxItem == item)
                    {
                        list.RemoveAt(num);
                        var num2 = FindCategoryItem(category);
                        _listbox.Items.RemoveAt(num2 + 2 + num);
                        break;
                    }

                    num++;
                }
            }
        }

        event EventHandler IToolbox.BeginDragAndDrop
        {
            add => _BeginDragAndDrop += value;
            remove => _BeginDragAndDrop -= value;
        }

        event EventHandler IToolbox.DropControl
        {
            add => _DropControl += value;
            remove => _DropControl -= value;
        }

        event EventHandler<ToolboxItemUsedArgs> IToolbox.ToolboxItemUsed
        {
            add => _ToolboxItemUsed += value;
            remove => _ToolboxItemUsed -= value;
        }

        ToolboxCategoryCollection IToolbox.Items
        {
            get
            {
                var array = new ToolboxCategoryItem[_toolboxItems.Count];
                var num = 0;
                foreach (var keyValuePair in _toolboxItems)
                {
                    var value = keyValuePair.Value;
                    var array2 = new ToolboxItem[value.Count];
                    var num2 = 0;
                    foreach (var toolboxBaseItem in value) array2[num2++] = toolboxBaseItem.Tag as ToolboxItem;
                    array[num++] = new ToolboxCategoryItem(keyValuePair.Key, new ToolboxItemCollection(array2));
                }

                return new ToolboxCategoryCollection(array);
            }
        }

        string IToolbox.SelectedCategory
        {
            get
            {
                var selectedIndex = _listbox.SelectedIndex;
                if (selectedIndex < 0) return null;
                ToolboxBaseItem toolboxBaseItem;
                do
                {
                    toolboxBaseItem = (ToolboxBaseItem) _listbox.Items[selectedIndex--];
                } while (!toolboxBaseItem.IsGroup && selectedIndex >= 0);

                if (!toolboxBaseItem.IsGroup) return null;
                return toolboxBaseItem.Text;
            }
            set
            {
                var num = FindCategoryItem(value);
                if (num < 0) return;
                if ((ToolboxCategoryState) ((ToolboxBaseItem) _listbox.Items[num]).Tag == ToolboxCategoryState.Collapsed) ExpandCategory(num);
                _listbox.SelectedIndex = num + 1;
            }
        }

        [Browsable(false)]
        ToolboxItem IToolbox.SelectedItem
        {
            get
            {
                var selectedItem = _listbox.SelectedItem;
                return (selectedItem != null ? selectedItem.Tag : null) as ToolboxItem;
            }
            set
            {
                if (value != null)
                {
                    foreach (var obj in _listbox.Items)
                    {
                        var toolboxBaseItem = (ToolboxBaseItem) obj;
                        if (toolboxBaseItem.Tag == value)
                        {
                            _listbox.SelectedItem = toolboxBaseItem;
                            break;
                        }
                    }

                    return;
                }

                _listbox.SelectedIndex = -1;
                var toolboxItemUsed = _ToolboxItemUsed;
                if (toolboxItemUsed == null) return;
                toolboxItemUsed(this, new ToolboxItemUsedArgs(null));
            }
        }

        public void LoadFile(string filename)
        {
            var controlLibraryManager = new ControlLibraryManager();
            controlLibraryManager.Load(filename);
            foreach (var category in controlLibraryManager.Categories)
                if (category.IsEnabled)
                    foreach (var toolComponent in category.ToolComponents)
                    {
                        var toolboxItem = new ToolboxItem();
                        toolboxItem.AssemblyName = toolComponent.LoadAssembly().GetName();
                        toolboxItem.Bitmap = controlLibraryManager.GetIcon(toolComponent);
                        toolboxItem.DisplayName = toolComponent.Name;
                        toolboxItem.TypeName = toolComponent.FullName;
                        _toolboxService.AddToolboxItem(toolboxItem, category.Name);
                    }
        }

        public void AddToolboxItem(Type componentType, string category, string displayName = null)
        {
            var tbi = CreateToolboxItem(componentType);
            tbi.DisplayName = displayName ?? componentType.Name;
            _toolboxService.AddToolboxItem(tbi, category);
        }

        public void AddToolboxItem(Control control, string category, string displayName = null)
        {
            var tbi = CreateToolboxItem(control.GetType());
            tbi.DisplayName = displayName ?? control.Name;
            tbi.Properties.Add("Source", control.ControlConvertSerializable());

            _toolboxService.AddToolboxItem(tbi, category);
        }

        public void ClearToolboxItem(string category)
        {
            var lis = _toolboxItems[category];
            lis.ForEach(s => _listbox.Items.Remove(s));
            lis.Clear();
        }

        public void AddCategory(string name, ToolboxCategoryState toolboxCategoryState = ToolboxCategoryState.Expanded)
        {
            var toolboxBaseItem = new ToolboxBaseItem(name, name, toolboxCategoryState == ToolboxCategoryState.Expanded ? 2 : 1, true);
            toolboxBaseItem.Tag = toolboxCategoryState;
            var toolboxPointerItem = new ToolboxPointerItem(0);
            var items = _listbox.Items;
            object[] items2 =
            {
                toolboxBaseItem,
                toolboxPointerItem
            };
            if (toolboxCategoryState == ToolboxCategoryState.Collapsed)
            {
                items.Add(toolboxBaseItem);
            }
            else
            {
                items.AddRange(items2);
            }

            _toolboxItems[name] = new List<ToolboxBaseItem>();
        }

        private int FindCategoryItem(string category)
        {
            var num = 0;
            foreach (var obj in _listbox.Items)
            {
                var toolboxBaseItem = (ToolboxBaseItem) obj;
                if (toolboxBaseItem.IsGroup && toolboxBaseItem.Text == category) return num;
                num++;
            }

            return -1;
        }

        private void CollapseCategory(int categoryItem)
        {
            var toolboxBaseItem = (ToolboxBaseItem) _listbox.Items[categoryItem];
            if (!toolboxBaseItem.IsGroup || (ToolboxCategoryState) toolboxBaseItem.Tag == ToolboxCategoryState.Collapsed) return;
            toolboxBaseItem.Tag = ToolboxCategoryState.Collapsed;
            _listbox.BeginUpdate();
            toolboxBaseItem.ImageIndex = 1;
            _listbox.Invalidate(_listbox.GetItemRectangle(categoryItem));
            var list = _toolboxItems[toolboxBaseItem.Text];
            categoryItem++;
            _listbox.Items.RemoveAt(categoryItem);
            for (var i = 0; i < list.Count; i++) _listbox.Items.RemoveAt(categoryItem);
            _listbox.EndUpdate();
        }

        private void ExpandCategory(int categoryItem)
        {
            var toolboxBaseItem = (ToolboxBaseItem) _listbox.Items[categoryItem];
            if (!toolboxBaseItem.IsGroup || (ToolboxCategoryState) toolboxBaseItem.Tag == ToolboxCategoryState.Expanded) return;
            toolboxBaseItem.Tag = ToolboxCategoryState.Expanded;
            _listbox.BeginUpdate();
            toolboxBaseItem.ImageIndex = 2;
            _listbox.Invalidate(_listbox.GetItemRectangle(categoryItem));
            var list = _toolboxItems[toolboxBaseItem.Text];
            _listbox.Items.Insert(++categoryItem, new ToolboxPointerItem(0));
            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                _listbox.Items.Insert(++categoryItem, item);
            }

            _listbox.EndUpdate();
        }

        private void OnItemDrag(object sender, ToolboxItemDragEventArgs arg)
        {
            if (arg.Item.IsGroup) return;
            if (_BeginDragAndDrop != null && arg.Item.Tag is ToolboxItem) _BeginDragAndDrop(this, null);
        }

        private void OnItemDoubleClick(object sender, EventArgs e)
        {
            var selectedItem = _listbox.SelectedItem;
            if (selectedItem.IsGroup) return;
            if (_DropControl != null && selectedItem.Tag is ToolboxItem) _DropControl(this, new EventArgs());
        }

        private void OnItemClick(object sender, EventArgs e)
        {
            var selectedIndex = _listbox.SelectedIndex;
            if (selectedIndex < 0) return;
            var selectedItem = _listbox.SelectedItem;
            if (selectedItem.IsGroup)
            {
                if ((ToolboxCategoryState) selectedItem.Tag == ToolboxCategoryState.Expanded)
                {
                    CollapseCategory(selectedIndex);
                    return;
                }

                ExpandCategory(selectedIndex);
            }
        }

        internal CustomToolboxItem CreateToolboxItem(Type type)
        {
            var toolboxItemAttribute = new ToolboxItemAttribute(typeof(CustomToolboxItem));
            //TypeDescriptor.GetAttributes(type)[typeof(ToolboxItemAttribute)] as ToolboxItemAttribute;
            var constructor = toolboxItemAttribute.ToolboxItemType.GetConstructor(Type.EmptyTypes);
            if (constructor == null) return new CustomToolboxItem(type);
            var toolboxItem = (CustomToolboxItem) constructor.Invoke(new object[0]);
            toolboxItem.Initialize(type);

            ToolboxBitmapAttribute toolboxBitmapAttribute = (ToolboxBitmapAttribute)TypeDescriptor.GetAttributes(type.GetControlRealType())[typeof(ToolboxBitmapAttribute)];
            if (toolboxBitmapAttribute != null)
            {
               toolboxItem.Bitmap = toolboxBitmapAttribute.GetImage(type.GetControlRealType()) as Bitmap;
            }

            return toolboxItem;
        }


        private event EventHandler _BeginDragAndDrop;

        private event EventHandler _DropControl;

        private event EventHandler<ToolboxItemUsedArgs> _ToolboxItemUsed;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolboxControl));
            this._images = new System.Windows.Forms.ImageList(this.components);
            this._listbox = new DataWindow.Toolbox.ToolboxList();
            this.SuspendLayout();
            // 
            // _images
            // 
            this._images.ImageStream = ((System.Windows.Forms.ImageListStreamer) (resources.GetObject("_images.ImageStream")));
            this._images.TransparentColor = System.Drawing.Color.Transparent;
            this._images.Images.SetKeyName(0, "arrow_cursor_16px.png");
            this._images.Images.SetKeyName(1, "collapsel_16x.png");
            this._images.Images.SetKeyName(2, "expand_16x.png");
            // 
            // _listbox
            // 
            this._listbox.BackColor = System.Drawing.SystemColors.Control;
            this._listbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._listbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._listbox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this._listbox.GroupBackColor = System.Drawing.SystemColors.Window;
            this._listbox.Images = this._images;
            this._listbox.ItemHeight = 24;
            this._listbox.ItemHoverBackColor = System.Drawing.SystemColors.ControlLight;
            this._listbox.Location = new System.Drawing.Point(0, 0);
            this._listbox.Name = "_listbox";
            this._listbox.SelectedItemBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this._listbox.SelectedItemBorderColor = System.Drawing.SystemColors.ActiveBorder;
            this._listbox.SelectedItemHoverBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._listbox.Size = new System.Drawing.Size(173, 368);
            this._listbox.TabIndex = 0;
            // 
            // ToolboxControl
            // 
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Controls.Add(this._listbox);
            this.Name = "ToolboxControl";
            this.Size = new System.Drawing.Size(173, 368);
            this.ResumeLayout(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_toolboxService != null) _toolboxService.Dispose();
                if (components != null) components.Dispose();
            }

            base.Dispose(disposing);
        }

        internal enum PictureIndex
        {
            Arrow,
            Plus,
            Minus
        }
    }
}