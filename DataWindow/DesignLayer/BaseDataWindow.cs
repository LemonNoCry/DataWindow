using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using DataWindow.Core;
using DataWindow.DesignerInternal;
using DataWindow.Serialization.Components;

namespace DataWindow.DesignLayer
{
    /// <summary>
    /// DataWindow基类 <br/>
    /// 刷新/加载样式直接操作 LayoutXml 属性 <br/>
    /// 三种使用方式:<br/>
    /// 1.直接继承BaseDataWindow,作为Form 直接使用 LayoutXml。设计直接使用
    /// 2.任意Control，使用 DataWindowAnalysis:ResolveToDataWindow(),SerializationDataWindow() 来加载使用,<br/>
    /// 设计器直接调用DataWindowDesigner.DesignerLayout(Control)<br/>
    /// 3.VS设计器中，添加 DefaultDesignerLoader组件<br/>
    /// <br/>
    /// Designer组件需设置:<br/>
    /// DesignerLoader:设置DefaultDesignerLoader组件;<br/>
    /// DesignedForm：设置为当前窗口;
    /// 即可使用designer.LayoutXML来操作
    ///<br/>
    /// 在设计器中展示的属性网格:<br/>
    /// 1.直接使用[Browsable(true)][DisplayNameAttribute][DescriptionAttribute]等<br/>
    /// 2.继承当前控件的父类Serialization，GetCollections()中添加本地化翻译<br/>
    /// </summary>
    public class BaseDataWindow : Form
    {
        public BaseDataWindow()
        {
            InitializeComponent();

            designer.DesignerHost.UseNativeType = true;
        }

        [Browsable(false)]
        [ReadOnly(true)]
        public string DefaultXml { get; set; }

        [Browsable(false)]
        [XmlIgnore]
        [ReadOnly(true)]
        public string LayoutXml
        {
            get => this.designer.LayoutXML;
            set => this.designer.LayoutXML = value;
        }

        /// <summary>
        /// 控件必须
        /// </summary>
        public readonly List<Control> MustEditControls = new List<Control>();

        /// <summary>
        /// 禁止编辑控件
        /// </summary>
        public readonly List<Control> ProhibitEditControls = new List<Control>();

        /// <summary>
        /// 控件翻译
        /// </summary>
        public readonly Dictionary<Control, string> ControlTranslation = new Dictionary<Control, string>();

        public Designer designer;
        public DefaultDesignerLoader defaultDesignerLoader;

        /// <summary>
        /// 窗体上固有的控件
        /// </summary>
        public readonly List<Control> InherentControls = new List<Control>();


        public void AddMustControls(params Control[] cons)
        {
            MustEditControls.AddRange(cons);
        }

        /// <summary>
        /// 判断控件是否必须
        /// 1.控件必须
        /// 2.控件.子控件存在必须
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public bool IsMustControl(Control con)
        {
            var flag = MustEditControls.Contains(con);
            if (flag)
            {
                return flag;
            }

            if (con.HasChildren)
            {
                foreach (Control c in con.Controls)
                {
                    flag = IsMustControl(c);
                    if (flag)
                    {
                        return flag;
                    }
                }
            }

            return flag;
        }

        public bool IsInherentControl(Control con)
        {
            return InherentControls.Contains(con);
        }

        public bool IsProhibitEditControl(Control con)
        {
            return ProhibitEditControls.Contains(con);
        }

        public Control GetInherentControl(string name)
        {
            return InherentControls.SingleOrDefault(s => s.Name.Equals(name));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            EachDataWindowControls(this, c =>
            {
                InherentControls.Add(c);
                ProhibitedOperationControl(c);
            });
        }

        public void EachDataWindowControls(Control control, Action<Control> action)
        {
            foreach (Control con in control.Controls)
            {
                action?.Invoke(con);
                if (con.HasChildren)
                {
                    EachDataWindowControls(con, action);
                }
            }
        }

        public void ProhibitedOperationControl(Control con)
        {
            if (con is ToolStrip ts)
            {
                ProhibitEditControls.Add(ts);
            }
        }

        ~BaseDataWindow()
        {
            Clear();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Clear();
        }

        public void Clear()
        {
            MustEditControls.Clear();
            ProhibitEditControls.Clear();
            ControlTranslation.Clear();
            InherentControls.Clear();
        }

        private void InitializeComponent()
        {
            this.designer = new DataWindow.DesignLayer.Designer();
            this.defaultDesignerLoader = new DataWindow.Serialization.Components.DefaultDesignerLoader();
            this.SuspendLayout();
            // 
            // designer
            // 
            this.designer.DesignedForm = this;
            this.designer.DesignerLoader = this.defaultDesignerLoader;
            this.designer.GridSize = new System.Drawing.Size(8, 8);
            // 
            // BaseDataWindow
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "BaseDataWindow";
            this.Text = "自定义表单";
            this.Size = new Size(800, 600);
            this.ResumeLayout(false);
        }
    }
}