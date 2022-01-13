using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataWindow.DesignerInternal;
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

            designer.LayoutXml = xml;
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

            return designer.LayoutXml;
        }

        #endregion

        #region 初始化DataWindow

        public static void EachDataWindowControls(this Control control, Action<Control> action)
        {
            foreach (Control con in control.Controls)
            {
                action?.Invoke(con);
                if (con.HasChildren)
                {
                    EachDataWindowControls(con, action);
                }
            }
        }

        public static void CreateSite(DesignerHost host, Control control)
        {
            var designerSite = new DesignerSite(host, control);
            designerSite.Name = control.Name;

            control.Site = designerSite;
        }

        public static void InitAllControls(BaseDataWindow bdw)
        {
            if (bdw.Controls.Count < 1)
            {
                return;
            }

            EachDataWindowControls(bdw, (c) => { });
        }

        #endregion
    }
}