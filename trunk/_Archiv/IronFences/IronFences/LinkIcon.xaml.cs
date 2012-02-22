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
using System.IO;
using System.Diagnostics;
using System.Timers;
using IronFences.Properties;

namespace IronFences
{
    /// <summary>
    /// Interaction logic for LinkIcon.xaml
    /// </summary>
    public partial class LinkIcon : UserControl
    {

        public LinkIcon()
        {
            InitializeComponent();
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }

        public String IconFile
        {
            get { return (String)GetValue(IconFileProperty); }
            set { SetValue(IconFileProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconFile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconFileProperty =
            DependencyProperty.Register("IconFile", typeof(String), typeof(LinkIcon), new UIPropertyMetadata(""));



        public String Link
        {
            get { return (String)GetValue(LinkProperty); }
            set { SetValue(LinkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Link.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LinkProperty =
            DependencyProperty.Register("Link", typeof(String), typeof(LinkIcon), new UIPropertyMetadata(""));

        private bool EnableClick = false;
        private bool IsSecondClick = false;
        private Timer timer = new Timer(Settings.Default.DoubleClickInterval);

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (EnableClick)
            {
                IsSecondClick = true;
            }
            else
            {
                EnableClick = true;
                timer.Start();
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            EnableClick = false;
            IsSecondClick = false;
        }

        private void StackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsSecondClick)
            {
                Process.Start(this.Link);
                timer.Stop();
                EnableClick = false;
                IsSecondClick = false;
            }
        }



    }


}
