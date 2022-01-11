using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Text;

namespace DataWindow.Services
{
    public class BaseEventBindingService : IEventBindingService
    {
        private Hashtable _eventProperties;
        public virtual IServiceProvider ServiceProvider { get; set; }

        string IEventBindingService.CreateUniqueMethodName(IComponent component, EventDescriptor e)
        {
            if (component == null) throw new ArgumentNullException("component");
            if (e == null) throw new ArgumentNullException("e");
            return CreateUniqueMethodName(component, e);
        }

        ICollection IEventBindingService.GetCompatibleMethods(EventDescriptor e)
        {
            if (e == null) throw new ArgumentNullException("e");
            return GetCompatibleMethods(e);
        }

        EventDescriptor IEventBindingService.GetEvent(PropertyDescriptor property)
        {
            EventPropertyDescriptor eventPropertyDescriptor;
            if ((eventPropertyDescriptor = property as EventPropertyDescriptor) != null) return eventPropertyDescriptor.Event;
            return null;
        }

        PropertyDescriptorCollection IEventBindingService.GetEventProperties(EventDescriptorCollection events)
        {
            if (events == null) throw new ArgumentNullException("events");
            var array = new PropertyDescriptor[events.Count];
            if (_eventProperties == null) _eventProperties = new Hashtable();
            for (var i = 0; i < events.Count; i++)
            {
                object eventDescriptorHashCode = GetEventDescriptorHashCode(events[i]);
                array[i] = (PropertyDescriptor) _eventProperties[eventDescriptorHashCode];
                if (array[i] == null)
                {
                    array[i] = new EventPropertyDescriptor(events[i], this);
                    _eventProperties[eventDescriptorHashCode] = array[i];
                }
            }

            return new PropertyDescriptorCollection(array);
        }

        PropertyDescriptor IEventBindingService.GetEventProperty(EventDescriptor e)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (_eventProperties == null) _eventProperties = new Hashtable();
            object eventDescriptorHashCode = GetEventDescriptorHashCode(e);
            var propertyDescriptor = (PropertyDescriptor) _eventProperties[eventDescriptorHashCode];
            if (propertyDescriptor == null)
            {
                propertyDescriptor = new EventPropertyDescriptor(e, this);
                _eventProperties[eventDescriptorHashCode] = propertyDescriptor;
            }

            return propertyDescriptor;
        }

        bool IEventBindingService.ShowCode()
        {
            return ShowCode();
        }

        bool IEventBindingService.ShowCode(int lineNumber)
        {
            return ShowCode(lineNumber);
        }

        bool IEventBindingService.ShowCode(IComponent component, EventDescriptor e)
        {
            if (component == null) throw new ArgumentNullException("component");
            if (e == null) throw new ArgumentNullException("e");
            var text = (string) ((IEventBindingService) this).GetEventProperty(e).GetValue(component);
            return text != null && ShowCode(component, e, text);
        }

        protected void ClearData()
        {
            var eventProperties = _eventProperties;
            if (eventProperties == null) return;
            eventProperties.Clear();
        }

        protected string CreateUniqueMethodName(IComponent component, EventDescriptor e)
        {
            return component.ToString().Split(' ')[0] + "_" + e.Name;
        }

        protected virtual ICollection GetCompatibleMethods(EventDescriptor e)
        {
            return new string[0];
        }

        protected string GetEventDescriptorHashCode(EventDescriptor eventDesc)
        {
            var stringBuilder = new StringBuilder(eventDesc.Name);
            stringBuilder.Append(eventDesc.EventType.GetHashCode().ToString());
            foreach (var obj in eventDesc.Attributes)
            {
                var attribute = (Attribute) obj;
                stringBuilder.Append(attribute.GetHashCode().ToString());
            }

            return stringBuilder.ToString();
        }

        protected object GetService(Type serviceType)
        {
            var serviceProvider = ServiceProvider;
            if (serviceProvider == null) return null;
            return serviceProvider.GetService(serviceType);
        }

        protected bool ShowCode()
        {
            return false;
        }

        protected bool ShowCode(int lineNumber)
        {
            return false;
        }

        protected bool ShowCode(object component, EventDescriptor e, string methodName)
        {
            return false;
        }

        public virtual void FreeMethod(object component, EventDescriptor e, string methodName)
        {
        }

        public virtual void UseMethod(object component, EventDescriptor e, string methodName)
        {
        }

        protected virtual void ValidateMethodName(string methodName)
        {
        }

        private class EventPropertyDescriptor : PropertyDescriptor
        {
            private readonly BaseEventBindingService _eventSvc;
            private TypeConverter _converter;

            internal EventPropertyDescriptor(EventDescriptor eventDesc, BaseEventBindingService eventSvc) : base(eventDesc, null)
            {
                Event = eventDesc;
                _eventSvc = eventSvc;
            }

            internal EventDescriptor Event { get; }

            public override Type ComponentType => Event.ComponentType;

            public override TypeConverter Converter
            {
                get
                {
                    if (_converter == null) _converter = new EventConverter(Event);
                    return _converter;
                }
            }

