using System;
using System.ComponentModel;
using System.Windows.Forms;
using DataWindow.Core;
using DataWindow.DesignLayer;

namespace DataWindow.Windows
{
    public partial class CustomForm : BaseDataWindow
    {
        public CustomForm()
        {
            InitializeComponent();
        }

        private Button button1;

        // public 设计时事件可选
        public void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Click1");
        }

        public void button1_Click2(object sender, EventArgs e)
        {
            MessageBox.Show("Click2");
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(35, 30);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // CustomForm
            // 
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.button1);
            this.Name = "CustomForm";
            this.ResumeLayout(false);

        }
    }
}