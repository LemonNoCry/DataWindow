using WeifenLuo.WinFormsUI.Docking;

namespace DataWindow.Windows.Dock
{
    public partial class PropertyWindow : DockContent
    {
        public PropertyWindow()
        {
            InitializeComponent();
            this.Propertybox.ShowEventTab = true;
        }

    }
}
