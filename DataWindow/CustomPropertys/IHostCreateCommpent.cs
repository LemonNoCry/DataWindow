using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DataWindow.CustomPropertys
{
    public interface IHostCreateCommpent
    {
        void CopyPropertyCommpent<T>(T source,T target);
    }
}
