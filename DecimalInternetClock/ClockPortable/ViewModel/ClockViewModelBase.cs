using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Clocks.Exceptions;
using Clocks.Model;
using HelpersPortable.Exceptions;

namespace Clocks.ViewModel
{
    public class ClockViewModelBase<T, E> : INotifyPropertyChanged
        where T : ClockBase<E>, new()
        where E : struct, IComparable, IFormattable
    {
        protected T _clock;

        public string TimeStringFormat
        {
            get;
            set;
        }

        protected const string TimeStringPropertyName = "TimeString";

        /// <summary>
        /// Public read-only property for UI to display decimal time
        /// </summary>
        public virtual string TimeString
        {
            get
            {
                int length = Enum.GetValues(typeof(E)).Length;
                object[] args = new object[length];
                for (int i = 0; i < length; i++)
                    args[i] = _clock[i];
                return String.Format(TimeStringFormat, args);
                
            }
        }

        public ClockViewModelBase()
            : this(new T())
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="clock_in">set the internal time representation to the given object</param>
        public ClockViewModelBase(T clock_in)
        {
            //if (!typeof(E).IsEnum)
            //    throw new XArchitectureException();
            _clock = clock_in;
            if (TimeStringFormat == null || TimeStringFormat == string.Empty || TimeStringFormat == "")
            {
                StringBuilder sb = new StringBuilder();
                int indexOfLastItem = Enum.GetValues(typeof(E)).Length - 1;
                for (int i = 0; i < indexOfLastItem; i++)
                    sb.AppendFormat("{{{0}}}.", i);
                sb.AppendFormat("{{{0}}}", indexOfLastItem); // the last element should not contain a trailing '.'(period).
                TimeStringFormat = sb.ToString();
            }
            if (TimeStringFormat.Count((c) => c == '{') != Enum.GetValues(typeof(E)).Length)
                throw new TimeStringFormatMismatchException();
        }

        /// <summary>
        /// Function to call to refresh the internal time representation of this viewmodel
        /// </summary>
        public virtual void UpdateNow()
        {
            _clock.UpdateNow();
            OnPropertyChanged(TimeStringPropertyName); //TODO: promote the string to a protected field
        }

        #region INotifyPropertyChanged Members

        public void OnPropertyChanged(String propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members
    }
}