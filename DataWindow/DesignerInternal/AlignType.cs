using System;

namespace DataWindow.DesignerInternal
{
    [Flags]
    public enum AlignType
    {
        Left = 1,
        Right = 2,
        Center = 4,
        Top = 8,
        Middle = 16,
        Bottom = 32
    }
}