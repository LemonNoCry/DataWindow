using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DataWindow.DesignerInternal;
using DataWindow.DesignLayer;
using DataWindow.Windows.Dock;
using WeifenLuo.WinFormsUI.Docking;

namespace DataWindow.Windows
{
    public partial class DataWindowDesigner : Form
    {
        private ToolboxWindow toolboxWindow;
        private PropertyWindow propertyWindow;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton tbPreview;
        private ToolStripButton tbDelete;
        private Designer activeDesigner;

        /// <summary>
        /// 是否可以多窗口
        /// </summary>
        public bool IsMultiple { get; set; } = false;

        public DataWindowDesigner()
        {
            InitializeComponent();

            if (IsMultiple)
            {
                this.dockPanel.DocumentStyle = DocumentStyle.DockingWindow;
            }

            this.toolboxWindow = new ToolboxWindow();
            this.toolboxWindow.Show(this.dockPanel, DockState.DockLeft);

            this.propertyWindow = new PropertyWindow();
            this.propertyWindow.Show(this.toolboxWindow.Pane, DockAlignment.Bottom, 0.5);

            EnableUndoRedo();
        }

        public static void DesignerLayout(Control control)
        {
            DataWindowDesigner dwd = new DataWindowDesigner();
            dwd.NewDesignedForm(control);
            dwd.ShowDialog();
        }


        private void OnlyOrMultipleOpen()
        {
            if (!IsMultiple)
            {
                IDockContent dockContents;
                while ((dockContents = dockPanel.Contents.FirstOrDefault(s => s is DesignerDocument)) != null)
                {
                    dockContents.DockHandler.Close();
                }
            }
        }

        private void NewDesignedForm()
        {
            OnlyOrMultipleOpen();
            string name = "from " + (this.dockPanel.DocumentsCount + 1);
            var rootType = typeof(CustomForm);

            var doc = new DesignerDocument(name, rootType);
            NewDesignerDocument(doc);
        }

        private void NewDesignedForm(Control control)
        {
            OnlyOrMultipleOpen();
            var doc = new DesignerDocument(control);
            NewDesignerDocument(doc);
        }

        private void NewDesignerDocument(DesignerDocument doc)
        {
            this.activeDesigner = doc.Designer;
            doc.FormClosing += (s, e) => { EndDesign(doc.Designer); };
            doc.Designer.DesignEvents.AddingVerb += DesignEvents_AddingVerb;
            doc.Designer.SelectionService.SelectionChanged += SelectionChanged;
            doc.Designer.ComponentChangeService.ComponentAdded += ComponentAdded;
            doc.Designer.ComponentChangeService.ComponentRemoved += ComponentRemoved;
            doc.Designer.ComponentChangeService.ComponentChanged += ComponentChanged;
            doc.AllowEndUserDocking = false;
            doc.Show(dockPanel, DockState.Document);

            tbSaveForm.Enabled = true;
        }

        private void OpenDesignedForm()
        {
            var openFileName = new OpenFileDialog();

            openFileName.Filter = @"XML text format (*.xml)|*.xml|Proprietary text format (*.*)|*.*";
            openFileName.FilterIndex = 1;
            openFileName.RestoreDirectory = true;

            if (openFileName.ShowDialog() == DialogResult.OK)
            {
                this.NewDesignedForm();

                if (openFileName.FilterIndex == 1)
                {
                    var txtReader = new StreamReader(openFileName.FileName);
                    string layoutString = txtReader.ReadToEnd();
                    txtReader.Close();

                    this.activeDesigner.InitLayout(layoutString);
                }
                else
                {
                    this.activeDesigner.LoadFromFile(openFileName.FileName);
                }

                tbSaveForm.Enabled = true;
            }
        }

        private void SaveDesignedForm()
        {
            var saveFileName = new SaveFileDialog();
            saveFileName.Filter = "XML Form (*.xml)|*.xml";
            saveFileName.FilterIndex = 1;
            saveFileName.RestoreDirectory = true;

            if (saveFileName.ShowDialog() == DialogResult.OK)
            {
                string test = this.activeDesigner.LayoutXML;

                TextWriter txtWriter = new StreamWriter(saveFileName.FileName);
                txtWriter.Write(test);
                txtWriter.Close();
            }
        }

