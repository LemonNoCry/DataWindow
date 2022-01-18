using System;
using System.Windows.Forms;
using DataWindow.CustomPropertys;
namespace DataWindow.Serialization
{
    [Serializable]
    public class CheckBoxSerializable: ControlSerializable, IPropertyCollections<CheckBox>
    {
        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as CheckBox);
        }

        public CustomPropertyCollection GetCollections(CheckBox control)
        {
            var cpc = base.GetCollections(control);

            cpc.Add(new CustomProperty("是否选中", "Checked", "数据", "Checked 是否选中。", control));
            cpc.Add(new CustomProperty("选中状态", "CheckState", "数据", "CheckState 选中状态。", control));


            cpc.Add(new CustomProperty("复选框的外观", "Appearance", "外观", "Appearance 复选框的外观。", control));
            cpc.Add(new CustomProperty("复选框的位置", "CheckAlign", "外观", "CheckAlign 复选框的位置。", control));
            cpc.Add(new CustomProperty("显示样式", "FlatStyle", "外观", "FlatStyle 显示样式。", control));
            cpc.Add(new CustomProperty("文本对齐方式", "TextAlign", "外观", "TextAlign 控件上显示的文本的对齐方式。", control));
            cpc.Add(new CustomProperty("自动省略", "AutoEllipsis", "外观", "AutoEllipsis 启用对超过按钮宽度意外的文本自动处理", control));

            return cpc;
        }
    }
}