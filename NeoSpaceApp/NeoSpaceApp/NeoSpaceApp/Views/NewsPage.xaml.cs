using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NeoSpaceApp.Extensions.Helpers;
using Clarity.Phone.Controls;
using NeoSpaceApp.ViewModels;
using System.Windows.Media;

namespace NeoSpaceApp.Views
{
    public partial class NewsPage : AnimatedBasePage
    {
        private NewsPageViewModel _vm;

        public NewsPage()
		{
			// Required to initialize variables
			InitializeComponent();
            FontSize = App.FontSize;
            Loaded += NewsControl_Loaded;
		}

        void NewsControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetForeground();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            SystemTray.SetBackgroundColor(this, Color.FromArgb(255, 24, 22, 22));
            SystemTray.SetForegroundColor(this, Color.FromArgb(255, 255, 255, 254));
            base.OnNavigatedTo(e);
            string id = "";
            if (NavigationContext.QueryString.TryGetValue("id", out id))
            {
                _vm = new NewsPageViewModel { News = App.MainViewModel.NewsPage.NewsList.Where(n => n.Model.Id == id).FirstOrDefault() };
                if(_vm == null)
                {
                    MessageBox.Show("Error","Can't find news",MessageBoxButton.OK);
                    NavigationService.GoBack();
                }
                _vm.PropertyChanged += _vm_PropertyChanged;
                _vm.Load(id);
            }
            DataContext = _vm;
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

        private void Image_ImageOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            AnimationHelper.FadeIn((Image)sender);
        }

        private void Button_Decrease_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (FontSize > 24)
                FontSize -= 2; App.FontSize = FontSize;
        }

        private void Button_Increase_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (FontSize < 45)
                FontSize += 2; App.FontSize = FontSize;
        }

        private void Button_LightDark_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.BackgroundColor = !App.BackgroundColor;
            SetForeground();
        }

        public void SetForeground()
        {
            if (App.BackgroundColor)
                VisualStateManager.GoToState(this, "Dark", false);
            else
                VisualStateManager.GoToState(this, "Light", false);
        }
    }
}