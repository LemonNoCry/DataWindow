using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DataWindow.Windows
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dwd = DataWindowDesigner.DesignerLayout(this.customUserControl1);
            dwd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(dwd.LayoutXml))
            {
                this.customUserControl1.designer.LayoutXml = dwd.LayoutXml;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.customUserControl1.designer.LayoutXml);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.customUserControl1.designer.LayoutXml = textBox1.Text;
        }
    }
}