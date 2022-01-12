using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DataWindow.CustomPropertys;

namespace DataWindow.DesignLayer
{
    public class PropertyboxControl : System.Windows.Forms.UserControl
    {
        private Designer _designer;

        private ComboBox comboBox;
        public localizationPropertyGrid propertyGrid;

        public PropertyboxControl()
        {
            InitializeComponent();
            InitializeCombobx();
        }

        public bool ShowEventTab { get; set; }

        public Designer Designer
        {
            get => _designer;
            set
            {
                if (_designer != value)
                {
                    _designer = value;
                    propertyGrid.Site = new PropertyGridSite(_designer.DesignerHost);
                    propertyGrid.PropertyTabs.RemoveTabType(typeof(EventsTab));
                    if (ShowEventTab) propertyGrid.PropertyTabs.AddTabType(typeof(EventsTab));
                }
            }
        }

        private void InitializeCombobx()
        {
            comboBox.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.FormattingEnabled = true;
            comboBox.Sorted = true;
            comboBox.DrawItem += ComboBox_DrawItem;
            comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
        }

        private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= comboBox.Items.Count) return;
            var graphics = e.Graphics;
            var brush = SystemBrushes.ControlText;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                {
                    graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
                    brush = SystemBrushes.HighlightText;
                }
                else
                {
                    graphics.FillRectangle(SystemBrushes.Window, e.Bounds);
                }
            }
            else
            {
                graphics.FillRectangle(SystemBrushes.Window, e.Bounds);
            }

            var obj = comboBox.Items[e.Index];
            var num = e.Bounds.X;
            IComponent component;
            if ((component = obj as IComponent) != null)
            {
                var text = string.Empty;
                Control control;
                if (component.Site != null)
                    text = component.Site.Name;
                else if ((control = obj as Control) != null) text = control.Name;
                if (string.IsNullOrEmpty(text)) text = obj.GetType().Name;
                using (var font = new Font(comboBox.Font, FontStyle.Bold))
                {
                    graphics.DrawString(text, font, brush, num, e.Bounds.Y);
                    num += (int) graphics.MeasureString(text + "-", font).Width;
                }
            }

            var s = obj.GetType().ToString();
            graphics.DrawString(s, comboBox.Font, brush, num, e.Bounds.Y);
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectionService = _designer.SelectionService;
            if (comboBox.SelectedItem != null)
            {
                selectionService.SetSelectedComponents(new[]
                {
                    comboBox.SelectedItem
                });
                Designer.DesignedForm.Focus();
            }
        }

        public void SetComponents(ComponentCollection components)
        {
            comboBox.Items.Clear();
            if (components != null)
                foreach (var item in components)
                    comboBox.Items.Add(item);
            if (propertyGrid.SelectedObject is CustomPropertyCollection cpc)
            {
                comboBox.SelectedItem = cpc.Sources;
            }

            comboBox.SelectedItem = propertyGrid.SelectedObject;
        }

        public void SetSelectedObjects(params object[] selectedObjects)
        {
            if (selectedObjects != null && selectedObjects.Length == 0)
            {
                propertyGrid.SelectedObject = null;
                return;
            }

            if (selectedObjects.Length == 1)
            {
                propertyGrid.SelectedObject = selectedObjects[0];
                comboBox.SelectedItem = selectedObjects[0];
                return;
            }

            propertyGrid.SelectedObjects = selectedObjects;
            comboBox.SelectedItem = null;
        }

        private void InitializeComponent()
        {
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.propertyGrid = new localizationPropertyGrid();
            this.SuspendLayout();
            // 
            // comboBox
            // 
            this.comboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboBox.FormattingEnabled = true;
            this.comboBox.Location = new System.Drawing.Point(0, 0);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(288, 20);
            this.comboBox.TabIndex = 0;
            // 
            // propertyGrid
            // 
            this.propertyGrid.DisplayMode = localizationPropertyGrid.DisplayModeEnum.ForNormalUser;
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 20);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(288, 413);
            this.propertyGrid.TabIndex = 1;
            // 
            // PropertyboxControl
            // 
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.comboBox);
            this.Name = "PropertyboxControl";
            this.Size = new System.Drawing.Size(288, 433);
            this.ResumeLayout(false);
        }
    }
}