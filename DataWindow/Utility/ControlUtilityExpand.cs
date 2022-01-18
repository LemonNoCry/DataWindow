using System;
using System.Windows.Forms;

namespace DataWindow.Utility
{
    public static class ControlUtilityExpand
    {
        public static bool IsContainerControl(this Control con)
        {
            switch (con)
            {
                case Panel _:
                case ScrollableControl _:
                    return true;
            }

            return false;
        }
        
        public static Type GetControlRealType(this Type type) 
        {
            if(type == null&&type.IsAssignableFrom(typeof(Control)))
            {
                return null;
            }
            if (type.Assembly.FullName == typeof(Control).Assembly.FullName)
            {
                return type;
            }
            return GetControlRealType(type.BaseType);
        }
    }
}