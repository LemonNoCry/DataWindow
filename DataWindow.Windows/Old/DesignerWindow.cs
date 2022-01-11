using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using DataWindow.DesignerInternal;
using DataWindow.DesignLayer;

namespace DataWindow.Windows.Old
{
    public partial class DesignerWindow : Form
    {
        private Designer designer;

        public DesignerWindow()
        {
            InitializeComponent();
            InitToolbar();
            InitDesigner();
            InitToolbox();
            InitPropertybox();
        }

        internal event EventHandler DesignEnded;

        private void DesignerWindow_Load(object sender, EventArgs e)
        {
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (designer.Active)
            {
                EndDesign();
            }
            else
            {
                var designEnded = DesignEnded;
                if (designEnded != null) designEnded(this, new EventArgs());
            }

            base.OnClosing(e);
        }

        private void InitToolbar()
        {
            EnableAlignResize(false);
        }

        protected void InitToolbox()
        {
            toolboxControl.AddToolboxItem(typeof(Button), "General");
            toolboxControl.AddToolboxItem(typeof(ListView), "General");
            toolboxControl.AddToolboxItem(typeof(TabControl), "General");
            toolboxControl.AddToolboxItem(typeof(TreeView), "General");
            toolboxControl.AddToolboxItem(typeof(TextBox), "General");
            toolboxControl.AddToolboxItem(typeof(Label), "General");
            toolboxControl.AddToolboxItem(typeof(Panel), "Containers");
            toolboxControl.AddToolboxItem(typeof(GroupBox), "Containers");
            toolboxControl.AddToolboxItem(typeof(DataGridView), "Data");
            toolboxControl.AddToolboxItem(typeof(Timer), "Components");
            toolboxControl.AddToolboxItem(typeof(BindingSource), "Components");
            toolboxControl.Designer = designer;
        }

        protected void InitPropertybox()
        {
            propertybox.Site = new PropertyGridSite(designer.DesignerHost);
            propertybox.Designer = designer;
        }

        protected void InitDesigner()
        {
            designer = designerControl1.Designer;
            designer.KeyDown += DesignedForm_KeyDown;
            designer.SelectionService.SelectionChanged += SelectionChanged;
            designer.ComponentChangeService.ComponentAdded += ComponentAdded;
            designer.ComponentChangeService.ComponentRemoved += ComponentRemoved;
            designer.ComponentChangeService.ComponentChanged += ComponentChanged;
        }

        private void DesignedForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) designer.DeleteSelected();

            if (e.Control && e.KeyCode == Keys.C) designer.CopyControls();

            if (e.Control && e.KeyCode == Keys.V) designer.PasteControls();

            if (e.Control && e.KeyCode == Keys.Z) designer.Undo();

            if (e.Control && e.KeyCode == Keys.Y) designer.Redo();
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            var selectionService = (ISelectionService) sender;
            var selectionCount = selectionService.SelectionCount;
            EnableAlignResize(selectionCount > 1);
            if (selectionCount == 0)
            {
                propertybox.SetSelectedObjects(designer.DesignedForm);
                return;
            }

            var array = new object[selectionCount];
            selectionService.GetSelectedComponents().CopyTo(array, 0);
            propertybox.SetSelectedObjects(array);
        }

        private void ComponentAdded(object sender, ComponentEventArgs e)
        {
            propertybox.SetComponents(designer.DesignerHost.Container.Components);
            EnableUndoRedo();
        }

        private void ComponentRemoved(object sender, ComponentEventArgs e)
        {
            propertybox.SetComponents(designer.DesignerHost.Container.Components);
            EnableUndoRedo();
        }

        private void ComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            EnableUndoRedo();
        }

        public void SetDesignedForm(Control designedForm, Dictionary<string, IComponent> designedComponents = null)
        {
            designer.DesignedForm = designedForm;
            if (designedComponents != null) designer.DesignedComponents = designedComponents;
        }

        private void EndDesign()
        {
            designer.KeyDown -= DesignedForm_KeyDown;
            designer.SelectionService.SelectionChanged -= SelectionChanged;
            designer.ComponentChangeService.ComponentAdded -= ComponentAdded;
            designer.ComponentChangeService.ComponentRemoved -= ComponentRemoved;
            designer.ComponentChangeService.ComponentChanged -= ComponentChanged;
            EnableAlignResize(false);
            CheckDesignedForm();
            designer.Active = false;
            designer.DesignContainer = null;
            var designEnded = DesignEnded;
            if (designEnded != null) designEnded(this, new EventArgs());

            propertybox.Site = null;
        }

        private void SaveDesignedForm()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML text format (*.xml)|*.xml|Proprietary text format (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.FilterIndex == 1)
            {
                var layoutXML = designer.LayoutXML;
                var streamWriter = new StreamWriter(saveFileDialog.FileName);
                streamWriter.Write((string) layoutXML);
                streamWriter.Close();
            }
        }

        private void CheckDesignedForm()
        {
            if (designer.IsDirty && MessageBox.Show("是否保存对表单的修改?", "确认提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) SaveDesignedForm();
        }

        private void EnableAlignResize(bool enable)
        {
            tbAlignBottom.Enabled = enable;
            tbAlignMiddle.Enabled = enable;
            tbAlignTop.Enabled = enable;
            tbAlignCenter.Enabled = enable;
            tbAlignRight.Enabled = enable;
            tbAlignLeft.Enabled = enable;
            tbSameBoth.Enabled = enable;
            tbSameWidth.Enabled = enable;
            tbSameHeight.Enabled = enable;
        }

        private void EnableUndoRedo()
        {
            tbUndo.Enabled = designer.UndoCount != 0;
            tbRedo.Enabled = designer.RedoCount != 0;
        }

        private void tbSaveForm_Click(object sender, EventArgs e)
        {
            SaveDesignedForm();
        }

        private void tbUndo_Click(object sender, EventArgs e)
        {
            designer.Undo();
            tbUndo.Enabled = designer.UndoCount != 0;
            tbRedo.Enabled = designer.RedoCount != 0;
        }

        private void tbRedo_Click(object sender, EventArgs e)
        {
            designer.Redo();
            tbUndo.Enabled = designer.UndoCount != 0;
            tbRedo.Enabled = designer.RedoCount != 0;
        }

        private void tbAlignLeft_Click(object sender, EventArgs e)
        {
            designer.Align(AlignType.Left);
        }

        private void tbAlignCenter_Click(object sender, EventArgs e)
        {
            designer.Align(AlignType.Center);
        }

        private void tbAlignRight_Click(object sender, EventArgs e)
        {
            designer.Align(AlignType.Right);
        }

        private void tbAlignTop_Click(object sender, EventArgs e)
        {
            designer.Align(AlignType.Top);
        }

        private void tbAlignMiddle_Click(object sender, EventArgs e)
        {
            designer.Align(AlignType.Middle);
        }

        private void tbAlignBottom_Click(object sender, EventArgs e)
        {
            designer.Align(AlignType.Bottom);
        }

        private void tbSameWidth_Click(object sender, EventArgs e)
        {
            designer.MakeSameSize(ResizeType.SameWidth);
        }

        private void tbSameHeight_Click(object sender, EventArgs e)
        {
            designer.MakeSameSize(ResizeType.SameWidth);
        }

        private void tbSameBoth_Click(object sender, EventArgs e)
        {
            designer.MakeSameSize(ResizeType.SameWidth | ResizeType.SameHeight);
        }
    }
}