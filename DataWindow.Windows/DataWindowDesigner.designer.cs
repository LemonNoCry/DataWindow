using DataWindow.Windows.Properties;
using System.ComponentModel;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DataWindow.Windows
{
    public partial class DataWindowDesigner 
    {
        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataWindowDesigner));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.tbNewForm = new System.Windows.Forms.ToolStripButton();
            this.tbOpenForm = new System.Windows.Forms.ToolStripButton();
            this.tbSaveForm = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tbPreview = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tbUndo = new System.Windows.Forms.ToolStripButton();
            this.tbRedo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbLock = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tbDelete = new System.Windows.Forms.ToolStripButton();
            this.tbAlignLeft = new System.Windows.Forms.ToolStripButton();
            this.tbAlignCenter = new System.Windows.Forms.ToolStripButton();
            this.tbAlignRight = new System.Windows.Forms.ToolStripButton();
            this.tbAlignTop = new System.Windows.Forms.ToolStripButton();
            this.tbAlignMiddle = new System.Windows.Forms.ToolStripButton();
            this.tbAlignBottom = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tbSameWidth = new System.Windows.Forms.ToolStripButton();
            this.tbSameHeight = new System.Windows.Forms.ToolStripButton();
            this.tbSameBoth = new System.Windows.Forms.ToolStripButton();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.vS2015BlueTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme();
            this.menuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.miNewForm = new System.Windows.Forms.ToolStripMenuItem();
            this.miOpenForm = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveForm = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.miExitDesigner = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.miUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.miRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem11 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.miAlignTop = new System.Windows.Forms.ToolStripMenuItem();
            this.miAlignMiddle = new System.Windows.Forms.ToolStripMenuItem();
            this.miAlignBottom = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem12 = new System.Windows.Forms.ToolStripSeparator();
            this.miAlignLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.miAlignCenter = new System.Windows.Forms.ToolStripMenuItem();
            this.miAlignRight = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.miSameHeight = new System.Windows.Forms.ToolStripMenuItem();
            this.miSameWidth = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem13 = new System.Windows.Forms.ToolStripSeparator();
            this.miSameBoth = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem14 = new System.Windows.Forms.ToolStripSeparator();
            this.miCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.miPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.miDeleteSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmiResetLayout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiPropertyGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFullProperty = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLocalizationProperty = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiResetDefault = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbNewForm,
            this.tbOpenForm,
            this.tbSaveForm,
            this.toolStripSeparator3,
            this.tbPreview,
            this.toolStripSeparator2,
            this.tbUndo,
            this.tbRedo,
            this.toolStripSeparator1,
            this.tbLock,
            this.toolStripSeparator5,
            this.tbDelete,
            this.tbAlignLeft,
            this.tbAlignCenter,
            this.tbAlignRight,
            this.tbAlignTop,
            this.tbAlignMiddle,
            this.tbAlignBottom,
            this.toolStripSeparator4,
            this.tbSameWidth,
            this.tbSameHeight,
            this.tbSameBoth});
            this.toolStrip.Location = new System.Drawing.Point(0, 25);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1008, 25);
            this.toolStrip.TabIndex = 7;
            this.toolStrip.Text = "toolStrip1";
            // 
            // tbNewForm
            // 
            this.tbNewForm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbNewForm.Image = global::DataWindow.Windows.Properties.Resources.new_form_16x;
            this.tbNewForm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbNewForm.Name = "tbNewForm";
            this.tbNewForm.Size = new System.Drawing.Size(23, 22);
            this.tbNewForm.Text = "新建表单";
            this.tbNewForm.ToolTipText = "新建表单 (Ctrl + Ｎ)";
            this.tbNewForm.Click += new System.EventHandler(this.miNewForm_Click);
            // 
            // tbOpenForm
            // 
            this.tbOpenForm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbOpenForm.Image = global::DataWindow.Windows.Properties.Resources.open_file_16x;
            this.tbOpenForm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbOpenForm.Name = "tbOpenForm";
            this.tbOpenForm.Size = new System.Drawing.Size(23, 22);
            this.tbOpenForm.Text = "打开文件";
            this.tbOpenForm.ToolTipText = "打开文件 (Ctrl + O)";
            this.tbOpenForm.Click += new System.EventHandler(this.miOpenForm_Click);
            // 
            // tbSaveForm
            // 
            this.tbSaveForm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbSaveForm.Enabled = false;
            this.tbSaveForm.Image = global::DataWindow.Windows.Properties.Resources.save_16x;
            this.tbSaveForm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSaveForm.Name = "tbSaveForm";
            this.tbSaveForm.Size = new System.Drawing.Size(23, 22);
            this.tbSaveForm.Text = "保存表单";
            this.tbSaveForm.ToolTipText = "保存表单 (Ctrl + S)";
            this.tbSaveForm.Click += new System.EventHandler(this.miSaveForm_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // tbPreview
            // 
            this.tbPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbPreview.Enabled = false;
            this.tbPreview.Image = global::DataWindow.Windows.Properties.Resources.preview_16x;
            this.tbPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbPreview.Name = "tbPreview";
            this.tbPreview.Size = new System.Drawing.Size(23, 22);
            this.tbPreview.Text = "预览";
            this.tbPreview.ToolTipText = "预览 (F5)";
            this.tbPreview.Click += new System.EventHandler(this.tbPreview_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tbUndo
            // 
            this.tbUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbUndo.Enabled = false;
            this.tbUndo.Image = global::DataWindow.Windows.Properties.Resources.undo_16x;
            this.tbUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbUndo.Name = "tbUndo";
            this.tbUndo.Size = new System.Drawing.Size(23, 22);
            this.tbUndo.Text = "撤销（Ctrl+Z）";
            this.tbUndo.Click += new System.EventHandler(this.miUndo_Click);
            // 
            // tbRedo
            // 
            this.tbRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbRedo.Enabled = false;
            this.tbRedo.Image = global::DataWindow.Windows.Properties.Resources.redo_16x;
            this.tbRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbRedo.Name = "tbRedo";
            this.tbRedo.Size = new System.Drawing.Size(23, 22);
            this.tbRedo.Text = "重做（Ctrl+Y）";
            this.tbRedo.Click += new System.EventHandler(this.miRedo_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tbLock
            // 
            this.tbLock.BackColor = System.Drawing.SystemColors.Control;
            this.tbLock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbLock.Enabled = false;
            this.tbLock.Image = global::DataWindow.Windows.Properties.Resources.lock_16x;
            this.tbLock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbLock.Name = "tbLock";
            this.tbLock.Size = new System.Drawing.Size(23, 22);
            this.tbLock.Text = "删除选中项";
            this.tbLock.ToolTipText = "锁定布局";
            this.tbLock.Click += new System.EventHandler(this.tbLock_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // tbDelete
            // 
            this.tbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbDelete.Enabled = false;
            this.tbDelete.Image = global::DataWindow.Windows.Properties.Resources.delete_16x;
            this.tbDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbDelete.Name = "tbDelete";
            this.tbDelete.Size = new System.Drawing.Size(23, 22);
            this.tbDelete.Text = "删除选中项";
            this.tbDelete.Click += new System.EventHandler(this.tbDelete_Click);
            // 
            // tbAlignLeft
            // 
            this.tbAlignLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbAlignLeft.Enabled = false;
            this.tbAlignLeft.Image = global::DataWindow.Windows.Properties.Resources.align_left_16x;
            this.tbAlignLeft.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbAlignLeft.Name = "tbAlignLeft";
            this.tbAlignLeft.Size = new System.Drawing.Size(23, 22);
            this.tbAlignLeft.Text = "左对齐";
            this.tbAlignLeft.Click += new System.EventHandler(this.miAlignLeft_Click);
            // 
            // tbAlignCenter
            // 
            this.tbAlignCenter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbAlignCenter.Enabled = false;
            this.tbAlignCenter.Image = global::DataWindow.Windows.Properties.Resources.align_center_16x;
            this.tbAlignCenter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbAlignCenter.Name = "tbAlignCenter";
            this.tbAlignCenter.Size = new System.Drawing.Size(23, 22);
            this.tbAlignCenter.Text = "居中对齐";
            this.tbAlignCenter.Click += new System.EventHandler(this.miAlignCenter_Click);
            // 
            // tbAlignRight
            // 
            this.tbAlignRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbAlignRight.Enabled = false;
            this.tbAlignRight.Image = global::DataWindow.Windows.Properties.Resources.align_right_16x;
            this.tbAlignRight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbAlignRight.Name = "tbAlignRight";
            this.tbAlignRight.Size = new System.Drawing.Size(23, 22);
            this.tbAlignRight.Text = "右对齐";
            this.tbAlignRight.Click += new System.EventHandler(this.miAlignRight_Click);
            // 
            // tbAlignTop
            // 
            this.tbAlignTop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbAlignTop.Enabled = false;
            this.tbAlignTop.Image = global::DataWindow.Windows.Properties.Resources.align_top_16x;
            this.tbAlignTop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbAlignTop.Name = "tbAlignTop";
            this.tbAlignTop.Size = new System.Drawing.Size(23, 22);
            this.tbAlignTop.Text = "顶端对齐";
            this.tbAlignTop.Click += new System.EventHandler(this.miAlignTop_Click);
            // 
            // tbAlignMiddle
            // 
            this.tbAlignMiddle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbAlignMiddle.Enabled = false;
            this.tbAlignMiddle.Image = global::DataWindow.Windows.Properties.Resources.align_middlle_16x;
            this.tbAlignMiddle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbAlignMiddle.Name = "tbAlignMiddle";
            this.tbAlignMiddle.Size = new System.Drawing.Size(23, 22);
            this.tbAlignMiddle.Text = "中间对齐";
            this.tbAlignMiddle.Click += new System.EventHandler(this.miAlignMiddle_Click);
            // 
            // tbAlignBottom
            // 
            this.tbAlignBottom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbAlignBottom.Enabled = false;
            this.tbAlignBottom.Image = global::DataWindow.Windows.Properties.Resources.align_bottom_16x;
            this.tbAlignBottom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbAlignBottom.Name = "tbAlignBottom";
            this.tbAlignBottom.Size = new System.Drawing.Size(23, 22);
            this.tbAlignBottom.Text = "底端对齐";
            this.tbAlignBottom.Click += new System.EventHandler(this.miAlignBottom_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // tbSameWidth
            // 
            this.tbSameWidth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbSameWidth.Enabled = false;
            this.tbSameWidth.Image = global::DataWindow.Windows.Properties.Resources.same_width_16x;
            this.tbSameWidth.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSameWidth.Name = "tbSameWidth";
            this.tbSameWidth.Size = new System.Drawing.Size(23, 22);
            this.tbSameWidth.Text = "使宽度相同";
            this.tbSameWidth.Click += new System.EventHandler(this.miSameWidth_Click);
            // 
            // tbSameHeight
            // 
            this.tbSameHeight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbSameHeight.Enabled = false;
            this.tbSameHeight.Image = global::DataWindow.Windows.Properties.Resources.same_height_16x;
            this.tbSameHeight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSameHeight.Name = "tbSameHeight";
            this.tbSameHeight.Size = new System.Drawing.Size(23, 22);
            this.tbSameHeight.Text = "使高度相同";
            this.tbSameHeight.Click += new System.EventHandler(this.miSameHeight_Click);
            // 
            // tbSameBoth
            // 
            this.tbSameBoth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbSameBoth.Enabled = false;
            this.tbSameBoth.Image = global::DataWindow.Windows.Properties.Resources.same_size_16x;
            this.tbSameBoth.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbSameBoth.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSameBoth.Name = "tbSameBoth";
            this.tbSameBoth.Size = new System.Drawing.Size(23, 22);
            this.tbSameBoth.Text = "使大小相同";
            this.tbSameBoth.Click += new System.EventHandler(this.miSameBoth_Click);
            // 
            // dockPanel
            // 
            this.dockPanel.AllowEndUserNestedDocking = false;
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.DockBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(57)))), ((int)(((byte)(85)))));
            this.dockPanel.DockLeftPortion = 200D;
            this.dockPanel.DockRightPortion = 200D;
            this.dockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingSdi;
            this.dockPanel.Location = new System.Drawing.Point(0, 50);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Padding = new System.Windows.Forms.Padding(6);
            this.dockPanel.ShowAutoHideContentOnHover = false;
            this.dockPanel.Size = new System.Drawing.Size(1008, 679);
            this.dockPanel.TabIndex = 8;
            this.dockPanel.Theme = this.vS2015BlueTheme1;
            this.dockPanel.ActiveDocumentChanged += new System.EventHandler(this.dockPanel_ActiveDocumentChanged);
            // 
            // menuItem1
            // 
            this.menuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNewForm,
            this.miOpenForm,
            this.miSaveForm,
            this.menuItem6,
            this.miExitDesigner});
            this.menuItem1.Name = "menuItem1";
            this.menuItem1.Size = new System.Drawing.Size(58, 21);
            this.menuItem1.Text = "文件(&F)";
            // 
            // miNewForm
            // 
            this.miNewForm.Name = "miNewForm";
            this.miNewForm.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.miNewForm.Size = new System.Drawing.Size(180, 22);
            this.miNewForm.Text = "新建(&N)";
            this.miNewForm.Click += new System.EventHandler(this.miNewForm_Click);
            // 
            // miOpenForm
            // 
            this.miOpenForm.Name = "miOpenForm";
            this.miOpenForm.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.miOpenForm.Size = new System.Drawing.Size(180, 22);
            this.miOpenForm.Text = "打开(&O)";
            this.miOpenForm.Click += new System.EventHandler(this.miOpenForm_Click);
            // 
            // miSaveForm
            // 
            this.miSaveForm.Name = "miSaveForm";
            this.miSaveForm.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.miSaveForm.Size = new System.Drawing.Size(180, 22);
            this.miSaveForm.Text = "保存(&S)";
            this.miSaveForm.Click += new System.EventHandler(this.miSaveForm_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Name = "menuItem6";
            this.menuItem6.Size = new System.Drawing.Size(177, 6);
            // 
            // miExitDesigner
            // 
            this.miExitDesigner.Name = "miExitDesigner";
            this.miExitDesigner.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.miExitDesigner.Size = new System.Drawing.Size(180, 22);
            this.miExitDesigner.Text = "退出(&X)";
            this.miExitDesigner.Click += new System.EventHandler(this.miExitDesigner_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miUndo,
            this.miRedo,
            this.menuItem11,
            this.menuItem8,
            this.menuItem9,
            this.menuItem14,
            this.miCopy,
            this.miPaste,
            this.miDeleteSelection,
            this.toolStripMenuItem1,
            this.tsmiResetDefault});
            this.menuItem2.Name = "menuItem2";
            this.menuItem2.Size = new System.Drawing.Size(59, 21);
            this.menuItem2.Text = "编辑(&E)";
            // 
            // miUndo
            // 
            this.miUndo.Name = "miUndo";
            this.miUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.miUndo.Size = new System.Drawing.Size(180, 22);
            this.miUndo.Text = "撤销(&U)";
            this.miUndo.Click += new System.EventHandler(this.miUndo_Click);
            // 
            // miRedo
            // 
            this.miRedo.Name = "miRedo";
            this.miRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.miRedo.Size = new System.Drawing.Size(180, 22);
            this.miRedo.Text = "重做(&R)";
            this.miRedo.Click += new System.EventHandler(this.miRedo_Click);
            // 
            // menuItem11
            // 
            this.menuItem11.Name = "menuItem11";
            this.menuItem11.Size = new System.Drawing.Size(177, 6);
            // 
            // menuItem8
            // 
            this.menuItem8.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAlignTop,
            this.miAlignMiddle,
            this.miAlignBottom,
            this.menuItem12,
            this.miAlignLeft,
            this.miAlignCenter,
            this.miAlignRight});
            this.menuItem8.Name = "menuItem8";
            this.menuItem8.Size = new System.Drawing.Size(180, 22);
            this.menuItem8.Text = "对齐(&A)";
            // 
            // miAlignTop
            // 
            this.miAlignTop.Name = "miAlignTop";
            this.miAlignTop.Size = new System.Drawing.Size(119, 22);
            this.miAlignTop.Text = "&Top";
            this.miAlignTop.Click += new System.EventHandler(this.miAlignTop_Click);
            // 
            // miAlignMiddle
            // 
            this.miAlignMiddle.Name = "miAlignMiddle";
            this.miAlignMiddle.Size = new System.Drawing.Size(119, 22);
            this.miAlignMiddle.Text = "&Middle";
            this.miAlignMiddle.Click += new System.EventHandler(this.miAlignMiddle_Click);
            // 
            // miAlignBottom
            // 
            this.miAlignBottom.Name = "miAlignBottom";
            this.miAlignBottom.Size = new System.Drawing.Size(119, 22);
            this.miAlignBottom.Text = "&Bottom";
            this.miAlignBottom.Click += new System.EventHandler(this.miAlignBottom_Click);
            // 
            // menuItem12
            // 
            this.menuItem12.Name = "menuItem12";
            this.menuItem12.Size = new System.Drawing.Size(116, 6);
            // 
            // miAlignLeft
            // 
            this.miAlignLeft.Name = "miAlignLeft";
            this.miAlignLeft.Size = new System.Drawing.Size(119, 22);
            this.miAlignLeft.Text = "&Left";
            this.miAlignLeft.Click += new System.EventHandler(this.miAlignLeft_Click);
            // 
            // miAlignCenter
            // 
            this.miAlignCenter.Name = "miAlignCenter";
            this.miAlignCenter.Size = new System.Drawing.Size(119, 22);
            this.miAlignCenter.Text = "&Center";
            this.miAlignCenter.Click += new System.EventHandler(this.miAlignCenter_Click);
            // 
            // miAlignRight
            // 
            this.miAlignRight.Name = "miAlignRight";
            this.miAlignRight.Size = new System.Drawing.Size(119, 22);
            this.miAlignRight.Text = "&Right";
            this.miAlignRight.Click += new System.EventHandler(this.miAlignRight_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSameHeight,
            this.miSameWidth,
            this.menuItem13,
            this.miSameBoth});
            this.menuItem9.Name = "menuItem9";
            this.menuItem9.Size = new System.Drawing.Size(180, 22);
            this.menuItem9.Text = "使用相同(&M)";
            // 
            // miSameHeight
            // 
            this.miSameHeight.Name = "miSameHeight";
            this.miSameHeight.Size = new System.Drawing.Size(206, 22);
            this.miSameHeight.Text = "相同高度(Same &Height)";
            this.miSameHeight.Click += new System.EventHandler(this.miSameHeight_Click);
            // 
            // miSameWidth
            // 
            this.miSameWidth.Name = "miSameWidth";
            this.miSameWidth.Size = new System.Drawing.Size(206, 22);
            this.miSameWidth.Text = "相同宽度(Same &Width)";
            this.miSameWidth.Click += new System.EventHandler(this.miSameWidth_Click);
            // 
            // menuItem13
            // 
            this.menuItem13.Name = "menuItem13";
            this.menuItem13.Size = new System.Drawing.Size(203, 6);
            // 
            // miSameBoth
            // 
            this.miSameBoth.Name = "miSameBoth";
            this.miSameBoth.Size = new System.Drawing.Size(206, 22);
            this.miSameBoth.Text = "相同大小(Same &Both)";
            this.miSameBoth.Click += new System.EventHandler(this.miSameBoth_Click);
            // 
            // menuItem14
            // 
            this.menuItem14.Name = "menuItem14";
            this.menuItem14.Size = new System.Drawing.Size(177, 6);
            // 
            // miCopy
            // 
            this.miCopy.Name = "miCopy";
            this.miCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.miCopy.Size = new System.Drawing.Size(180, 22);
            this.miCopy.Text = "复制(&C)";
            this.miCopy.Click += new System.EventHandler(this.miCopy_Click);
            // 
            // miPaste
            // 
            this.miPaste.Name = "miPaste";
            this.miPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.miPaste.Size = new System.Drawing.Size(180, 22);
            this.miPaste.Text = "粘贴(&P)";
            this.miPaste.Click += new System.EventHandler(this.miPaste_Click);
            // 
            // miDeleteSelection
            // 
            this.miDeleteSelection.Name = "miDeleteSelection";
            this.miDeleteSelection.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.miDeleteSelection.Size = new System.Drawing.Size(180, 22);
            this.miDeleteSelection.Text = "删除(&D)";
            this.miDeleteSelection.Click += new System.EventHandler(this.miDeleteSelection_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem1,
            this.menuItem2,
            this.menuItem4});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 25);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmiResetLayout
            // 
            this.tsmiResetLayout.Name = "tsmiResetLayout";
            this.tsmiResetLayout.Size = new System.Drawing.Size(180, 22);
            this.tsmiResetLayout.Text = "重置布局";
            this.tsmiResetLayout.Click += new System.EventHandler(this.tsmiResetLayout_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(177, 6);
            // 
            // tsmiPropertyGrid
            // 
            this.tsmiPropertyGrid.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFullProperty,
            this.tsmiLocalizationProperty});
            this.tsmiPropertyGrid.Name = "tsmiPropertyGrid";
            this.tsmiPropertyGrid.Size = new System.Drawing.Size(180, 22);
            this.tsmiPropertyGrid.Text = "属性网格";
            // 
            // tsmiFullProperty
            // 
            this.tsmiFullProperty.Name = "tsmiFullProperty";
            this.tsmiFullProperty.Size = new System.Drawing.Size(180, 22);
            this.tsmiFullProperty.Text = "显示完整属性";
            this.tsmiFullProperty.Click += new System.EventHandler(this.tsmiFullProperty_Click);
            // 
            // tsmiLocalizationProperty
            // 
            this.tsmiLocalizationProperty.Name = "tsmiLocalizationProperty";
            this.tsmiLocalizationProperty.Size = new System.Drawing.Size(180, 22);
            this.tsmiLocalizationProperty.Text = "显示本地化属性";
            this.tsmiLocalizationProperty.Click += new System.EventHandler(this.tsmiLocalizationProperty_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiResetLayout,
            this.toolStripMenuItem2,
            this.tsmiPropertyGrid});
            this.menuItem4.Name = "menuItem4";
            this.menuItem4.Size = new System.Drawing.Size(60, 21);
            this.menuItem4.Text = "布局(&V)";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
            // 
            // tsmiResetDefault
            // 
            this.tsmiResetDefault.Name = "tsmiResetDefault";
            this.tsmiResetDefault.Size = new System.Drawing.Size(180, 22);
            this.tsmiResetDefault.Text = "重置默认界面";
            this.tsmiResetDefault.Click += new System.EventHandler(this.tsmiResetDefault_Click);
            // 
            // DataWindowDesigner
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DataWindowDesigner";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "表单设计器";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private IContainer components;

        private ToolStrip toolStrip;
        private ToolStripButton tbNewForm;
        private ToolStripButton tbOpenForm;
        private ToolStripButton tbSaveForm;
        private ToolStripButton tbUndo;
        private ToolStripButton tbRedo;
        private ToolStripButton tbAlignLeft;
        private ToolStripButton tbAlignCenter;
        private ToolStripButton tbAlignRight;
        private ToolStripButton tbAlignTop;
        private ToolStripButton tbAlignMiddle;
        private ToolStripButton tbAlignBottom;
        private ToolStripButton tbSameWidth;
        private ToolStripButton tbSameHeight;
        private ToolStripButton tbSameBoth;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator4;

        private DockPanel dockPanel;

        #endregion

        private VS2015BlueTheme vS2015BlueTheme1;
        private ToolStripMenuItem menuItem1;
        private ToolStripMenuItem miNewForm;
        private ToolStripMenuItem miOpenForm;
        private ToolStripMenuItem miSaveForm;
        private ToolStripSeparator menuItem6;
        private ToolStripMenuItem miExitDesigner;
        private ToolStripMenuItem menuItem2;
        private ToolStripMenuItem miUndo;
        private ToolStripMenuItem miRedo;
        private ToolStripSeparator menuItem11;
        private ToolStripMenuItem menuItem8;
        private ToolStripMenuItem miAlignTop;
        private ToolStripMenuItem miAlignMiddle;
        private ToolStripMenuItem miAlignBottom;
        private ToolStripMenuItem miAlignLeft;
        private ToolStripMenuItem miAlignCenter;
        private ToolStripMenuItem miAlignRight;
        private ToolStripMenuItem menuItem9;
        private ToolStripMenuItem miSameHeight;
        private ToolStripMenuItem miSameWidth;
        private ToolStripMenuItem miSameBoth;
        private ToolStripSeparator menuItem14;
        private ToolStripMenuItem miCopy;
        private ToolStripMenuItem miPaste;
        private ToolStripMenuItem miDeleteSelection;
        private MenuStrip menuStrip1;
        private ToolStripSeparator menuItem12;
        private ToolStripButton tbLock;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripSeparator menuItem13;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem tsmiResetDefault;
        private ToolStripMenuItem menuItem4;
        private ToolStripMenuItem tsmiResetLayout;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem tsmiPropertyGrid;
        private ToolStripMenuItem tsmiFullProperty;
        private ToolStripMenuItem tsmiLocalizationProperty;
    }
}