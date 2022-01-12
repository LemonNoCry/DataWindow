using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DataWindow.Serialization.Components;

namespace DataWindow.DesignLayer
{
    public class DesignerControl : System.Windows.Forms.UserControl
    {
        private IContainer components;

        public DefaultDesignerLoader defaultDesignerLoader;

        public Designer Designer;

        public DesignerControl()
        {
            InitializeComponent();
        }

        public DesignerControl(Control root) : this()
        {
            DesignedForm = root;
        }

        public DesignerControl(BaseDataWindow root) : this()
        {
            defaultDesignerLoader = root.defaultDesignerLoader;
            Designer = root.designer;
            DesignedForm = root;
        }

        public DesignerControl(Control root, string layoutXml) : this()
        {
            DesignedForm = root;
            Designer.LayoutXml = layoutXml;
        }

        public Control DesignedForm
        {
            get => Designer.DesignedForm;
            private set => Designer.DesignedForm = value;
        }

        private void DesignerControl_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            Designer.DesignContainer = this;
            Dock = DockStyle.Fill;
            if (DesignedForm != null) Designer.Active = true;
        }

        public void button1_Click(object sender, EventArgs e)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            defaultDesignerLoader = new DefaultDesignerLoader();
            Designer = new Designer();
            SuspendLayout();
            Designer.DesignerLoader = defaultDesignerLoader;
            Designer.GridSize = new Size(8, 8);
            AutoScaleDimensions = new SizeF(6f, 12f);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Name = "DesignerControl";
            Size = new Size(339, 374);
            Load += DesignerControl_Load;
            ResumeLayout(false);
        }
    }
}