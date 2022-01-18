using System;
using System.Windows.Forms;
using DataWindow.CustomPropertys;

namespace DataWindow.Serialization
{
    [Serializable]
    public class DateTimePickerSerializable : ControlSerializable, IPropertyCollections<DateTimePicker>
    {
        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as DateTimePicker);
        }

        public CustomPropertyCollection GetCollections(DateTimePicker control)
        {
            var cpc = base.GetCollections(control);

            if (!string.IsNullOrWhiteSpace(control.CustomFormat) && control.Format != DateTimePickerFormat.Custom)
            {
                //必须定义为Custom，CustomFormat才有效
                control.Format = DateTimePickerFormat.Custom;
            }

            control.MaxDate = new DateTime(2100, 1, 1);
            control.MinDate = new DateTime(1950, 1, 1);
            if (control.Value < control.MinDate | control.Value > control.MaxDate)
            {
                control.Value = DateTime.Now;
            }


            cpc.Add(new CustomProperty("自定义格式化", "CustomFormat", "行为", "CustomFormat 用于格式化在控件中显示的日期/或时间的自定义字符串。", control));
            cpc.Add(new CustomProperty("格式化", "Format", "行为", "Format 确定日期和时间使用标准格式显示还是使用自定义格式。", control));
            cpc.Add(new CustomProperty("最大日期", "MaxDate", "行为", "MaxDate 最大日期", control));
            cpc.Add(new CustomProperty("最小日期", "MinDate", "行为", "MinDate 最小日期", control));

            cpc.Add(new CustomProperty("月历文本字体", "CalendarFont", "外观", "CalendarFont 月历文本字体。", control));
            cpc.Add(new CustomProperty("月历文本颜色", "CalendarForeColor", "外观", "CalendarForeColor 月历文本颜色。", control));
            cpc.Add(new CustomProperty("月份背景色", "CalendarMonthBackground", "外观", "CalendarMonthBackground 月份背景色。", control));
            cpc.Add(new CustomProperty("日历标题背景色", "CalendarTitleBackColor", "外观", "CalendarTitleBackColor 日历标题背景色。", control));
            cpc.Add(new CustomProperty("日历标题字体颜色", "CalendarTitleForeColor", "外观", "CalendarTitleForeColor 日历标题字体颜色。", control));
            cpc.Add(new CustomProperty("日历结尾日期的前景色", "CalendarTrailingForeColor", "外观", "CalendarTrailingForeColor 用于显示前接目期和后续日期文本的颜色。前接日期和后续日期是指在当前月历上豆示的上一个月和下—个月的目期。。", control));
            cpc.Add(new CustomProperty("下拉对齐方式", "DropDownAlign", "外观", "DropDownAlign 下拉对齐方式。", control));
            cpc.Add(new CustomProperty("显示复选框", "ShowCheckBox", "外观", "ShowCheckBox 是否显示复选框，当复选框未选中时，标识未选择任何值。", control));
            cpc.Add(new CustomProperty("显示数字显示框", "ShowUpDown", "外观", "ShowUpDown 显示数字显示框而不是显示下拉日历。", control));

            return cpc;
        }
    }
}