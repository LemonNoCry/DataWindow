using System;
using System.ComponentModel;
using System.Windows.Forms;
using DataWindow.Core;
using DataWindow.DesignLayer;

namespace DataWindow.Windows
{
    public partial class CustomForm : BaseDataWindow
    {
        // public 设计时事件可选
        public void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Click1");
        }

        public void button1_Click2(object sender, EventArgs e)
        {
            MessageBox.Show("Click2");
        }
    }
}