using NeoSpaceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NeoSpaceApp.ViewModels
{
    public class NewsPageViewModel : LoadableBase
    {
        private string _id;
        public string Id
        {
            get { return _id; }
            set { _id = value; NotifyPropertyChanged("Id"); }
        }

        private NewsViewModel _news;
        public NewsViewModel News
        {
            get { return _news; }
            set { _news = value; NotifyPropertyChanged("News"); }
        }

        public NewsPageViewModel()
        {
            IsLoaded = News != null && News.Model != null && !string.IsNullOrEmpty(News.Model.Content);
            PropertyChanged += NewsPageViewModel_PropertyChanged;
        }

        void NewsPageViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsBusy")
            {
                if (IsBusy)
                    ProgressBarVisibility = Visibility.Visible;
                else
                    ProgressBarVisibility = Visibility.Collapsed;
            }
        }

        public void Refresh()
        {
            IsLoaded = false;
            IsBusy = false;
            News = null;
            Load(Id);
        }


        public void Load(string id)
        {
            if (!IsLoaded)
            {
                if (!IsBusy)
                {
                    IsBusy = true;
                    try
                    {
                        Id = id;
                        News = new NewsViewModel();
                        News.LoadCompleted += News_LoadCompleted;
                        News.Load(id);
                    }
                    catch { IsBusy = false; }
                }
            }
        }

        void News_LoadCompleted(object sender, EventArgs e)
        {
            IsBusy = false;
            IsLoaded = true;
        }
    }
}
