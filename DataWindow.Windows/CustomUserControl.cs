using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataWindow.DesignLayer;
using DataWindow.Serialization.Components;

namespace DataWindow.Windows
{
    public partial class CustomUserControl : UserControl
    {
        public CustomUserControl()
        {
            InitializeComponent();

            defaultDesignerLoader = new DefaultDesignerLoader();
            designer = new Designer()
            {
                DesignedForm = this,
                DesignerLoader = defaultDesignerLoader,
            };
        }

        public Designer designer;
        public DefaultDesignerLoader defaultDesignerLoader;
    }
}