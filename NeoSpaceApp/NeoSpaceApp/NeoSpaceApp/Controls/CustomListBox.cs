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

namespace NeoSpaceApp.Controls
{
    public class CustomListBox : ListBox
    {
        public HyperlinkButton Button_LoadMore;
        public EventHandler LoadMore;

        public CustomListBox()
        {
			//SetButtonVisibility(this,Visibility.Collapsed);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button_LoadMore = GetTemplateChild("Button_LoadMore") as HyperlinkButton;
            if (Button_LoadMore != null)
                Button_LoadMore.Click += new RoutedEventHandler(Button_LoadMore_Click);
        }

        void Button_LoadMore_Click(object sender, RoutedEventArgs e)
        {
            if(LoadMore != null)
                LoadMore(this, null);
        }
	
		public static readonly DependencyProperty ButtonVisibilityProperty =
		DependencyProperty.RegisterAttached("ButtonVisibility", typeof(Visibility), typeof(CustomListBox),
			new PropertyMetadata(null));
		
		public Visibility ButtonVisibility
        {
            get { return (Visibility)GetValue(ButtonVisibilityProperty); }
            set { SetValue(ButtonVisibilityProperty, value); }
        }
    }

    
}