        private void CheckDesignedForm()
        {
            if (this.activeDesigner.IsDirty == true)
            {
                if (MessageBox.Show("是否保存对表单的修改?", "确认提示",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SaveDesignedForm();
                }
            }
        }

        private void EndDesign(Designer designer)
        {
            if (designer == null) return;

            designer.SelectionService.SelectionChanged -= SelectionChanged;
            designer.ComponentChangeService.ComponentAdded -= ComponentAdded;
            designer.ComponentChangeService.ComponentRemoved -= ComponentRemoved;
            designer.ComponentChangeService.ComponentChanged -= ComponentChanged;
            CheckDesignedForm();
            designer.Active = false;
            designer.DesignContainer = null;
        }

        private void EnableFeatures(bool enable)
        {
            this.tbLock.Enabled = enable;
        }


        private void EnableAlignResize(bool enable)
        {
            this.miAlignBottom.Enabled = enable;
            this.miAlignMiddle.Enabled = enable;
            this.miAlignTop.Enabled = enable;
            this.miAlignCenter.Enabled = enable;
            this.miAlignRight.Enabled = enable;
            this.miAlignLeft.Enabled = enable;

            this.tbAlignBottom.Enabled = enable;
            this.tbAlignMiddle.Enabled = enable;
            this.tbAlignTop.Enabled = enable;
            this.tbAlignCenter.Enabled = enable;
            this.tbAlignLeft.Enabled = enable;
            this.tbAlignRight.Enabled = enable;

            this.miSameBoth.Enabled = enable;
            this.miSameWidth.Enabled = enable;
            this.miSameHeight.Enabled = enable;

            this.tbSameBoth.Enabled = enable;
            this.tbSameWidth.Enabled = enable;
            this.tbSameHeight.Enabled = enable;
        }

        private void EnableUndoRedo()
        {
            miUndo.Enabled = (this.activeDesigner?.UndoCount > 0);
            miRedo.Enabled = (this.activeDesigner?.RedoCount > 0);

            tbUndo.Enabled = (this.activeDesigner?.UndoCount > 0);
            tbRedo.Enabled = (this.activeDesigner?.RedoCount > 0);
        }

        #region 内部事件

        private void dockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            if (this.dockPanel.ActiveDocument is DesignerDocument doc)
            {
                this.activeDesigner = doc.Designer;
                this.SelectionChanged(this.activeDesigner.SelectionService, EventArgs.Empty);

                this.toolboxWindow.Toolbox.Designer = this.activeDesigner;

                this.propertyWindow.Propertybox.SetComponents(this.activeDesigner.DesignerHost.Container.Components);

                this.EnableUndoRedo();

                this.tbPreview.Enabled = true;
                this.tbSaveForm.Enabled = true;
            }
            else
            {
                this.tbPreview.Enabled = false;
                this.tbSaveForm.Enabled = false;
            }
        }

        private bool DesignEvents_AddingVerb(IComponent primarySelection, DesignerVerb verb)
        {
            return true;
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            var selectionService = (ISelectionService) sender;
            int selectionCount = selectionService.SelectionCount;

            EnableFeatures(selectionCount > 0);
            EnableAlignResize(selectionCount > 1);
            if (selectionCount >= 1)
            {
                this.miDeleteSelection.Enabled = true;
                this.miCopy.Enabled = true;
                this.tbDelete.Enabled = true;
            }
            else
            {
                this.miDeleteSelection.Enabled = false;
                this.miCopy.Enabled = false;
                this.tbDelete.Enabled = false;
            }

            this.propertyWindow.Propertybox.Designer = this.activeDesigner;
            if (selectionCount == 0)
            {
                this.propertyWindow.Propertybox.SetSelectedObjects(this.activeDesigner.DesignedForm);
            }
            else
            {
                var selected = new object[selectionCount];
                selectionService.GetSelectedComponents().CopyTo(selected, 0);
                this.propertyWindow.Propertybox.SetSelectedObjects(selected);
            }
        }

