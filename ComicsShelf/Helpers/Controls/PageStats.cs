using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class PageStats: StackLayout
   {

      public ProgressBar readingProgress { get; set; }
      public Label readingText { get; set; }

      #region New
      public PageStats()
      {
         this.readingProgress = new ProgressBar { HorizontalOptions = LayoutOptions.Fill, HeightRequest = 5 };
         this.Children.Add(this.readingProgress);
         this.readingText = new Label { HorizontalOptions = LayoutOptions.CenterAndExpand, HorizontalTextAlignment = TextAlignment.Center };
         this.Children.Add(new ContentView
         {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.End,
            Padding = 2,
            BackgroundColor = (Color)Application.Current.Resources["colorLighter"],
            Content = this.readingText
         });
         this.Margin = 0;
         this.Padding = 0;
         this.Spacing = 0;
      }
      #endregion

      #region ReadingPage
      public static readonly BindableProperty ReadingPageProperty =
         BindableProperty.Create("ReadingPage", typeof(short), typeof(PageStats), (short)0,
         propertyChanged: OnReadingPageChanged, defaultBindingMode: BindingMode.TwoWay);
      public short ReadingPage
      {
         get { return (short)GetValue(ReadingPageProperty); }
         set { SetValue(ReadingPageProperty, value); }
      }
      private static void OnReadingPageChanged(BindableObject bindable, object oldValue, object newValue)
      {
         var self = (bindable as PageStats);
         self.readingText.Text = ((short)newValue).ToString();
         self.FadeTo(1.0, 250, Easing.SinIn)
            .ContinueWith((task1)=> {
               System.Threading.Tasks.Task.Delay(1000)
                  .ContinueWith((task) => {
                     self.FadeTo(0.0, 250, Easing.SinIn);
                  });
            });
      }
      #endregion

      #region ReadingPercent
      public static readonly BindableProperty ReadingPercentProperty =
         BindableProperty.Create("ReadingPercent", typeof(double), typeof(PageStats), (double)0,
         propertyChanged: OnReadingPercentChanged, defaultBindingMode: BindingMode.TwoWay);
      public double ReadingPercent
      {
         get { return (double)GetValue(ReadingPercentProperty); }
         set { SetValue(ReadingPercentProperty, value); }
      }
      private static void OnReadingPercentChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as PageStats).readingProgress.Progress = (double)newValue; }
      #endregion

   }
}