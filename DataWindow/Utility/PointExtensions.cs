using System;
using System.Drawing;

namespace DataWindow.Utility
{
    internal static class PointExtensions
    {
        public static int Distance(this Point p1, Point p2)
        {
            var num = p1.X - p2.X;
            var num2 = p1.Y - p2.Y;
            return (int) Math.Sqrt(num * num + num2 * num2);
        }
    }
}