            public override bool IsReadOnly => Attributes[typeof(ReadOnlyAttribute)].Equals(ReadOnlyAttribute.Yes);

            public override Type PropertyType => Event.EventType;

            public override bool CanResetValue(object component)
            {
                return GetValue(component) != null;
            }

            public override object GetValue(object component)
            {
                if (component == null) throw new ArgumentNullException("component");
                var compoentSite = GetCompoentSite(component);
                var value = GetDictionaryService(compoentSite).GetValue(new ReferenceEventClosure(component, this));
                if (value == null) return null;
                return value.ToString();
            }

            public override void SetValue(object component, object value)
            {
                if (component == null) throw new ArgumentNullException("component");
                if (IsReadOnly) throw new InvalidOperationException("试图设置只读事件。");
                if (value != null && !(value is string)) throw new ArgumentException("不能设置值" + value + "。");
                var compoentSite = GetCompoentSite(component);
                var dictionaryService = GetDictionaryService(compoentSite);
                var text = (string) value;
                if (text != null && text.Length == 0) text = null;
                var key = new ReferenceEventClosure(component, this);
                var value2 = dictionaryService.GetValue(key);
                var text2 = value2 != null ? value2.ToString() : null;
                if (text2 == text || text2 != null && text != null && text2.Equals(text)) return;
                if (text != null) _eventSvc.ValidateMethodName(text);
                var componentChangeService = compoentSite.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                if (componentChangeService != null)
                    try
                    {
                        componentChangeService.OnComponentChanging(component, this);
                    }
                    catch (CheckoutException ex)
                    {
                        if (ex == CheckoutException.Canceled) return;
                        throw ex;
                    }

                if (text != null) _eventSvc.UseMethod(component, Event, text);
                if (text2 != null) _eventSvc.FreeMethod(component, Event, text2);
                dictionaryService.SetValue(key, text);
                if (componentChangeService != null) componentChangeService.OnComponentChanged(component, this, text2, text);
                OnValueChanged(component, EventArgs.Empty);
            }

            public override void ResetValue(object component)
            {
                SetValue(component, null);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return CanResetValue(component);
            }

            private ISite GetCompoentSite(object component)
            {
                ISite site = null;
                IComponent component2;
                if ((component2 = component as IComponent) != null) site = component2.Site;
                IReferenceService referenceService;
                IComponent component3;
                if (site == null && (referenceService = _eventSvc.ServiceProvider.GetService(typeof(IReferenceService)) as IReferenceService) != null && (component3 = referenceService.GetComponent(component)) != null) site = component3.Site;
                if (site == null) throw new InvalidOperationException("组件 " + component + " 没有设置 Site 属性");
                return site;
            }

            private IDictionaryService GetDictionaryService(ISite site)
            {
                IDictionaryService result;
                if ((result = site.GetService(typeof(IDictionaryService)) as IDictionaryService) != null) return result;
                throw new InvalidOperationException("没有找到 IDictionaryService。");
            }

            private class EventConverter : TypeConverter
            {
                private readonly EventDescriptor _evt;

                internal EventConverter(EventDescriptor evt)
                {
                    _evt = evt;
                }

                public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
                {
                    return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
                }

                public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
                {
                    return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
                }

                public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
                {
                    if (value == null) return value;
                    if (!(value is string)) return base.ConvertFrom(context, culture, value);
                    if (((string) value).Length == 0) return null;
                    return value;
                }

                public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
                {
                    if (!(destinationType == typeof(string))) return base.ConvertTo(context, culture, value, destinationType);
                    if (value != null) return value;
                    return string.Empty;
                }

                public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
                {
                    string[] array = null;
                    IEventBindingService eventBindingService;
                    if (context != null && (eventBindingService = context.GetService(typeof(IEventBindingService)) as IEventBindingService) != null)
                    {
                        var compatibleMethods = eventBindingService.GetCompatibleMethods(_evt);
                        array = new string[compatibleMethods.Count];
                        var num = 0;
                        foreach (var obj in compatibleMethods)
                        {
                            var text = (string) obj;
                            array[num++] = text;
                        }
                    }

                    return new StandardValuesCollection(array);
                }

                public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
                {
                    return false;
                }

                public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
                {
                    return true;
                }
            }

            private class ReferenceEventClosure
            {
                private readonly EventPropertyDescriptor propertyDescriptor;

                private readonly object reference;

                public ReferenceEventClosure(object reference, EventPropertyDescriptor prop)
                {
                    this.reference = reference;
                    propertyDescriptor = prop;
                }

                public override int GetHashCode()
                {
                    return reference.GetHashCode() * propertyDescriptor.GetHashCode();
                }

                public override bool Equals(object otherClosure)
                {
                    if (!(otherClosure is ReferenceEventClosure)) return false;
                    var referenceEventClosure = (ReferenceEventClosure) otherClosure;
                    return referenceEventClosure.reference == reference && referenceEventClosure.propertyDescriptor == propertyDescriptor;
                }
            }
        }
    }
}