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
    public class BaseDataWindowSerializable : ControlSerializable, IPropertyCollections<BaseDataWindow>,IHostCreateCommpent
    {
        public void CopyPropertyCommpent<BaseDataWindow>(BaseDataWindow source, BaseDataWindow target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.InherentControls.Clear();
            target.InherentControls.AddRange(source.InherentControls);

            target.MustEditControls.Clear();
            target.MustEditControls.AddRange(source.MustEditControls);

            target.ControlTranslation.Clear();
            foreach (var key in source.ControlTranslation)
                target.ControlTranslation.Add(key.Key,key.Value);

            target.SetDefaultLayoutXml(source.GetDefaultLayoutXml());
        }

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