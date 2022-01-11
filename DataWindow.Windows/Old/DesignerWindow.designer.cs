using DataWindow.Windows.Properties;

namespace DataWindow.Windows.Old
{
		public partial class DesignerWindow : global::System.Windows.Forms.Form
	{
				private void InitializeComponent()
		{
			this.splitter1 = new global::System.Windows.Forms.Splitter();
			this.splitter2 = new global::System.Windows.Forms.Splitter();
			this.toolStrip = new global::System.Windows.Forms.ToolStrip();
			this.tbSaveForm = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new global::System.Windows.Forms.ToolStripSeparator();
			this.tbUndo = new global::System.Windows.Forms.ToolStripButton();
			this.tbRedo = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new global::System.Windows.Forms.ToolStripSeparator();
			this.tbAlignLeft = new global::System.Windows.Forms.ToolStripButton();
			this.tbAlignCenter = new global::System.Windows.Forms.ToolStripButton();
			this.tbAlignRight = new global::System.Windows.Forms.ToolStripButton();
			this.tbAlignTop = new global::System.Windows.Forms.ToolStripButton();
			this.tbAlignMiddle = new global::System.Windows.Forms.ToolStripButton();
			this.tbAlignBottom = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new global::System.Windows.Forms.ToolStripSeparator();
			this.tbSameWidth = new global::System.Windows.Forms.ToolStripButton();
			this.tbSameHeight = new global::System.Windows.Forms.ToolStripButton();
			this.tbSameBoth = new global::System.Windows.Forms.ToolStripButton();
			this.toolboxControl = new global::DataWindow.DesignLayer.ToolboxControl();
			this.propertybox = new global::DataWindow.DesignLayer.PropertyboxControl();
			this.designerControl1 = new global::DataWindow.DesignLayer.DesignerControl();
			this.toolStrip.SuspendLayout();
			base.SuspendLayout();
			this.splitter1.Location = new global::System.Drawing.Point(180, 25);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new global::System.Drawing.Size(3, 536);
			this.splitter1.TabIndex = 4;
			this.splitter1.TabStop = false;
			this.splitter2.Dock = global::System.Windows.Forms.DockStyle.Right;
			this.splitter2.Location = new global::System.Drawing.Point(681, 25);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new global::System.Drawing.Size(3, 536);
			this.splitter2.TabIndex = 5;
			this.splitter2.TabStop = false;
			this.toolStrip.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.tbSaveForm,
				this.toolStripSeparator2,
				this.tbUndo,
				this.tbRedo,
				this.toolStripSeparator1,
				this.tbAlignLeft,
				this.tbAlignCenter,
				this.tbAlignRight,
				this.tbAlignTop,
				this.tbAlignMiddle,
				this.tbAlignBottom,
				this.toolStripSeparator4,
				this.tbSameWidth,
				this.tbSameHeight,
				this.tbSameBoth
			});
			this.toolStrip.Location = new global::System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new global::System.Drawing.Size(944, 25);
			this.toolStrip.TabIndex = 9;
			this.toolStrip.Text = "toolStrip1";
			this.tbSaveForm.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbSaveForm.Image = Resources.save_16x;
			this.tbSaveForm.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.tbSaveForm.Name = "tbSaveForm";
			this.tbSaveForm.Size = new global::System.Drawing.Size(23, 22);
			this.tbSaveForm.Text = "保存表单";
			this.tbSaveForm.Click += new global::System.EventHandler(this.tbSaveForm_Click);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new global::System.Drawing.Size(6, 25);
			this.tbUndo.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbUndo.Image = Resources.undo_16x;
			this.tbUndo.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.tbUndo.Name = "tbUndo";
			this.tbUndo.Size = new global::System.Drawing.Size(23, 22);
			this.tbUndo.Text = "撤销（Ctrl+Z）";
			this.tbUndo.Click += new global::System.EventHandler(this.tbUndo_Click);
			this.tbRedo.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbRedo.Image = global::DataWindow.Windows.Properties.Resources.redo_16x;
			this.tbRedo.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.tbRedo.Name = "tbRedo";
			this.tbRedo.Size = new global::System.Drawing.Size(23, 22);
			this.tbRedo.Text = "重做（Ctrl+Y）";
			this.tbRedo.Click += new global::System.EventHandler(this.tbRedo_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new global::System.Drawing.Size(6, 25);
			this.tbAlignLeft.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbAlignLeft.Image = global::DataWindow.Windows.Properties.Resources.align_left_16x;
			this.tbAlignLeft.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.tbAlignLeft.Name = "tbAlignLeft";
			this.tbAlignLeft.Size = new global::System.Drawing.Size(23, 22);
			this.tbAlignLeft.Text = "左对齐";
			this.tbAlignLeft.Click += new global::System.EventHandler(this.tbAlignLeft_Click);
			this.tbAlignCenter.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbAlignCenter.Image = global::DataWindow.Windows.Properties.Resources.align_center_16x;
			this.tbAlignCenter.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.tbAlignCenter.Name = "tbAlignCenter";
			this.tbAlignCenter.Size = new global::System.Drawing.Size(23, 22);
			this.tbAlignCenter.Text = "居中对齐";
			this.tbAlignCenter.Click += new global::System.EventHandler(this.tbAlignCenter_Click);
			this.tbAlignRight.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbAlignRight.Image = global::DataWindow.Windows.Properties.Resources.align_right_16x;
			this.tbAlignRight.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.tbAlignRight.Name = "tbAlignRight";
			this.tbAlignRight.Size = new global::System.Drawing.Size(23, 22);
			this.tbAlignRight.Text = "右对齐";
			this.tbAlignRight.Click += new global::System.EventHandler(this.tbAlignRight_Click);
			this.tbAlignTop.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbAlignTop.Image = global::DataWindow.Windows.Properties.Resources.align_top_16x;
			this.tbAlignTop.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.tbAlignTop.Name = "tbAlignTop";
			this.tbAlignTop.Size = new global::System.Drawing.Size(23, 22);
			this.tbAlignTop.Text = "顶端对齐";
			this.tbAlignTop.Click += new global::System.EventHandler(this.tbAlignTop_Click);
			this.tbAlignMiddle.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbAlignMiddle.Image = global::DataWindow.Windows.Properties.Resources.align_middle_16x;
			this.tbAlignMiddle.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.tbAlignMiddle.Name = "tbAlignMiddle";
			this.tbAlignMiddle.Size = new global::System.Drawing.Size(23, 22);
			this.tbAlignMiddle.Text = "中间对齐";
			this.tbAlignMiddle.Click += new global::System.EventHandler(this.tbAlignMiddle_Click);
			this.tbAlignBottom.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbAlignBottom.Image = global::DataWindow.Windows.Properties.Resources.align_bottom_16x;
			this.tbAlignBottom.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.tbAlignBottom.Name = "tbAlignBottom";
			this.tbAlignBottom.Size = new global::System.Drawing.Size(23, 22);
			this.tbAlignBottom.Text = "底端对齐";
			this.tbAlignBottom.Click += new global::System.EventHandler(this.tbAlignBottom_Click);
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new global::System.Drawing.Size(6, 25);
			this.tbSameWidth.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbSameWidth.Image = global::DataWindow.Windows.Properties.Resources.same_width_16x;
			this.tbSameWidth.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.tbSameWidth.Name = "tbSameWidth";
			this.tbSameWidth.Size = new global::System.Drawing.Size(23, 22);
			this.tbSameWidth.Text = "使宽度相同";
			this.tbSameWidth.Click += new global::System.EventHandler(this.tbSameWidth_Click);
			this.tbSameHeight.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbSameHeight.Image = Resources.same_height_16x;
			this.tbSameHeight.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.tbSameHeight.Name = "tbSameHeight";
			this.tbSameHeight.Size = new global::System.Drawing.Size(23, 22);
			this.tbSameHeight.Text = "使高度相同";
			this.tbSameHeight.Click += new global::System.EventHandler(this.tbSameHeight_Click);
			this.tbSameBoth.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbSameBoth.Image = global::DataWindow.Windows.Properties.Resources.same_size_16x;
			this.tbSameBoth.ImageScaling = global::System.Windows.Forms.ToolStripItemImageScaling.None;
			this.tbSameBoth.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.tbSameBoth.Name = "tbSameBoth";
			this.tbSameBoth.Size = new global::System.Drawing.Size(23, 22);
			this.tbSameBoth.Text = "使大小相同";
			this.tbSameBoth.Click += new global::System.EventHandler(this.tbSameBoth_Click);
			this.toolboxControl.BackColor = global::System.Drawing.SystemColors.Control;
			this.toolboxControl.Designer = null;
			this.toolboxControl.Dock = global::System.Windows.Forms.DockStyle.Left;
			this.toolboxControl.Location = new global::System.Drawing.Point(0, 25);
			this.toolboxControl.Name = "toolboxControl";
			this.toolboxControl.Size = new global::System.Drawing.Size(180, 536);
			this.toolboxControl.TabIndex = 2;
			this.propertybox.Designer = null;
			this.propertybox.Dock = global::System.Windows.Forms.DockStyle.Right;
			this.propertybox.Location = new global::System.Drawing.Point(684, 25);
			this.propertybox.Name = "propertybox";
			this.propertybox.Size = new global::System.Drawing.Size(260, 536);
			this.propertybox.TabIndex = 7;
			this.designerControl1.BackColor = global::System.Drawing.Color.White;
			this.designerControl1.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.designerControl1.Location = new global::System.Drawing.Point(183, 25);
			this.designerControl1.Name = "designerControl1";
			this.designerControl1.Size = new global::System.Drawing.Size(498, 536);
			this.designerControl1.TabIndex = 10;
			this.AutoScaleBaseSize = new global::System.Drawing.Size(5, 13);
			base.ClientSize = new global::System.Drawing.Size(944, 561);
			base.Controls.Add(this.designerControl1);
			base.Controls.Add(this.splitter2);
			base.Controls.Add(this.splitter1);
			base.Controls.Add(this.toolboxControl);
			base.Controls.Add(this.propertybox);
			base.Controls.Add(this.toolStrip);
			this.Font = new global::System.Drawing.Font("Tahoma", 8f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 204);
			base.Name = "DesignerWindow";
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "表单设计";
			base.Load += new global::System.EventHandler(this.DesignerWindow_Load);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

				private global::DataWindow.DesignLayer.ToolboxControl toolboxControl;

				private global::System.Windows.Forms.Splitter splitter1;

				private global::DataWindow.DesignLayer.PropertyboxControl propertybox;

				private global::System.Windows.Forms.ToolStrip toolStrip;

				private global::System.Windows.Forms.ToolStripButton tbSaveForm;

				private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator2;

				private global::System.Windows.Forms.ToolStripButton tbUndo;

				private global::System.Windows.Forms.ToolStripButton tbRedo;

				private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

				private global::System.Windows.Forms.ToolStripButton tbAlignLeft;

				private global::System.Windows.Forms.ToolStripButton tbAlignCenter;

				private global::System.Windows.Forms.ToolStripButton tbAlignRight;

				private global::System.Windows.Forms.ToolStripButton tbAlignTop;

				private global::System.Windows.Forms.ToolStripButton tbAlignMiddle;

				private global::System.Windows.Forms.ToolStripButton tbAlignBottom;

				private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator4;

				private global::System.Windows.Forms.ToolStripButton tbSameWidth;

				private global::System.Windows.Forms.ToolStripButton tbSameHeight;

				private global::System.Windows.Forms.ToolStripButton tbSameBoth;

				private global::DataWindow.DesignLayer.DesignerControl designerControl1;

				private global::System.Windows.Forms.Splitter splitter2;
	}
}
