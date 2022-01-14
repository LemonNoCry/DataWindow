using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DataWindow.Core;
using DataWindow.DesignLayer;

namespace DataWindow.Windows
{
    public partial class CustomForm : BaseDataWindow
    {
        private Button btnLogin;
        private TextBox tbAccount;
        private Label lblAccount;

        public CustomForm()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            AddInherentControls();
            AddMustControls(btnLogin);
            AddProhibitEditControls(tbAccount);

            AddControlTranslation(new Dictionary<Control, string>()
            {
                {tbAccount, "账号输入框"}
            });
        }

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
            this.btnLogin = new System.Windows.Forms.Button();
            this.tbAccount = new System.Windows.Forms.TextBox();
            this.lblAccount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(143, 149);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 0;
            this.btnLogin.Text = "登录";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // tbAccount
            // 
            this.tbAccount.Location = new System.Drawing.Point(172, 80);
            this.tbAccount.Name = "tbAccount";
            this.tbAccount.Size = new System.Drawing.Size(100, 21);
            this.tbAccount.TabIndex = 1;
            // 
            // lblAccount
            // 
            this.lblAccount.AutoSize = true;
            this.lblAccount.Location = new System.Drawing.Point(116, 83);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(29, 12);
            this.lblAccount.TabIndex = 2;
            this.lblAccount.Text = "账号";
            // 
            // CustomForm
            // 
            this.ClientSize = new System.Drawing.Size(588, 396);
            this.Controls.Add(this.lblAccount);
            this.Controls.Add(this.tbAccount);
            this.Controls.Add(this.btnLogin);
            this.Name = "CustomForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello");
        }
    }
}