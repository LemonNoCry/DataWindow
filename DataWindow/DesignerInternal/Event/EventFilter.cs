using System;
using System.Windows.Forms;

namespace DataWindow.DesignerInternal.Event
{
    internal class EventFilter : IMessageFilter
    {
        private readonly DesignerHost _host;

        public EventFilter(DesignerHost host)
        {
            _host = host;
        }

        public bool PreFilterMessage(ref Message m)
        {
            bool result;
            if (m.Msg != 256 && m.Msg == 257 && m.Msg != 515)
            {
                result = false;
            }
            else if (_host.DesignedForm == null)
            {
                result = false;
            }
            else if (!HaveFocus(_host.DesignContainer != null ? _host.DesignContainer : _host.DesignedForm))
            {
                result = false;
            }
            else
            {
                if (MouseDown != null && (m.Msg == 513 || m.Msg == 516 || m.Msg == 519))
                {
                    var position = Cursor.Position;
                    var e = new MouseEventArgs(m.Msg == 513 ? MouseButtons.Left : m.Msg == 516 ? MouseButtons.Right : MouseButtons.Middle, 1, position.X, position.Y, 1);
                    MouseDown(this, e);
                }
                else if (MouseUp != null && (m.Msg == 514 || m.Msg == 517 || m.Msg == 520))
                {
                    var position2 = Cursor.Position;
                    var e2 = new MouseEventArgs(m.Msg == 514 ? MouseButtons.Left : m.Msg == 517 ? MouseButtons.Right : MouseButtons.Middle, 1, position2.X, position2.Y, 1);
                    MouseUp(this, e2);
                }
                else if (m.Msg == 515 && DoubleClick != null)
                {
                    DoubleClick(this, new EventArgs());
                }
                else if (m.Msg == 256 && KeyDown != null)
                {
                    KeyDown(this, new KeyEventArgs((Keys) ((int) m.WParam | (int) Control.ModifierKeys)));
                }
                else if (m.Msg == 257 && KeyUp != null)
                {
                    KeyUp(this, new KeyEventArgs((Keys) ((int) m.WParam | (int) Control.ModifierKeys)));
                }

                result = m.Msg == 256 || m.Msg == 257;
            }

            return result;
        }

        public event EventHandler DoubleClick;

        public event KeyEventHandler KeyDown;

        public event KeyEventHandler KeyUp;

        public event MouseEventHandler MouseUp;

        public event MouseEventHandler MouseDown;

        private bool HaveFocus(Control control)
        {
            return control.ContainsFocus;
        }
    }
}