using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Windows.UI.Xaml;

namespace HelpersPortable.WPF
{
    /// <summary>
    /// Class for Binding read-only WPF properties
    /// </summary>
    public class DataPiping
    {
        #region DataPipes (Attached DependencyProperty)

        public static readonly DependencyProperty DataPipesProperty =
            DependencyProperty.RegisterAttached("DataPipes",
            typeof(DataPipeCollection),
            typeof(DataPiping),
            new PropertyMetadata(null));

        public static void SetDataPipes(DependencyObject o, DataPipeCollection value)
        {
            o.SetValue(DataPipesProperty, value);
        }

        public static DataPipeCollection GetDataPipes(DependencyObject o)
        {
            return (DataPipeCollection)o.GetValue(DataPipesProperty);
        }

        #endregion DataPipes (Attached DependencyProperty)
    }

    public class DataPipeCollection : ObservableCollection<DataPipe>
    {
    }

    public class DataPipe : DependencyObject
    {
        #region Source (DependencyProperty)

        public object Source
        {
            get { return (object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(DataPipe),
            new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataPipe)d).OnSourceChanged(e);
        }

        protected virtual void OnSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            Target = e.NewValue;
        }

        #endregion Source (DependencyProperty)

        #region Target (DependencyProperty)

        public object Target
        {
            get { return (object)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(object), typeof(DataPipe),
            new PropertyMetadata(null));

        #endregion Target (DependencyProperty)

        //protected override DependencyObject CreateInstanceCore()
        //{
        //    return new DataPipe();
        //}
    }
}