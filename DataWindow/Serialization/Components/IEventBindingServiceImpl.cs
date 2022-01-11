using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Forms;
using DataWindow.Services;

namespace DataWindow.Serialization.Components
{
    internal class IEventBindingServiceImpl : BaseEventBindingService
    {
        private readonly Dictionary<object, List<EventData>> _events = new Dictionary<object, List<EventData>>();

        private bool _loading;
        public object Source { get; set; }

        public override IServiceProvider ServiceProvider
        {
            set
            {
                var serviceProvider = ServiceProvider;
                IComponentChangeService componentChangeService;
                if (serviceProvider != null && (componentChangeService = serviceProvider.GetService(typeof(IComponentChangeService)) as IComponentChangeService) != null) componentChangeService.ComponentRemoved -= OnComponentRemoved;
                base.ServiceProvider = value;
                if (value == null) return;
                IComponentChangeService componentChangeService2;
                if ((componentChangeService2 = value.GetService(typeof(IComponentChangeService)) as IComponentChangeService) != null) componentChangeService2.ComponentRemoved += OnComponentRemoved;
            }
        }

        public List<EventData> GetEventDatas(object compoent)
        {
            if (_events.ContainsKey(compoent)) return _events[compoent];
            return null;
        }

        internal void AddEvent(object component, string eventName, string methodName)
        {
            var list = GetEventDatas(component);
            if (list == null)
            {
                list = new List<EventData>();
                _events.Add(component, list);
            }

            list.Add(new EventData(eventName, methodName));
        }

        private void LoadEventData(object s, EventArgs e)
        {
            _loading = true;
            var arrayList = new ArrayList();
            foreach (var keyValuePair in _events)
            {
                var events = TypeDescriptor.GetEvents(keyValuePair.Key);
                var component = (IComponent) keyValuePair.Key;
                if (component == null || component.Site == null)
                    arrayList.Add(keyValuePair.Key);
                else
                    foreach (var eventData in keyValuePair.Value)
                    {
                        var e2 = events[eventData.EventName];
                        ((IEventBindingService) this).GetEventProperty(e2).SetValue(keyValuePair.Key, eventData.MethodName);
                    }
            }

            foreach (var key in arrayList) _events.Remove(key);
            _loading = false;
        }

        public void RefreshEventData()
        {
            ClearData();
            LoadEventData(this, new EventArgs());
        }

        private void OnComponentRemoved(object sender, ComponentEventArgs e)
        {
            _events.Remove(e.Component);
        }

        private ICollection GetCompatibleMethods(object obj, EventDescriptor ed)
        {
            var arrayList = new ArrayList();
            var type = obj.GetType();
            var method = ed.EventType.GetMethod("Invoke");
            var parameters = method.GetParameters();
            foreach (var methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                var parameters2 = methodInfo.GetParameters();
                if (method.ReturnType == methodInfo.ReturnType && parameters.Length == parameters2.Length)
                {
                    var num = 0;
                    var flag = true;
                    var array = parameters;
                    for (var j = 0; j < array.Length; j++)
                    {
                        if (array[j].ParameterType != parameters2[num].ParameterType)
                        {
                            flag = false;
                            break;
                        }

                        num++;
                    }

                    if (flag) arrayList.Add(methodInfo.Name);
                }
            }

            return arrayList;
        }

        protected override ICollection GetCompatibleMethods(EventDescriptor e)
        {
            if (Source == null)
                for (var control = ((IDesignerHost) ServiceProvider.GetService(typeof(IDesignerHost))).RootComponent as Control; control != null; control = control.Parent)
                    if (control is Form)
                    {
                        Source = control;
                        break;
                    }

            if (Source == null) return new ArrayList();
            return GetCompatibleMethods(Source, e);
        }

        public override void FreeMethod(object component, EventDescriptor e, string methodName)
        {
            if (_loading) return;
            var eventDatas = GetEventDatas(component);
            if (eventDatas == null) return;
            foreach (var eventData in eventDatas)
                if (eventData.EventName == e.Name && eventData.MethodName == methodName)
                {
                    eventDatas.Remove(eventData);
                    break;
                }
        }

        public override void UseMethod(object component, EventDescriptor e, string methodName)
        {
            if (_loading) return;
            FreeMethod(component, e, methodName);
            AddEvent(component, e.Name, methodName);
        }

        public void Clear()
        {
            _events.Clear();
            ClearData();
        }

        private void UpdateEvents(object eventSource, bool adding)
        {
            if (eventSource == null) return;
            eventSource.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var keyValuePair in _events)
                if (keyValuePair.Value != null)
                {
                    var type = keyValuePair.Key.GetType();
                    foreach (var eventData in keyValuePair.Value)
                    {
                        var @event = type.GetEvent(eventData.EventName);
                        if (@event != null)
                        {
                            var @delegate = eventData.MethodDelegate;
                            if (@delegate == null)
                            {
                                @delegate = Delegate.CreateDelegate(@event.EventHandlerType, eventSource, eventData.MethodName, false, false);
                                eventData.MethodDelegate = @delegate;
                            }

                            if (@delegate != null)
                            {
                                @event.RemoveEventHandler(keyValuePair.Key, @delegate);
                                if (adding) @event.AddEventHandler(keyValuePair.Key, @delegate);
                            }
                        }
                    }
                }
        }

        public void BindEvents(object eventSource)
        {
            UpdateEvents(eventSource, true);
        }

        public void UnbindEvents(object eventSource)
        {
            UpdateEvents(eventSource, false);
        }

        public class EventData
        {
            public EventData(string eventName, string methodName)
            {
                EventName = eventName;
                MethodName = methodName;
            }

            public string EventName { get; set; }

            public string MethodName { get; set; }

            public Delegate MethodDelegate { get; set; }
        }
    }
}