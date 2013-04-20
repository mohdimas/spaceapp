using NeoSpaceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoSpaceApp.ViewModels
{
    public class MainViewModel : ModelBase
    {
        private NewsListPageViewModel _newsPage;
        public NewsListPageViewModel NewsPage
        {
            get { return _newsPage; }
            set { _newsPage = value; NotifyPropertyChanged("NewsPage"); }
        }
    }
}
