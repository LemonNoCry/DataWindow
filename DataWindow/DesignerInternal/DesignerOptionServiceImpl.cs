using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Design;

namespace DataWindow.DesignerInternal
{
    internal class DesignerOptionServiceImpl : DesignerOptionService
    {
        protected override void PopulateOptionCollection(DesignerOptionCollection options)
        {
            if (options.Parent == null)
                CreateOptionCollection(options, "DesignerOptions", new DesignerOptions
                {
                    GridSize = new Size(8, 8),
                    ShowGrid = false,
                    UseSmartTags = true,
                    UseSnapLines = true,
                    ObjectBoundSmartTagAutoShow = true,
                    EnableInSituEditing = true,
                    SnapToGrid = true,
                    UseOptimizedCodeGeneration = false
                });
        }
    }
}