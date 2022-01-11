using System.ComponentModel;
using System.Windows.Forms;
using DataWindow.Core;

namespace DataWindow.DesignLayer
{
    public class localizationPropertyGrid : PropertyGrid
    {
        public localizationPropertyGrid()
        {
            DisplayMode = DisplayModeEnum.None;
        }

        /// <summary>
        /// 网格显示模式。
        /// </summary>
        public enum DisplayModeEnum
        {
            /// <summary>
            /// 不处理，就是PropertyGrid之前的方式。
            /// </summary>
            None,

            /// <summary>
            /// 实施人员和业务人员(普通用户)使用。
            /// </summary>
            ForNormalUser
        }

        /// <summary>
        /// 网格显示模式。
        /// </summary>
        [Browsable(true)]
        [Description("None:不处理，就是PropertyGrid之前的方式。ForAdvancedUser:高级用户使用。 ForNormalUser:普通用户使用。")]
        public DisplayModeEnum DisplayMode { get; set; }

        /// <summary>
        /// 获取或设置在网格中显示属性的对象。<br/>
        /// 返回结果:对象列表中的第一个对象。如果当前没有选定任何对象，返回值则为 null。
        /// </summary>
        public new object SelectedObject
        {
            get { return base.SelectedObject; }
            set
            {
                if (value == null)
                {
                    base.SelectedObject = null;
                    return;
                }

                switch (DisplayMode)
                {
                    case DisplayModeEnum.None:
                        base.SelectedObject = value;
                        break;
                    case DisplayModeEnum.ForNormalUser:
                    default:
                        if (value is Control con)
                        {
                            base.SelectedObject = con.GetCollections();
                        }
                        else
                        {
                            base.SelectedObject = value;
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// 获取或设置当前选定的对象。
        /// 返回结果:System.Object 类型数组。默认为空数组。
        /// </summary>
        /// <exception cref="System.ArgumentException">对象数组中某一项的值为 null。</exception>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new object[] SelectedObjects
        {
            get { return base.SelectedObjects; }
            set
            {
                if (value == null)
                {
                    base.SelectedObjects = null;
                    return;
                }

                switch (DisplayMode)
                {
                    case DisplayModeEnum.None:
                        base.SelectedObjects = value;
                        break;
                    case DisplayModeEnum.ForNormalUser:
                    default:
                        if (value is Control[] cons)
                        {
                            base.SelectedObject = cons.GetCollections();
                        }
                        else
                        {
                            base.SelectedObject = value;
                        }

                        break;
                }
            }
        }
    }
}