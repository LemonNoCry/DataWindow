﻿using System;
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
        private Dock.MyTextBox myTextBox1;
        private Label label1;
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
            this.label1 = new System.Windows.Forms.Label();
            this.myTextBox1 = new DataWindow.Windows.Dock.MyTextBox();
            this.SuspendLayout();
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(107, 259);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 0;
            this.btnLogin.Text = "登录";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // tbAccount
            // 
            this.tbAccount.Location = new System.Drawing.Point(400, 244);
            this.tbAccount.Name = "tbAccount";
            this.tbAccount.Size = new System.Drawing.Size(100, 21);
            this.tbAccount.TabIndex = 1;
            // 
            // lblAccount
            // 
            this.lblAccount.AutoSize = true;
            this.lblAccount.Location = new System.Drawing.Point(344, 247);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(29, 12);
            this.lblAccount.TabIndex = 2;
            this.lblAccount.Text = "账号";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(344, 289);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "密码";
            // 
            // myTextBox1
            // 
            this.myTextBox1.Location = new System.Drawing.Point(400, 286);
            this.myTextBox1.Name = "myTextBox1";
            this.myTextBox1.Size = new System.Drawing.Size(100, 21);
            this.myTextBox1.TabIndex = 3;
            // 
            // CustomForm
            // 
            this.ClientSize = new System.Drawing.Size(588, 396);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.myTextBox1);
            this.Controls.Add(this.lblAccount);
            this.Controls.Add(this.tbAccount);
            this.Controls.Add(this.btnLogin);
            this.Name = "CustomForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            MessageBox.Show(GetLayoutXml());
        }

    }
}