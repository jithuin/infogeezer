using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecimalInternetClock.Helpers;
using DecimalInternetClock.HotKeys;
using DecimalInternetClock.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace DecimalInternetClock.Configuration
{
    public class MainConfiguration
    {
        private SimpleIoc Container = new SimpleIoc();

        public MainConfiguration()
        {
            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    Container.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    Container.Register<IDataService, DataService>();
            ////}

            Container.Register<MainOptions>();

            ResizerHotkeyList.Instance.SetToDefault(); // TODO integrate to the IOC model
        }

        public MainOptions MainOptions
        {
            get
            {
                return Container.GetInstance<MainOptions>();
            }
        }
    }
}