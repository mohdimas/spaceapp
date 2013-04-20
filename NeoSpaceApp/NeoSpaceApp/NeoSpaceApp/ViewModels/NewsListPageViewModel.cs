using HtmlAgilityPack;
using NeoSpaceApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;

namespace NeoSpaceApp.ViewModels
{
    public class NewsListPageViewModel : LoadableBase
    {
        private WebClient _wc = new WebClient();

        private int offset = 0;
        private int limit = 20;

        private ObservableCollection<NewsViewModel> _newsList;
        public ObservableCollection<NewsViewModel> NewsList
        {
            get { return _newsList; }
            set 
            { 
                _newsList = value; 
                NotifyPropertyChanged("NewsList"); 
            }
        }

        private NewsViewModel _firstNews;
        public NewsViewModel FirstNews
        {
            get { return _firstNews; }
            set { _firstNews = value; NotifyPropertyChanged("FirstNews"); }
        }

        private ObservableCollection<NewsViewModel> _secondaryNewsList;
        public ObservableCollection<NewsViewModel> SecondaryNewsList
        {
            get { return _secondaryNewsList; }
            set
            {
                _secondaryNewsList = value;
                NotifyPropertyChanged("SecondaryNewsList");
            }
        }

        public NewsListPageViewModel()
        {
            IsLoaded = NewsList != null && NewsList.Count != 0;
            PropertyChanged += NewsListPageViewModel_PropertyChanged;
        }

        void NewsListPageViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
            NewsList.Clear();
            Load();
        }

        public void LoadMore()
        {
            if (IsLoaded)
            {
                if (!IsBusy)
                {
                    IsBusy = true;
                    offset = NewsList.Count;
                    try
                    {
                        _wc.DownloadStringCompleted += wc_DownloadStringCompleted;
                        _wc.DownloadStringAsync(new Uri("http://neo.jpl.nasa.gov/news/"));
                    }
                    catch { }
                }
            }
        }

        public void Load()
        {
            if (!IsLoaded)
            {
                if (!IsBusy)
                {
                    IsBusy = true;
                    try
                    {
                        NewsList = new ObservableCollection<NewsViewModel>();
                        FirstNews = null;
                        SecondaryNewsList = null;

                        _wc.DownloadStringCompleted += wc_DownloadStringCompleted;
                        _wc.DownloadStringAsync(new Uri("http://neo.jpl.nasa.gov/news/"));
                    }
                    catch { IsBusy = false; }
                }
            }
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    ParseNews(e.Result);
                }
                catch { IsBusy = false; }
            }
            else
                IsBusy = false;
        }

        void ParseNews(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var newsListContainer = doc.DocumentNode.SelectSingleNode("//a[@name='news']");
            if (newsListContainer != null)
            {
                var outerTable = newsListContainer.SelectSingleNode("descendant::table");
                if (outerTable != null)
                {
                    var table = outerTable.SelectSingleNode("descendant::table");
                    if (table != null)
                    {
                        var rows = table.SelectNodes("descendant::tr");
                        if (rows != null)
                        {
                            var i = 0;
                            var count = 0;
                            foreach (var row in rows)
                            {
                                var field = row.SelectSingleNode("descendant::td[@align='left']");
                                if (field != null)
                                {
                                    var link = field.SelectSingleNode("descendant::a");
                                    if (link != null)
                                    {
                                        var news = new NewsViewModel();
                                        var test = row.InnerText;
                                        news.Load(link.Attributes["href"].Value);
                                        if (news.Model != null)
                                        {
                                            if (i >= offset - 1 && count < limit && !NewsList.Any(n => n.Model.Id == news.Model.Id))
                                            {
                                                NewsList.Add(news);
                                                count++;
                                            }
                                            i++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (NewsList.Count > 0)
            {
                FirstNews = NewsList[0];
                SecondaryNewsList = new ObservableCollection<NewsViewModel>(NewsList.Skip(1));
            }
            IsLoaded = true;
            IsBusy = false;
        }
    }
}
