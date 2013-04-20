using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.Serialization;


namespace NeoSpaceApp.Models
{
    public class LoadableBase : ModelBase
    {
        private Boolean _isLoaded;

        public Boolean IsLoaded
        {
            get { return _isLoaded; }
            set { _isLoaded = value; NotifyPropertyChanged("IsLoaded"); }
        }

        private Boolean _isBusy;

        public Boolean IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged("IsBusy"); }
        }
        
        private Boolean _isError;

        public Boolean IsError
        {
            get { return _isError; }
            set { _isError = value; NotifyPropertyChanged("IsError"); }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; NotifyPropertyChanged("Message"); }
        }

        private Visibility _progressBarVisibility;

        public Visibility ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                _progressBarVisibility = value;
                NotifyPropertyChanged("ProgressBarVisibility");
            }
        }
    }
}
