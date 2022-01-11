using System.ComponentModel.Design;
using System.Drawing;

namespace DataWindow.DesignerInternal
{
    internal class IDesignerOptionServiceImpl : IDesignerOptionService
    {
        private Size gridSize = new Size(8, 8);

        private bool showGrid;

        private bool snapToGrid = true;

        public void SetOptionValue(string pageName, string valueName, object value)
        {
            if (pageName.IndexOf("WindowsFormsDesigner\\General") != -1)
            {
                if (valueName == "GridSize")
                {
                    gridSize = (Size) value;
                    return;
                }

                if (valueName == "GridSize.Width")
                {
                    gridSize.Width = (int) value;
                    return;
                }

                if (valueName == "GridSize.Height")
                {
                    gridSize.Height = (int) value;
                    return;
                }

                if (valueName == "ShowGrid")
                {
                    showGrid = (bool) value;
                    return;
                }

                if (valueName == "SnapToGrid") snapToGrid = (bool) value;
            }
        }

        public object GetOptionValue(string pageName, string valueName)
        {
            object result;
            if (pageName.IndexOf("WindowsFormsDesigner\\General") == -1)
                result = null;
            else if (valueName == "GridSize")
                result = gridSize;
            else if (valueName == "GridSize.Width")
                result = gridSize.Width;
            else if (valueName == "GridSize.Height")
                result = gridSize.Height;
            else if (valueName == "ShowGrid")
                result = showGrid;
            else if (valueName == "SnapToGrid")
                result = snapToGrid;
            else
                result = null;
            return result;
        }
    }
}