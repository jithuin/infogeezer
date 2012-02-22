using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ScreenWorm
{
    /// <summary>
    /// Interaction logic for Worm.xaml
    /// </summary>
    public partial class Worm : UserControl
    {
        private static int seed = 0;
        private static Random rnd = new Random(seed);
        private static readonly int WormSegment = 5;

        public Worm()
        {
            Position = InitialList();
            InitializeComponent();
            Velocity = new MyVector(10 * rnd.NextDouble(), 10 * rnd.NextDouble());
            seed = rnd.Next();
            _timer.AutoReset = true;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Start();
            
        }
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() => { nextPoint(); }));
        }

        //static public MyPoint LastPosition { get; set; }
        private void nextPoint()
        {
            double x, y;
            x = (rnd.NextDouble() - 0.1) * Velocity.X;
            y = (rnd.NextDouble() - 0.1) * Velocity.Y;
            if (Position[0].X + x < 0 || Position[0].X + x > this.ActualWidth)
            {
                x = -x;
                Velocity.X = -Velocity.X;
            }
            if (Position[0].Y + y < 0 || Position[0].Y + y > this.ActualHeight)
            {
                y = -y;
                Velocity.Y = -Velocity.Y;
            }

            for (int i = WormSegment - 1; i >= 1; i--)
            {
                Position[i].X = Position[i - 1].X;
                Position[i].Y = Position[i - 1].Y;
            }
            Position[0].X += x;
            Position[0].Y += y;
        }
        Timer _timer = new Timer(50);

        

        public MyVector Velocity { get; set; }

        public ObservableCollection<MyPoint> Position { get; set; }
        //{
        //    get { return (List<MyPoint>)GetValue(PositionProperty); }
        //    set { SetValue(PositionProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty PositionProperty =
        //    DependencyProperty.Register("Position", typeof(List<MyPoint>), typeof(Window1), new UIPropertyMetadata(null));

        private static ObservableCollection<MyPoint> InitialList()
        {
            ObservableCollection<MyPoint> ret = new ObservableCollection<MyPoint>();
            Random rnd = new Random(seed);
            //if (LastPosition == null)
            //{
            //    LastPosition = new MyPoint();
            //}
            MyPoint point;
            point = new MyPoint(rnd.NextDouble() * 300, rnd.NextDouble() * 300);
            for (int i = 0; i < WormSegment; i++)
            {
                //point = new MyPoint(rnd.NextDouble() * 300 + LastPosition.X, rnd.NextDouble() * 300 + LastPosition.Y);
                
                ret.Add(new MyPoint(point.X, point.Y));
            }
            //LastPosition.X = point.X;
            //LastPosition.Y = point.Y;
            seed = rnd.Next();
            return ret;
        }
    }

    public class MyPoint : INotifyPropertyChanged
    {
        public MyPoint()
        {
            x = 0;
            y = 0;
        }
        public MyPoint(double x_in, double y_in)
        {
            x = x_in;
            y = y_in;
        }
        private double x, y;
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                if (value != x)
                {
                    OnPropertyChanged("X");
                    x = value;
                }
            }
        }
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                if (value != y)
                {
                    OnPropertyChanged("Y");
                    y = value;
                }
            }
        }


        #region INotifyPropertyChanged Members
        protected void OnPropertyChanged(String PropName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    public class MyVector : MyPoint
    {
        public MyVector() : base() { ;}
        public MyVector(double x, double y) : base(x, y) { ;}

        private double _length; // helytelen: ezt számolni kellene
        public double Length
        {
            get { return _length; }
            set
            {
                if (_length != value)
                {
                    OnPropertyChanged("Length");
                    _length = value;
                }
            }
        }
    }
}
