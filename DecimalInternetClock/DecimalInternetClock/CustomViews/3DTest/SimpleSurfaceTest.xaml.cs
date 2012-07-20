using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows;
using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using System.Windows.Media.Media3D;

namespace DecimalInternetClock.Views
{
    /// <summary>
    /// Interaction logic for SimpleSurfaceTest.xaml
    /// </summary>
    public partial class SimpleSurfaceTest : UserControl
    {
        public SimpleSurfaceTest()
        {
            InitializeComponent();
            //ss.IsHiddenLine = false;
            //ss.Viewport3d = viewport;
            //AddSinc();
        }
//        private void AddSinc()
//        {
//            ss.Xmin = -8;
//            ss.Xmax = 8;
//            ss.Zmin = -8;
//            ss.Zmax = 8;
//            ss.Ymin = -1;
//            ss.Ymax = 1;
//            ss.CreateSurface(Sinc);
//        }
//        private Point3D Sinc(double x, double z)
//        {
//            double r = Math.Sqrt(x * x + z * z) + 0.00001;
//            double y = Math.Sin(r) / r;
//            return new Point3D(x, y, z);
//        }
    }
}
