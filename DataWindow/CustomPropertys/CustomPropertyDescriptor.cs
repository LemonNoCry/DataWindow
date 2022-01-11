using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace DataWindow.CustomPropertys
{
    /// <summary>
    /// 属性描述
    /// </summary>
    public class CustomPropertyDescriptor : PropertyDescriptor, ISite
    {
        private readonly CustomProperty _customProperty = null;
        private readonly PropertyDescriptor _propertyDescriptor = null;

        public CustomPropertyDescriptor(PropertyDescriptor basePropertyDescriptor, Attribute[] attrs, CustomProperty customProperty)
            : base(basePropertyDescriptor, attrs)
        {
            this._propertyDescriptor = basePropertyDescriptor;
            this._customProperty = customProperty;
        }

        #region PropertyDescriptor

        public override bool CanResetValue(object component)
        {
            return _customProperty.DefaultValue != null;
        }

        public override Type ComponentType
        {
            get { return _propertyDescriptor.ComponentType; }
        }

        [RefreshProperties(RefreshProperties.All)]
        public override object GetValue(object component)
        {
            //return _customProperty.Value;
            return _propertyDescriptor.GetValue(component);
        }

        public override bool IsReadOnly
        {
            get { return _customProperty.IsReadOnly; }
        }

        public override Type PropertyType
        {
            get { return this._propertyDescriptor.PropertyType; }
        }

        public override void ResetValue(object component)
        {
            //_customProperty.ResetValue();
            _propertyDescriptor.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            //_customProperty.Value = value;
            _propertyDescriptor.SetValue(component, value);
        }

        private bool? flag = null;

        public override bool ShouldSerializeValue(object component)
        {
            if (!flag.HasValue)
            {
                flag = _propertyDescriptor.ShouldSerializeValue(component);
            }

            return flag.Value;
        }

        //
        public override string Description
        {
            get { return _customProperty.Description; }
        }

        public override string Category
        {
            get { return _customProperty.Category; }
        }

        [RefreshProperties(RefreshProperties.All)]
        public override string DisplayName
        {
            get { return _customProperty.Name; }
        }

        public override bool IsBrowsable
        {
            get { return _customProperty.IsBrowsable; }
        }

        public CustomProperty CustomProperty
        {
            get { return _customProperty; }
        }

        #endregion


        #region ISite

        public object GetService(Type serviceType)
        {
            return _customProperty.Site.GetService(serviceType);
        }

        public IComponent Component => _customProperty.Site.Component;
        public IContainer Container => _customProperty.Site.Container;
        public bool DesignMode => _customProperty.Site.DesignMode;

        string ISite.Name
        {
            get => _customProperty.Site.Name;
            set => _customProperty.Site.Name = value;
        }

        #endregion
    }
}