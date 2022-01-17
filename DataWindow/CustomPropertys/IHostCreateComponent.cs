using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DataWindow.CustomPropertys
{
    public interface IHostCreateComponent<in T>
    {
        void CopyPropertyComponent(T source, T target);
    }
}