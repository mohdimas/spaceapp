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

namespace NeoSpaceApp.Extensions.Helpers
{
    public class AnimationHelper
    {
        public static void FadeIn(FrameworkElement image, int index = -1, EventHandler callback = null)
        {
            image.Opacity = 0;

            var ease = new PowerEase();
            ease.EasingMode = EasingMode.EaseOut;
            ease.Power = 2.0f;

            Storyboard sb = new Storyboard();

            DoubleAnimation xa = new DoubleAnimation();
            Storyboard.SetTargetProperty(xa, new PropertyPath(Image.OpacityProperty));
            if (index != -1)
                xa.BeginTime = TimeSpan.FromSeconds((double)index / 20);
            else
            {
                int rand = new Random().Next(300);
                xa.BeginTime = TimeSpan.FromMilliseconds(rand);
            }
            xa.Duration = TimeSpan.FromSeconds(1);
            xa.From = 0;
            xa.To = 1;
            xa.EasingFunction = ease;

            sb.Children.Add(xa);

            Storyboard.SetTarget(xa, image);
            sb.Begin();
            sb.Completed += (sender, e) =>
            {
                if (callback != null)
                    callback(image, null);
            };
        }

        public static void FadeOut(FrameworkElement image, int index = -1, EventHandler callback = null)
        {
            var ease = new PowerEase();
            ease.EasingMode = EasingMode.EaseOut;
            ease.Power = 2.0f;

            Storyboard sb = new Storyboard();

            DoubleAnimation xa = new DoubleAnimation();
            Storyboard.SetTargetProperty(xa, new PropertyPath(Image.OpacityProperty));
            if (index != -1)
                xa.BeginTime = TimeSpan.FromSeconds((double)index / 20);
            else
            {
                int rand = new Random().Next(300);
                xa.BeginTime = TimeSpan.FromMilliseconds(rand);
            }
            xa.Duration = TimeSpan.FromSeconds(1);
            xa.From = 1;
            xa.To = 0;
            xa.EasingFunction = ease;

            sb.Children.Add(xa);

            Storyboard.SetTarget(xa, image);
            sb.Begin();
            sb.Completed += (sender, e) => 
            {
                if (callback != null)
                    callback(image, null);
            };
        }

        public static void FadeIn(ImageBrush image)
        {
            var ease = new PowerEase();
            ease.EasingMode = EasingMode.EaseOut;
            ease.Power = 2.0f;

            Storyboard sb = new Storyboard();

            DoubleAnimation xa = new DoubleAnimation();
            Storyboard.SetTargetProperty(xa, new PropertyPath(ImageBrush.OpacityProperty));
            int rand = new Random(100).Next();
            xa.Duration = TimeSpan.FromSeconds(1);
            xa.BeginTime = TimeSpan.FromMilliseconds(rand);
            xa.From = 0;
            xa.To = 0.2;
            xa.EasingFunction = ease;

            sb.Children.Add(xa);

            Storyboard.SetTarget(xa, image);
            sb.Begin();
        }
    }
}
