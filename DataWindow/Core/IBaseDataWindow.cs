using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Serialization;
using DataWindow.DesignLayer;
using DataWindow.Serialization.Components;

namespace DataWindow.Core
{
    public interface IBaseDataWindow
    {
        Designer GetDesigner();
        DefaultDesignerLoader GetDefaultDesignerLoader();
        void InitAllControl();

        void SetDefaultLayoutXml(string xml);
        string GetDefaultLayoutXml();

        void SetLayoutXml(string xml);
        string GetLayoutXml();

        List<Control> GetInherentControls();
        void AddInherentControls();
        void AddInherentControls(Control[] controls);
        bool IsInherentControl(Control con);
        Control GetInherentControl(string name);


        List<Control> GetMustEditControls();
        void AddMustControls(params Control[] cons);
        bool IsMustControl(Control control);
        bool IsMustControl(string name);

        List<Control> GetProhibitEditControls();
        void AddProhibitEditControls(params Control[] cons);
        bool IsProhibitEditControl(Control con);
        Control GetProhibitEditControl(string name);
        bool IsProhibitEditControl(string name);


        Dictionary<Control, string> GetControlTranslation();
        void AddControlTranslation(IDictionary<Control, string> translation);
        void AddControlTranslation(Control control, string translation);
    }
}