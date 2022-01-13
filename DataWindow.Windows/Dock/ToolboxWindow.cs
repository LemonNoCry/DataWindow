using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DataWindow.Core;
using DataWindow.DesignLayer;
using DataWindow.Toolbox;
using WeifenLuo.WinFormsUI.Docking;

namespace DataWindow.Windows.Dock
{
    public partial class ToolboxWindow : DockContent
    {
        public ToolboxWindow()
        {
            InitializeComponent();
            InitToolbox();
        }

        public IBaseDataWindow BaseDataWindow { get; set; }

        private void InitToolbox()
        {
            this.Toolbox.AddCategory("固有控件", ToolboxCategoryState.Collapsed);

            string groupName = "公共控件";
            this.Toolbox.AddToolboxItem(typeof(Button), groupName, "按钮");
            this.Toolbox.AddToolboxItem(typeof(CheckBox), groupName, "选择框");
            //this.Toolbox.AddToolboxItem(typeof(CheckedListBox), groupName);
            this.Toolbox.AddToolboxItem(typeof(ComboBox), groupName, "下拉框");
            this.Toolbox.AddToolboxItem(typeof(DateTimePicker), groupName, "日期时间选择器");
            this.Toolbox.AddToolboxItem(typeof(Label), groupName, "标签");
            this.Toolbox.AddToolboxItem(typeof(LinkLabel), groupName, "链接标签");
            //this.Toolbox.AddToolboxItem(typeof(ListBox), groupName);
            //this.Toolbox.AddToolboxItem(typeof(ListView), groupName);
            this.Toolbox.AddToolboxItem(typeof(MaskedTextBox), groupName, "掩码输入框");
            this.Toolbox.AddToolboxItem(typeof(MonthCalendar), groupName, "日历");
            //this.Toolbox.AddToolboxItem(typeof(NotifyIcon), groupName);
            this.Toolbox.AddToolboxItem(typeof(NumericUpDown), groupName, "数值输入框");
            this.Toolbox.AddToolboxItem(typeof(PictureBox), groupName, "图片");
            //this.Toolbox.AddToolboxItem(typeof(ProgressBar), groupName);
            this.Toolbox.AddToolboxItem(typeof(RadioButton), groupName, "单选框");
            this.Toolbox.AddToolboxItem(typeof(RichTextBox), groupName, "富文本框");
            this.Toolbox.AddToolboxItem(typeof(TextBox), groupName, "输入框");
            //this.Toolbox.AddToolboxItem(typeof(ToolTip), groupName);
            //this.Toolbox.AddToolboxItem(typeof(TreeView), groupName);
            //this.Toolbox.AddToolboxItem(typeof(WebBrowser), groupName);

            groupName = "容器";
            this.Toolbox.AddToolboxItem(typeof(FlowLayoutPanel), groupName, "流式布局");
            this.Toolbox.AddToolboxItem(typeof(GroupBox), groupName, "分组容器");
            this.Toolbox.AddToolboxItem(typeof(Panel), groupName, "面板");
            this.Toolbox.AddToolboxItem(typeof(SplitContainer), groupName, "分割器");
            this.Toolbox.AddToolboxItem(typeof(TabControl), groupName, "选项卡容器");
            this.Toolbox.AddToolboxItem(typeof(TableLayoutPanel), groupName, "表格布局");


            //groupName = "菜单和工具栏";
            //this.Toolbox.AddToolboxItem(typeof(ContextMenuStrip), groupName);
            //this.Toolbox.AddToolboxItem(typeof(MenuStrip), groupName);
            //this.Toolbox.AddToolboxItem(typeof(StatusStrip), groupName);
            //this.Toolbox.AddToolboxItem(typeof(ToolStrip), groupName);
            //this.Toolbox.AddToolboxItem(typeof(ToolStripContainer), groupName);

            //groupName = "数据";
            //this.Toolbox.AddToolboxItem(typeof(BindingNavigator), groupName);
            //this.Toolbox.AddToolboxItem(typeof(BindingSource), groupName);
            //this.Toolbox.AddToolboxItem(typeof(DataGridView), groupName);

            //groupName = "组件";
            //this.Toolbox.AddToolboxItem(typeof(BackgroundWorker), groupName);
            //this.Toolbox.AddToolboxItem(typeof(ErrorProvider), groupName);
            //this.Toolbox.AddToolboxItem(typeof(Timer), groupName);
        }

        public void AddDataWindowControl(Control parent)
        {
            Dictionary<Control, string> controlTranslation = new Dictionary<Control, string>();
            if (BaseDataWindow != null)
            {
                controlTranslation = BaseDataWindow.GetControlTranslation();
            }

            foreach (Control con in parent.Controls)
            {
                string displayName;
                if (!controlTranslation.TryGetValue(con, out displayName))
                {
                    if (string.IsNullOrWhiteSpace(displayName))
                    {
                        displayName = con.Text;
                    }

                    if (string.IsNullOrWhiteSpace(displayName))
                    {
                        displayName = con.Name;
                    }
                }

                this.Toolbox.AddToolboxItem(con, "固有控件", displayName);
                if (con.HasChildren)
                {
                    AddDataWindowControl(con);
                }
            }
        }
    }
}