        private void ComponentAdded(object sender, ComponentEventArgs e)
        {
            this.propertyWindow.Propertybox.SetComponents(this.activeDesigner.DesignerHost.Container.Components);

            EnableUndoRedo();
        }

        private void ComponentRemoved(object sender, ComponentEventArgs e)
        {
            this.propertyWindow.Propertybox.SetComponents(this.activeDesigner.DesignerHost.Container.Components);

            EnableUndoRedo();
        }

        private void ComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            EnableUndoRedo();
        }

        #endregion

        #region 文件菜单

        private void miNewForm_Click(object sender, EventArgs e)
        {
            NewDesignedForm();
        }

        private void miOpenForm_Click(object sender, System.EventArgs e)
        {
            OpenDesignedForm();
        }

        private void miSaveForm_Click(object sender, System.EventArgs e)
        {
            SaveDesignedForm();
        }

        private void miExitDesigner_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region 编辑菜单

        private void tbPreview_Click(object sender, EventArgs e)
        {
            if (this.dockPanel.ActiveDocument is DesignerDocument doc)
            {
                doc.Preview();
            }
        }

        private void tbDelete_Click(object sender, EventArgs e)
        {
            if (this.dockPanel.ActiveDocument is DesignerDocument doc)
            {
                doc.Designer.DeleteSelected();
            }
        }

        private void miAlignTop_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.Align(AlignType.Top);
        }

        private void miAlignMiddle_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.Align(AlignType.Middle);
        }

        private void miAlignBottom_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.Align(AlignType.Bottom);
        }

        private void miAlignLeft_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.Align(AlignType.Left);
        }

        private void miAlignCenter_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.Align(AlignType.Center);
        }

        private void miAlignRight_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.Align(AlignType.Right);
        }

        private void miSameHeight_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.MakeSameSize(ResizeType.SameHeight);
        }

        private void miSameWidth_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.MakeSameSize(ResizeType.SameWidth);
        }

        private void miSameBoth_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.MakeSameSize(ResizeType.SameHeight | ResizeType.SameWidth);
        }

        private void miUndo_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.Undo();
            miUndo.Enabled = (this.activeDesigner.UndoCount != 0);
            miRedo.Enabled = (this.activeDesigner.RedoCount != 0);
        }

        private void miRedo_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.Redo();
            miUndo.Enabled = (this.activeDesigner.UndoCount != 0);
            miRedo.Enabled = (this.activeDesigner.RedoCount != 0);
        }

        private void miDeleteSelection_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.DeleteSelected();
        }

        private void miCopy_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.CopyControls();
        }

        private void miPaste_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.PasteControls();
        }

        private void tbLock_Click(object sender, EventArgs e)
        {
            this.activeDesigner.Lock();
        }

        #endregion

        #region 布局菜单

        private void tsmiResetLayout_Click(object sender, EventArgs e)
        {
            this.toolboxWindow.Show(this.dockPanel, DockState.DockLeft);
            this.propertyWindow.Show(this.toolboxWindow.Pane, DockAlignment.Bottom, 0.5);
        }

        private void tsmiFullProperty_Click(object sender, EventArgs e)
        {
            this.propertyWindow.Propertybox.propertyGrid.DisplayMode = localizationPropertyGrid.DisplayModeEnum.None;
            this.SelectionChanged(this.activeDesigner.SelectionService, EventArgs.Empty);
        }

        private void tsmiLocalizationProperty_Click(object sender, EventArgs e)
        {
            this.propertyWindow.Propertybox.propertyGrid.DisplayMode = localizationPropertyGrid.DisplayModeEnum.ForNormalUser;
            this.SelectionChanged(this.activeDesigner.SelectionService, EventArgs.Empty);
        }

        #endregion

        #region 工具按钮菜单

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            this.toolboxWindow.Dispose();
            this.propertyWindow.Dispose();

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}