using System;
using System.Windows.Forms;
using DataWindow.CustomPropertys;

namespace DataWindow.Serialization
{
    [Serializable]
    public class TextBoxSerializable : ControlSerializable, IPropertyCollections<TextBox>
    {
        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as TextBox);
        }

        public CustomPropertyCollection GetCollections(TextBox control)
        {
            var cpc = base.GetCollections(control);

            cpc.Add(new CustomProperty("多行编辑中的文本行", "Lines", "数据", "Lines 控件上显示的文本的对齐方式", control));
            cpc.Add(new CustomProperty("最大字符数", "MaxLength", "数据", "MaxLength 最大字符数。", control));


            cpc.Add(new CustomProperty("滚动条", "ScrollBars", "外观", "ScrollBars 对于多行编辑时，显示哪些滚动条。", control));
            cpc.Add(new CustomProperty("文本对齐方式", "TextAlign", "外观", "TextAlign 控件上显示的文本的对齐方式。", control));
            cpc.Add(new CustomProperty("格式化", "CharacterCasing", "外观", "CharacterCasing 指定所有字符赢保持不变还是转换为大小写。", control));

            return cpc;
        }
    }
}