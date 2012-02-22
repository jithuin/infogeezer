using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WindowsFormsApplication7
{
    class Fonts
    {
        //Static Members
        public static Font normal = new Font("Arial", 10f, FontStyle.Regular, GraphicsUnit.Pixel);
        public static Font bold = new Font("Arial", 10f, FontStyle.Bold, GraphicsUnit.Pixel);


        public static void Change(float size)
        {
            normal = new Font(normal.Name, size, normal.Style, GraphicsUnit.Pixel);
            bold = new Font(bold.Name, size, bold.Style, GraphicsUnit.Pixel);
            return;
        }
        public static void Change(String type)
        {
            normal = new Font(type, normal.Size, normal.Style, GraphicsUnit.Pixel);
            bold = new Font(type, bold.Size, bold.Style, GraphicsUnit.Pixel);
            return;
        }
        public static void Change(FontFamily type)
        {
            normal = new Font(type, normal.Size, normal.Style, GraphicsUnit.Point);
            bold = new Font(type, bold.Size, bold.Style, GraphicsUnit.Point);
            return;
        }
    }
}
