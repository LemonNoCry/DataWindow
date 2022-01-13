using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Forms;

namespace DataWindow.Utility
{
    internal static class ControlExtensions
    {
        public static Control FindControl(this Control control, string name)
        {
            if (control == null) return null;

            var enumerator = control.Controls.GetEnumerator();
            while (enumerator.MoveNext())
                if (((Control) enumerator.Current).Name == name)
                    return (Control) enumerator.Current;
            return null;
        }

        public static bool HaveParentInList(this Control control, List<Control> parentsList)
        {
            if (control == null) return false;
            for (var parent = control.Parent; parent != null; parent = parent.Parent)
                if (parentsList.Contains(parent))
                    return true;
            return false;
        }

        public static T GetProperty<T>(this Control control, string propertyName)
        {
            var propertyDescriptor = TypeDescriptor.GetProperties(control)[propertyName];
            if (propertyDescriptor != null)
                try
                {
                    return (T) propertyDescriptor.GetValue(control);
                }
                catch (Exception)
                {
                }

            return default;
        }

        public static void SetProperty(this Control control, string propertyName, object value)
        {
            var site = control.Site;
            if (site != null)
            {
                var propertyDescriptor = TypeDescriptor.GetProperties(control)[propertyName];
                if (propertyDescriptor != null)
                {
                    var designerTransaction = ((IDesignerHost) site.GetService(typeof(IDesignerHost))).CreateTransaction("Change " + propertyName + " " + control.Name);
                    if (designerTransaction == null)
                    {
                        propertyDescriptor.SetValue(control, value);
                        return;
                    }

                    using (designerTransaction)
                    {
                        propertyDescriptor.SetValue(control, value);
                        designerTransaction.Commit();
                    }
                }
            }
        }

        public static bool IsVisiable(this Control control)
        {
            try
            {
                return (bool) typeof(Control).InvokeMember("GetState", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, control, new object[]
                {
                    2
                });
            }
            catch (Exception)
            {
            }

            return false;
        }

        public static bool IsEnabled(this Control control)
        {
            var propertyDescriptor = TypeDescriptor.GetProperties(control).Find("Enabled", true);
            object obj;
            return (obj = propertyDescriptor != null ? propertyDescriptor.GetValue(control) : null) is bool && (bool) obj;
        }

        public static Control[] ToArray(this Control.ControlCollection controls)
        {
            var array = new Control[controls.Count];
            controls.CopyTo(array, 0);
            return array;
        }

        public static Control First(this Control.ControlCollection controls)
        {
            if (controls.Count == 0) return null;
            var tabIndex = controls[0].TabIndex;
            var index = 0;
            for (var i = 1; i < controls.Count; i++)
                if (controls[i].TabIndex < tabIndex)
                {
                    index = i;
                    tabIndex = controls[i].TabIndex;
                }

            return controls[index];
        }

        public static Control Last(this Control.ControlCollection controls)
        {
            if (controls.Count == 0) return null;
            var tabIndex = controls[0].TabIndex;
            var index = 0;
            for (var i = 1; i < controls.Count; i++)
                if (controls[i].TabIndex >= tabIndex)
                {
                    index = i;
                    tabIndex = controls[i].TabIndex;
                }

            return controls[index];
        }

        public static Control Next(this Control.ControlCollection controls, Control current)
        {
            var num = controls.IndexOf(current);
            var num2 = -1;
            var tabIndex = current.TabIndex;
            var num3 = 0;
            var flag = false;
            for (var i = 0; i < controls.Count; i++)
                if (controls[i].TabIndex >= tabIndex && (controls[i].TabIndex != tabIndex || i > num))
                {
                    if (controls[i].TabIndex == tabIndex)
                    {
                        num2 = i;
                        break;
                    }

                    if (!flag)
                    {
                        flag = true;
                        num3 = controls[i].TabIndex;
                        num2 = i;
                    }
                    else if (controls[i].TabIndex < num3)
                    {
                        num3 = controls[i].TabIndex;
                        num2 = i;
                    }
                }

            if (num2 >= 0) return controls[num2];
            return null;
        }

        public static Control Previous(this Control.ControlCollection controls, Control current)
        {
            var num = controls.IndexOf(current);
            var num2 = -1;
            var tabIndex = current.TabIndex;
            var num3 = 0;
            var flag = false;
            for (var i = 0; i < controls.Count; i++)
                if (controls[i].TabIndex <= tabIndex && (controls[i].TabIndex != tabIndex || i < num))
                {
                    if (controls[i].TabIndex == tabIndex)
                    {
                        num2 = i;
                        break;
                    }

                    if (!flag)
                    {
                        flag = true;
                        num3 = controls[i].TabIndex;
                        num2 = i;
                    }
                    else if (controls[i].TabIndex > num3)
                    {
                        num3 = controls[i].TabIndex;
                        num2 = i;
                    }
                }

            if (num2 >= 0) return controls[num2];
            return null;
        }
    }
}