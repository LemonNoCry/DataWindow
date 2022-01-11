using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataWindow.DesignLayer;
using DataWindow.Serialization;
using DataWindow.Serialization.Components;
using DataWindow.Utility;

namespace DataWindow.Core
{
    public static class DataWindowAnalysis
    {
        public static Encoding Encoding = Encoding.UTF8;

        public static ControlSerializable GetSerializationControls(Control control)
        {
            var sc = Collections.ControlConvertSerializable(control);
            //if (control.Parent != null)
            //{
            //    sc.ParentSerializable = Collections.ControlConvertSerializable(control.Parent);
            //}

            if (control.HasChildren)
            {
                sc.ControlsSerializable = new List<ControlSerializable>();
                foreach (Control con in control.Controls)
                {
                    var cs = GetSerializationControls(con);
                    sc.ControlsSerializable.Add(cs);
                }
            }

            return sc;
        }

        #region 序列化

        public static string SerializationControls(Control control)
        {
            return GetSerializationControls(control).XmlSerialize(Encoding);
        }

        public static string SerializationControls(ControlSerializable controlSerializable)
        {
            return controlSerializable.XmlSerialize(Encoding);
        }

        public static void SerializationControls(Control control, string path)
        {
            GetSerializationControls(control).XmlSerializeToFile(path, Encoding);
        }

        public static ControlSerializable DeserializeControls(string xml)
        {
            return XmlSerializeUtility.XmlDeserialize<ControlSerializable>(xml, Encoding);
        }

        public static ControlSerializable DeserializeControlsForPath(string path)
        {
            return XmlSerializeUtility.XmlDeserializeFromFile<ControlSerializable>(path, Encoding);
        }

        #endregion

        #region 解析xml

        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="control">父控件</param>
        /// <param name="xml">xml</param>
        public static void ResolveToDataWindow(Control control, string xml)
        {
            DefaultDesignerLoader defaultDesignerLoader = new DefaultDesignerLoader();
            Designer designer = new Designer();
            designer.DesignerLoader = defaultDesignerLoader;
            designer.DesignedForm = control;

            designer.LayoutXML = xml;
        }

        /// <summary>
        /// 序列化Control
        /// </summary>
        /// <param name="control">父控件</param>
        /// <returns>xml</returns>
        public static string SerializationDataWindow(Control control)
        {
            DefaultDesignerLoader defaultDesignerLoader = new DefaultDesignerLoader();
            Designer designer = new Designer();
            designer.DesignerLoader = defaultDesignerLoader;
            designer.DesignContainer = control;

            return designer.LayoutXML;
        }

        //public static void ResolveToOverlayer(ControlSerializable cs, Overlayer overlayer)
        //{
        //    overlayer.Reset();

        //    overlayer.BaseDataWindow.SuspendLayout();
        //    cs.ControlSerializableToControl(overlayer.BaseDataWindow);
        //    ResolveToOverlayerChild(cs.ControlsSerializable, overlayer.BaseDataWindow, overlayer);
        //    overlayer.BaseDataWindow.ResumeLayout(true);
        //}

        //private static void ClearDataWindowControls(Control control)
        //{
        //    foreach (Control con in control.Controls)
        //    {
        //        if (con.HasChildren)
        //        {
        //            ClearDataWindowControls(con);
        //        }
        //    }

        //    control.Controls.Clear();
        //}

        //public static void ResolveToDataWindow(ControlSerializable cs, BaseDataWindow baseDataWindow)
        //{
        //    baseDataWindow.Text = cs.Text;

        //    baseDataWindow.SuspendLayout();
        //    ClearDataWindowControls(baseDataWindow);

        //    ResolveToOverlayerChild(cs.ControlsSerializable, baseDataWindow, baseDataWindow);
        //    baseDataWindow.ResumeLayout(true);
        //}

        //public static void ResolveToOverlayerChild(List<ControlSerializable> css, Control control, Overlayer overlayer)
        //{
        //    foreach (var child in css)
        //    {
        //        var con = overlayer.BaseDataWindow.GetInherentControl(child.Name) ?? ControlHelper.CreateControl(child);

        //        child.ControlSerializableToControl(con);

        //        overlayer.SetControlProperty(con);
        //        control.Controls.Add(con);

        //        if (child.ControlsSerializable != null && child.ControlsSerializable.Any())
        //        {
        //            ResolveToOverlayerChild(child.ControlsSerializable, con, overlayer);
        //        }
        //    }
        //}

        //public static void ResolveToOverlayerChild(List<ControlSerializable> css, Control control, BaseDataWindow baseDataWindow)
        //{
        //    foreach (var child in css)
        //    {
        //        var con = baseDataWindow.GetInherentControl(child.Name) ?? ControlHelper.CreateControl(child);

        //        control.Controls.Add(con);
        //        child.ControlSerializableToControl(con);
        //        con.Visible = true;

        //        if (child.ControlsSerializable != null && child.ControlsSerializable.Any())
        //        {
        //            ResolveToOverlayerChild(child.ControlsSerializable, con, baseDataWindow);
        //        }
        //    }
        //}

        #endregion
    }
}