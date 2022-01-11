using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DataWindow.Utility;

namespace DataWindow.Toolbox
{
    internal class ToolboxList : ListBox
    {
        private readonly int DragDistance = 3;

        private int _selectedIndex = -1;

        private int itemUnderMouse = -1;

        private Point mouseClickOrigin;

        [Category("Appearance")]
        [DefaultValue("ActiveCaption")]
        public Color ItemHoverBackColor { get; set; } = SystemColors.ActiveCaption;

        [Category("Appearance")]
        [DefaultValue("GradientActiveCaption")]
        public Color SelectedItemHoverBackColor { get; set; } = SystemColors.GradientActiveCaption;

        [Category("Appearance")]
        [DefaultValue("GradientInactiveCaption")]
        public Color SelectedItemBackColor { get; set; } = SystemColors.GradientInactiveCaption;

        [Category("Appearance")]
        [DefaultValue("ActiveBorder")]
        public Color SelectedItemBorderColor { get; set; } = SystemColors.ActiveBorder;

        [Category("Appearance")]
        [DefaultValue("Window")]
        public Color GroupBackColor { get; set; } = SystemColors.Window;

        [Category("Appearance")]
        public ImageList Images { get; set; }

        [Browsable(false)]
        [DefaultValue(null)]
        public new ToolboxBaseItem SelectedItem
        {
            get
            {
                if (_selectedIndex < 0) return null;
                return Items[_selectedIndex] as ToolboxBaseItem;
            }
            set
            {
                var num = -1;
                if (value != null) num = Items.IndexOf(value);
                if (num != _selectedIndex) ChangeSelection(num);
            }
        }

        public override int SelectedIndex
        {
            get => _selectedIndex;
            set => ChangeSelection(value);
        }

        public event EventHandler<ToolboxItemDragEventArgs> ItemDrag;

        private void ChangeSelection(int newSelectedIndex)
        {
            if (newSelectedIndex == _selectedIndex) return;
            if (_selectedIndex >= 0)
            {
                var selectedIndex = _selectedIndex;
                _selectedIndex = -1;
                PaintItem(selectedIndex, false, null);
            }

            if (newSelectedIndex >= 0)
            {
                _selectedIndex = newSelectedIndex;
                PaintItem(_selectedIndex, false, null);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != MouseButtons.None)
            {
                if (_selectedIndex >= 0 && mouseClickOrigin.Distance(MousePosition) > DragDistance && ItemDrag != null)
                {
                    var item = Items[_selectedIndex] as ToolboxBaseItem;
                    ItemDrag(this, new ToolboxItemDragEventArgs(item));
                }

                return;
            }

            var pt = PointToClient(MousePosition);
            var itemAt = GetItemAt(pt);
            if (itemAt != -1 && itemUnderMouse != itemAt)
            {
                var obj = Items[itemAt];
                if (itemUnderMouse != -1) PaintItem(itemUnderMouse, false, null);
                itemUnderMouse = itemAt;
                PaintItem(itemUnderMouse, true, null);
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (itemUnderMouse != -1)
            {
                PaintItem(itemUnderMouse, false, null);
                itemUnderMouse = -1;
            }

            base.OnMouseLeave(e);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= Items.Count) return;
            var pt = PointToClient(MousePosition);
            var hover = GetItemRectangle(e.Index).Contains(pt) & (MouseButtons == MouseButtons.None);
            PaintItem(e.Index, hover, e.Graphics);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            mouseClickOrigin = MousePosition;
            var itemAt = GetItemAt(PointToClient(MousePosition));
            if (itemAt >= 0) ChangeSelection(itemAt);
            base.OnMouseDown(e);
        }

        protected void PaintItem(int index, bool hover, Graphics graphics)
        {
            var graphics2 = graphics == null ? Graphics.FromHwnd(Handle) : graphics;
            var itemRectangle = GetItemRectangle(index);
            var flag = index == _selectedIndex;
            var toolboxBaseItem = Items[index] as ToolboxBaseItem;
            var text = toolboxBaseItem.ShowDisplayText();
            hover = hover && !toolboxBaseItem.IsGroup;
            var color = BackColor;
            if (toolboxBaseItem.IsGroup && flag)
                color = SelectedItemBackColor;
            else if (toolboxBaseItem.IsGroup)
                color = GroupBackColor;
            else if (hover && flag)
                color = SelectedItemHoverBackColor;
            else if (hover)
                color = ItemHoverBackColor;
            else if (flag) color = SelectedItemBackColor;
            var solidBrush = new SolidBrush(color);
            var solidBrush2 = new SolidBrush(ForeColor);
            graphics2.FillRectangle(solidBrush, itemRectangle);
            if (hover || flag)
                using (var pen = new Pen(SelectedItemBorderColor, 1f))
                {
                    itemRectangle.Size = new Size(itemRectangle.Size.Width - 1, itemRectangle.Size.Height - 1);
                    graphics2.DrawRectangle(pen, itemRectangle);
                }

            itemRectangle = GetItemRectangle(index);
            var num = 0;
            if (Images != null)
            {
                var height = Images.ImageSize.Height;
                num = Images.ImageSize.Width;
                var num2 = toolboxBaseItem.IsGroup ? 2 : 4;
                var num3 = itemRectangle.Left + num2;
                if (RightToLeft == RightToLeft.Yes) num3 = itemRectangle.Right - num2 - num;
                if (toolboxBaseItem.ImageIndex >= 0) Images.Draw(graphics2, num3 + 4, itemRectangle.Top + num2, num, height, toolboxBaseItem.ImageIndex);
            }

            var sizeF = graphics2.MeasureString(text, Font);
            var num4 = (itemRectangle.Height - (int) sizeF.Height) / 2;
            var num5 = 1;
            var font = Font;
            if (toolboxBaseItem.IsGroup) font = new Font(font.FontFamily, font.Size, FontStyle.Bold);
            var stringFormat = new StringFormat();
            if (RightToLeft == RightToLeft.Yes)
            {
                stringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                itemRectangle.Size = new Size(itemRectangle.Width - num - 2, itemRectangle.Height);
            }
            else
            {
                itemRectangle.Location = new Point(itemRectangle.Left + num + 10, itemRectangle.Top);
            }

            itemRectangle.Inflate(-num5, -num4);
            graphics2.DrawString(text, font, solidBrush2, itemRectangle, stringFormat);
            solidBrush2.Dispose();
            solidBrush.Dispose();
            if (toolboxBaseItem.IsGroup) font.Dispose();
            if (graphics == null) graphics2.Dispose();
        }

        private int GetItemAt(Point pt)
        {
            var i = TopIndex;
            var count = Items.Count;
            while (i < count)
            {
                if (GetItemRectangle(i).Contains(pt)) return i;
                i++;
            }

            return -1;
        }
    }
}