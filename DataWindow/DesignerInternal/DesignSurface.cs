using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

namespace DataWindow.DesignerInternal
{
    internal class DesignSurface : UserControl
    {
        private Control _designedControl;

        private Control _savedParent;

        public DesignSurface()
        {
            _savedParent = Parent;
            if (_savedParent != null) _savedParent.SizeChanged += OnParentResize;
        }

        public override ISite Site
        {
            get => base.Site;
            set
            {
                IDesignerHost designerHost2;
                if (value != null)
                {
                    IDesignerHost designerHost;
                    if ((designerHost = value.GetService(typeof(IDesignerHost)) as IDesignerHost) != null) designerHost.AddService(typeof(DesignSurface), this);
                }
                else if (base.Site != null && (designerHost2 = base.Site.GetService(typeof(IDesignerHost)) as IDesignerHost) != null)
                {
                    designerHost2.RemoveService(typeof(DesignSurface));
                }

                base.Site = value;
            }
        }

        internal Control DesignedControl
        {
            set
            {
                if (_designedControl != null)
                {
                    _designedControl.BackColorChanged -= FormBackColorChanged;
                    _designedControl.BackgroundImageChanged -= FormBackgroundImageChanged;
                    _designedControl.BackgroundImageLayoutChanged -= FormBackgroundImageLayoutChanged;
                    _designedControl.FontChanged -= FormFontChanged;
                    _designedControl.ForeColorChanged -= FormForeColorChanged;
                    ScrollableControl scrollableControl;
                    if ((scrollableControl = _designedControl as ScrollableControl) != null) scrollableControl.AutoScroll = AutoScroll;
                }

                if (value != null)
                {
                    var control = value;
                    while (control != null && control.BackColor == Color.Transparent) control = control.Parent;
                    if (control != null)
                        BackColor = control.BackColor;
                    else
                        BackColor = SystemColors.Control;
                    BackgroundImage = value.BackgroundImage;
                    BackgroundImageLayout = value.BackgroundImageLayout;
                    Font = value.Font;
                    ForeColor = value.ForeColor;
                    value.BackColorChanged += FormBackColorChanged;
                    value.BackgroundImageChanged += FormBackgroundImageChanged;
                    value.BackgroundImageLayoutChanged += FormBackgroundImageLayoutChanged;
                    value.FontChanged += FormFontChanged;
                    value.ForeColorChanged += FormForeColorChanged;
                    ScrollableControl scrollableControl2;
                    if ((scrollableControl2 = value as ScrollableControl) != null)
                    {
                        AutoScroll = scrollableControl2.AutoScroll;
                        scrollableControl2.AutoScroll = false;
                    }
                }

                _designedControl = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_savedParent != null) _savedParent.SizeChanged -= OnParentResize;
            base.Dispose(disposing);
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            if (!AutoScroll && (Left != 0 || Top != 0))
            {
                Location = new Point(0, 0);
                ScrollableControl scrollableControl;
                if ((scrollableControl = Parent as ScrollableControl) != null && (scrollableControl.AutoScrollPosition.X != 0 || scrollableControl.AutoScrollPosition.Y != 0)) scrollableControl.AutoScrollPosition = new Point(0, 0);
            }

            base.OnLocationChanged(e);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            if (_savedParent != null) _savedParent.SizeChanged -= OnParentResize;
            _savedParent = Parent;
            if (Parent != null)
            {
                Parent.SizeChanged += OnParentResize;
                ScrollableControl scrollableControl;
                if ((scrollableControl = Parent as ScrollableControl) != null) scrollableControl.AutoScroll = false;
            }

            base.OnParentChanged(e);
        }

        private void FormForeColorChanged(object sender, EventArgs e)
        {
            ForeColor = _designedControl.ForeColor;
        }

        private void FormFontChanged(object sender, EventArgs e)
        {
            Font = _designedControl.Font;
        }

        private void FormBackgroundImageChanged(object sender, EventArgs e)
        {
            BackgroundImage = _designedControl.BackgroundImage;
        }

        private void FormBackgroundImageLayoutChanged(object sender, EventArgs e)
        {
            BackgroundImageLayout = _designedControl.BackgroundImageLayout;
        }

        private void FormBackColorChanged(object sender, EventArgs e)
        {
            BackColor = _designedControl.BackColor;
        }

        private void OnParentResize(object o, EventArgs e)
        {
            SuspendLayout();
            var selectionService = (ISelectionService) Site.GetService(typeof(ISelectionService));
            var selectedComponents = selectionService.GetSelectedComponents();
            selectionService.SetSelectedComponents(null);
            Size = ((Control) o).ClientSize;
            Location = new Point(0, 0);
            selectionService.SetSelectedComponents(selectedComponents);
            ResumeLayout();
        }
    }
}