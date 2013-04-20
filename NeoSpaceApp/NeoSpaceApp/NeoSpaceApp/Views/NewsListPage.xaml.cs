using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Clarity.Phone.Controls;
using Clarity.Phone.Controls.Animations;
using System.Collections.ObjectModel;
using Microsoft.Phone.Shell;
using NeoSpaceApp.ViewModels;
using NeoSpaceApp.Extensions.Helpers;

namespace NeoSpaceApp.Views
{
    public partial class NewsListPage : AnimatedBasePage
    {
        private NewsListPageViewModel _vm;

        // Constructor
        public NewsListPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;

            _vm = App.MainViewModel.NewsPage = new NewsListPageViewModel();
            _vm.PropertyChanged += _vm_PropertyChanged;
            _vm.Load();
            DataContext = _vm;

            ListBox_News.LoadMore += ListBox_News_LoadMore;
            Button_FirstItem.Click += new RoutedEventHandler(Button_FirstItem_Click);
        }

        void Button_FirstItem_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.NewsList.Count > 0)
                NavigationService.Navigate(new Uri("/Views/NewsPage.xaml?id=" + _vm.NewsList[0].Model.Id, UriKind.Relative));
        }

        void ListBox_News_LoadMore(object sender, EventArgs e)
        {
            _vm.LoadMore();
        }

        void _vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ProgressBarVisibility")
            {
                if (_vm.ProgressBarVisibility == Visibility.Visible)
                {
                    ProgressIndicator progress = new ProgressIndicator
                    {
                        IsVisible = true,
                        IsIndeterminate = true,
                        Text = "Loading news list..."
                    };
                    SystemTray.SetProgressIndicator(this, progress);
                }
                else
                {
                    ProgressIndicator progress = new ProgressIndicator
                    {
                        IsVisible = false
                    };
                    SystemTray.SetProgressIndicator(this, progress);
                }
            }

            if (e.PropertyName == "IsError")
            {
                if (_vm.IsError)
                {
                    //ListBox_News.ButtonVisibility = System.Windows.Visibility.Collapsed;
                    MessageBox.Show("Oops something went wrong.\nPlease check your connection.", "Error", MessageBoxButton.OK);
                }
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            ListBox_News.SelectedIndex = -1;
            SystemTray.SetBackgroundColor(this, Color.FromArgb(255, 24, 22, 22));
            SystemTray.SetForegroundColor(this, Color.FromArgb(255, 255, 255, 254));
            base.OnNavigatedTo(e);
        }

        protected override Clarity.Phone.Controls.Animations.AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            return base.GetAnimation(animationType, toOrFrom);
        }

        protected override void AnimationsComplete(AnimationType animationType)
        {
            base.AnimationsComplete(animationType);
        }

        private void ListBox_News_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ListBox_News.SelectedIndex != -1)
                NavigationService.Navigate(new Uri("/Views/NewsPage.xaml?id=" + ((NewsViewModel)ListBox_News.SelectedItem).Model.Id, UriKind.Relative));
        }

        private void Image_ImageOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            AnimationHelper.FadeIn((Image)sender);
        }

        private void ApplicationBarIconButton_Click(object sender, System.EventArgs e)
        {
            _vm.Refresh();
        }
    }
}