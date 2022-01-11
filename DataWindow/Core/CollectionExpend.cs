using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DataWindow.CustomPropertys;
using DataWindow.Serialization;
using Mapster;

namespace DataWindow.Core
{
    public static class CollectionExpend
    {
        /// <summary>
        /// 控件转换 ControlSerializable
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static ControlSerializable ControlConvertSerializable(this Control control)
        {
            return Collections.ControlConvertSerializable(control);
        }

        public static ControlSerializable[] ControlConvertSerializable(this IEnumerable<Control> controls)
        {
            return controls.Select(s => s.ControlConvertSerializable()).ToArray();
        }

        public static ControlSerializable ControlConvertAllSerializable(this Control control)
        {
            return DataWindowAnalysis.GetSerializationControls(control);
        }

        public static CustomPropertyCollection GetCollections(this Control control)
        {
            return Collections.GetCollections(control);
        }

        public static CustomPropertyCollection[] GetCollections(this IEnumerable<Control> controls)
        {
            return controls.Select(s => s.GetCollections()).ToArray();
        }

        public static void ControlSerializableToControl(this ControlSerializable cs, Control control)
        {
            dynamic dyn = cs;
            TypeAdapter.Adapt(dyn, control);
        }
    }
}