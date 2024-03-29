﻿using System;
using System.Windows.Forms;
using DataWindow.DesignLayer;
using WeifenLuo.WinFormsUI.Docking;

namespace DataWindow.Windows.Dock
{
    public class DesignerDocument : DockContent
    {
        public DesignerControl DesignerControl { get; private set; }

        public Designer Designer
        {
            get { return this.DesignerControl.Designer; }
        }

        public Type RootComponentType { get; }

        public DesignerDocument(string text, Type rootType)
        {
            this.Text = text;
            this.RootComponentType = rootType;
            InitializeDesigner();
        }

        public DesignerDocument(Control form)
        {
            this.Text = form.Text;
            this.RootComponentType = form.GetType();
            InitializeDesigner(form);
        }

        private void InitializeDesigner(Control root = null)
        {
            if (root == null)
            {
                root = (Control) Activator.CreateInstance(RootComponentType);
                root.Name = RootComponentType.Name;
            }

            if (root is BaseDataWindow bdw)
            {
                this.DesignerControl = new DesignerControl(bdw) {Dock = DockStyle.Fill};
            }
            else
            {
                this.DesignerControl = new DesignerControl(root) {Dock = DockStyle.Fill};
            }

            this.DesignerControl.Designer.KeyDown += DesignedForm_KeyDown;
            this.Controls.Add(DesignerControl);
        }


        public void Preview()
        {
            var form = new CustomForm();
            form.SetLayoutXml(this.DesignerControl.Designer.LayoutXml);
            form.ShowDialog();
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            (sender as Form).Hide();
        }

        private void DesignedForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                this.Designer.DeleteSelected();
            else if (e.KeyCode == Keys.F5)
                this.Preview();
            else if (e.Control == true && e.KeyCode == Keys.A)
                this.Designer.SelectAll();
            else if (e.Control == true && e.KeyCode == Keys.C)
                this.Designer.CopyControls();
            else if (e.Control == true && e.KeyCode == Keys.V)
                this.Designer.PasteControls();
            else if (e.Control == true && e.KeyCode == Keys.Z)
                this.Designer.Undo();
            else if (e.Control == true && e.KeyCode == Keys.Y)
                this.Designer.Redo();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            this.Designer.KeyDown -= DesignedForm_KeyDown;
            base.OnFormClosed(e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DesignerDocument
            // 
            this.AllowEndUserDocking = false;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "DesignerDocument";
            this.ResumeLayout(false);
        }
    }
}