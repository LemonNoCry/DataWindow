﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using DataWindow.CustomPropertys;
using DataWindow.Serialization.CustomizeProperty;

namespace DataWindow.Serialization
{
    /// <summary>
    /// 控件序列化基类 <br/>
    /// </summary>
    [Serializable]
    public class ControlSerializable : IPropertyCollections<Control>, IHostCreateComponent<Control>
    {
        public string Name { get; set; }

        [XmlElement(Type = typeof(XmlType))]
        public Type Type { get; set; }

        public string Text { get; set; }

        [XmlElement(Type = typeof(XmlColor))]
        public Color BackColor { get; set; }

        [XmlElement(Type = typeof(XmlColor))]
        public Color ForeColor { get; set; }

        public CustomizeFont Font { get; set; }

        [XmlElement(Type = typeof(CustomizePoint))]
        public Point Location { get; set; }

        [XmlElement(Type = typeof(CustomizeSize))]
        public Size Size { get; set; }

        public bool AutoSize { get; set; }

        [XmlElement(Type = typeof(CustomizeRectangle))]
        public Rectangle ClientRectangle { get; set; }

        [XmlElement(Type = typeof(CustomizePadding))]
        public Padding Margin { get; set; }

        public AnchorStyles Anchor { get; set; }
        public DockStyle Dock { get; set; }
        public object Tag { get; set; }
        public int TabIndex { get; set; }

        public bool Visible { get; set; }

        [XmlIgnore]
        public Control Current { get; set; }

        public ControlSerializable ParentSerializable { get; set; }
        public List<ControlSerializable> ControlsSerializable { get; set; }

        public virtual CustomPropertyCollection GetCollections(Control control)
        {
            var collection = new CustomPropertyCollection();
            collection.Sources = control;
            collection.Add(new CustomProperty("Name", "Name", "数据", "控件的Name", control) {IsReadOnly = true});
            collection.Add(new CustomProperty("文本", "Text", "数据", "Text 要显示的内容。", control, typeof(MultilineStringEditor)));

            collection.Add(new CustomProperty("背景色", "BackColor", "外观", "BackColor 背景色。", control));
            collection.Add(new CustomProperty("字体颜色", "ForeColor", "外观", "ForeColor 前景色。", control));
            collection.Add(new CustomProperty("字体", "Font", "外观", "Font 字体。", control));
            collection.Add(new CustomProperty("显示", "Visible", "外观", "是否显示", control));
            collection.Add(new CustomProperty("鼠标样式", "Cursor", "外观", "Cursor 鼠标移过该控件时显示的光标", control));
            collection.Add(new CustomProperty("从右向左显示", "RightToLeft", "外观", "RightToLeft 从右向左显示。", control));
            collection.Add(new CustomProperty("是否有焦点", "Sorted", "外观", "Sorted 是否有焦点。", control));


            collection.Add(new CustomProperty("位置", "Location", "布局", "Location 控件左上角相对于其容器左上角的坐标。", control));
            collection.Add(new CustomProperty("大小", "Size", "布局", "Size 控件的大小，以像素为单位。", control));
            collection.Add(new CustomProperty("自动大小", "AutoSize", "布局", "自动调整大小以适应内容长度", control));
            collection.Add(new CustomProperty("边距", "Margin", "布局", "Margin 指定此控件与另一控件之间边距的距离。", control));
            collection.Add(new CustomProperty("内间距", "Padding", "布局", "Padding 指定控件的内部间距。", control));
            collection.Add(new CustomProperty("锚", "Anchor", "布局", "Anchor 定义要绑定到容器的边缘，当控件锚定位到某个控件时，与指定边缘最接近的控件边缘与指定边缘之间的距离将保持不变。", control));
            collection.Add(new CustomProperty("停靠", "Dock", "布局", "Dock 定义要绑定到容器的控件边框。", control));
            collection.Add(new CustomProperty("锁定布局", "Locked", "布局", "Locked 锁定布局，不可移动。", control));
            collection.Add(new CustomProperty("最大大小", "MaximumSize", "布局", "MaximumSize 指定控件最大大小。", control));
            collection.Add(new CustomProperty("最小大小", "MinimumSize", "布局", "MinimumSize 指定控件最小大小。", control));

            collection.Add(new CustomProperty("Tab索引", "TabIndex", "行为", "TabIndex 确定此控件将占用的 Tab 键顺序索引。", control));
            collection.Add(new CustomProperty("是否启用", "Enabled", "行为", "Enabled 是否启用该控件。", control));

            control.Tag = control.Tag ?? "";
            collection.Add(new CustomProperty("Tag", "Tag", "行为", "与用户关联的自定义数据", control));

            collection.Add(new CustomProperty("Type", "Type", "内部", "类型", null, Type.FullName, null) {IsReadOnly = true, ValueType = typeof(string)});
            return collection;
        }

        public virtual void CopyPropertyComponent(Control source, Control target)
        {
            if (source == null || target == null)
            {
                return;
            }
        }
    }
}