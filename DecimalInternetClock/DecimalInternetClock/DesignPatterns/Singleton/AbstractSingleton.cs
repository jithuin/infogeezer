using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecimalInternetClock.DesignPatterns.Singleton
{
    public abstract class AbstractSingleton 
    {
        static AbstractSingleton _instance;

        public static AbstractSingleton Instance
        {
            get { return _instance; }
            private set
            {
                if (_instance != null)
                    throw new InvalidOperationException("singleton");
                _instance = value;
            }
        }

        protected AbstractSingleton()
        {
            Instance = this;
        }
    }
}