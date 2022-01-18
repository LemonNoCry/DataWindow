using System;
using System.Drawing;
using System.Windows.Forms;
using DataWindow.CustomPropertys;

namespace DataWindow.Serialization
{
    [Serializable]
    public class LabelSerializable : ControlSerializable, IPropertyCollections<Label>
    {
        public ContentAlignment TextAlign { get; set; } = ContentAlignment.TopLeft;

        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as Label);
        }

        public CustomPropertyCollection GetCollections(Label control)
        {
            var cpc = base.GetCollections(control);

            cpc.Add(new CustomProperty("文本对齐方式", "TextAlign", "外观", "TextAlign 控件上显示的文本的对齐方式", control));
            cpc.Add(new CustomProperty("自动省略", "AutoEllipsis", "外观", "AutoEllipsis 启用对超过按钮宽度意外的文本自动处理", control));


            return cpc;
        }
    }
}