using System;
using System.Windows.Forms;
using DataWindow.CustomPropertys;
using DataWindow.DesignLayer;

namespace DataWindow.Serialization
{
    ///<summary> 
    ///这里要添加对序列化的支持 
    ///</summary> 
    [Serializable]
    public class BaseDataWindowSerializable : ControlSerializable, IPropertyCollections<BaseDataWindow>
    {
        public override CustomPropertyCollection GetCollections(Control control)
        {
            return GetCollections(control as BaseDataWindow);
        }

        public CustomPropertyCollection GetCollections(BaseDataWindow control)
        {
            var cpc = base.GetCollections(control);


            return cpc;
        }
    }
}