using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DataWindow.Utility;

namespace DataWindow.CustomPropertys
{
    /// <summary>
    /// 自定义属性集合
    /// </summary>
    public class CustomPropertyCollection : List<CustomProperty>, ICustomTypeDescriptor
    {
        #region 内部方法

        public CustomProperty FindCustomProperty(string name)
        {
            return this.FirstOrDefault(s => s.PropertyNames.Contains(name));
        }

        #endregion

        #region ICustomTypeDescriptor 成员

        public PropertyDescriptorCollection _propDescCol;
        public object Sources { get; set; }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection CustomAddProperties(PropertyDescriptorCollection pdc)
        {
            var fixedPro = this.Where(s => s.FixedValue != null && !string.IsNullOrWhiteSpace(s.FixedValue.ToString())).ToList();
            if (fixedPro.Any())
            {
                foreach (CustomProperty cp in fixedPro)
                {
                    List<Attribute> attrs = new List<Attribute>();
                    //[Browsable(false)]
                    if (!cp.IsBrowsable)
                    {
                        attrs.Add(new BrowsableAttribute(cp.IsBrowsable));
                    }

                    //[ReadOnly(true)]
                    if (cp.IsReadOnly)
                    {
                        attrs.Add(new ReadOnlyAttribute(cp.IsReadOnly));
                    }

                    //[Editor(typeof(editor),typeof(UITypeEditor))]
                    if (cp.EditorType != null)
                    {
                        attrs.Add(new EditorAttribute(cp.EditorType, typeof(System.Drawing.Design.UITypeEditor)));
                    }

                    _propDescCol.Add(new CustomPropertyDescriptor(cp, attrs.ToArray()));
                }
            }

            return pdc;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (_propDescCol == null)
            {
                PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this.Sources, attributes, true);
                _propDescCol = new PropertyDescriptorCollection(null);

                foreach (PropertyDescriptor prop in baseProps)
                {
                    var cp = FindCustomProperty(prop.Name);
                    if (cp == null)
                    {
                        if (prop.DisplayName.HasChinese() && prop.Description.HasChinese())
                        {
                            _propDescCol.Add(prop);
                        }

                        continue;
                    }

                    var attrs = prop.Attributes.Cast<Attribute>().ToList();

                    //[Browsable(false)]
                    if (!cp.IsBrowsable)
                    {
                        attrs.Add(new BrowsableAttribute(cp.IsBrowsable));
                    }

                    //[ReadOnly(true)]
                    if (cp.IsReadOnly)
                    {
                        attrs.Add(new ReadOnlyAttribute(cp.IsReadOnly));
                    }

                    //[Editor(typeof(editor),typeof(UITypeEditor))]
                    if (cp.EditorType != null)
                    {
                        attrs.Add(new EditorAttribute(cp.EditorType, typeof(System.Drawing.Design.UITypeEditor)));
                    }


                    _propDescCol.Add(new CustomPropertyDescriptor(prop, attrs.ToArray(), cp));
                }

                CustomAddProperties(_propDescCol);
                //foreach (CustomProperty cp in this)
                //{
                //    List<Attribute> attrs = new List<Attribute>();
                //    //[Browsable(false)]
                //    if (!cp.IsBrowsable)
                //    {
                //        attrs.Add(new BrowsableAttribute(cp.IsBrowsable));
                //    }

                //    //[ReadOnly(true)]
                //    if (cp.IsReadOnly)
                //    {
                //        attrs.Add(new ReadOnlyAttribute(cp.IsReadOnly));
                //    }

                //    //[Editor(typeof(editor),typeof(UITypeEditor))]
                //    if (cp.EditorType != null)
                //    {
                //        attrs.Add(new EditorAttribute(cp.EditorType, typeof(System.Drawing.Design.UITypeEditor)));
                //    }

                //    _propDescCol.Add(new CustomPropertyDescriptor(cp, attrs.ToArray()));
                //}
            }

            return _propDescCol;
        }

        public PropertyDescriptorCollection GetProperties()
        {
            if (_propDescCol == null)
            {
                PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this.Sources, true);
                _propDescCol = new PropertyDescriptorCollection(null);

                foreach (PropertyDescriptor prop in baseProps)
                {
                    var cp = FindCustomProperty(prop.Name);
                    if (cp == null)
                    {
                        continue;
                    }

                    var attrs = prop.Attributes.Cast<Attribute>().ToList();

                    //[Browsable(false)]
                    if (!cp.IsBrowsable)
                    {
                        attrs.Add(new BrowsableAttribute(cp.IsBrowsable));
                    }

                    //[ReadOnly(true)]
                    if (cp.IsReadOnly)
                    {
                        attrs.Add(new ReadOnlyAttribute(cp.IsReadOnly));
                    }

                    //[Editor(typeof(editor),typeof(UITypeEditor))]
                    if (cp.EditorType != null)
                    {
                        attrs.Add(new EditorAttribute(cp.EditorType, typeof(System.Drawing.Design.UITypeEditor)));
                    }


                    _propDescCol.Add(new CustomPropertyDescriptor(prop, attrs.ToArray(), cp));
                }
                CustomAddProperties(_propDescCol);
            }

            return _propDescCol;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return Sources;
        }

        #endregion
    }
}