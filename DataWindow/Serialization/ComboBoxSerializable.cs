using System;
using System.Windows.Forms;
using DataWindow.CustomPropertys;
namespace DataWindow.Serialization
{
    [Serializable]
    public class ComboBoxSerializable: ControlSerializable, IPropertyCollections<ComboBox>
    {
        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as ComboBox);
        }

        public CustomPropertyCollection GetCollections(ComboBox control)
        {
            var cpc = base.GetCollections(control);

            cpc.Add(new CustomProperty("数据", "Items", "数据", "Items 数据", control));
            cpc.Add(new CustomProperty("显示值", "DisplayMember", "数据", "DisplayMember 显示值", control));
            cpc.Add(new CustomProperty("实际值", "ValueMember", "数据", "ValueMember 实际值", control));

            cpc.Add(new CustomProperty("下拉框的样式", "DropDownStyle", "外观", "DropDownStyle 下拉框的样式。", control));
            cpc.Add(new CustomProperty("显示样式", "FlatStyle", "外观", "FlatStyle 显示样式。", control));
            cpc.Add(new CustomProperty("下拉框的宽度", "DropDownWidth", "外观", "DropDownWidth 下拉框的宽度。", control));
            cpc.Add(new CustomProperty("下拉框的高度", "DropDownHeight", "外观", "DropDownHeight 下拉框的高度。", control));
            cpc.Add(new CustomProperty("自动调整", "IntegralHeight", "外观", "IntegralHeight 自动调整大小以避免显示部分项。", control));
            cpc.Add(new CustomProperty("下拉框中的数据高度", "ItemHeight", "外观", "ItemHeight 下拉框中的数据高度。", control));
            cpc.Add(new CustomProperty("下拉框中的最多项数", "MaxDropDownItems", "外观", "MaxDropDownItems 下拉框中的最多项数。", control));
            cpc.Add(new CustomProperty("是否排序", "Sorted", "外观", "Sorted 下拉框是否排序。", control));
            cpc.Add(new CustomProperty("格式化", "FormatString", "外观", "FormatString 格式化。", control));

            return cpc;
        }
    }
}