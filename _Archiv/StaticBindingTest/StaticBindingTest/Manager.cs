using System.ComponentModel;

namespace StaticBindingTest.Storage
{
    public class Manager : INotifyPropertyChanged
    {
        private Manager() { }

        #region IsOnline

        private bool _isOnline = false;

        public bool IsOnline
        {
            get { return _isOnline; }
            set
            {
                if (_isOnline != value)
                {
                    _isOnline = value;
                    RaisePropertyChanged("IsOnline");
                }
            }
        }

        #endregion

        #region Instance

        private static Manager _instance;

        public static Manager Instance
        {
            get 
            { 
                if (_instance == null)
                    _instance = new Manager();
                return _instance;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
