using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class PageStatsView : StackLayout
   {

      public ProgressBar readingProgress { get; set; }
      public Label readingText { get; set; }

      #region New
      public PageStatsView()
      {
         this.readingProgress = new ProgressBar { HorizontalOptions = LayoutOptions.Fill, HeightRequest = 5 };
         this.Children.Add(this.readingProgress);
         this.readingText = new Label { HorizontalOptions = LayoutOptions.CenterAndExpand, HorizontalTextAlignment = TextAlignment.Center };
         this.Children.Add(new ContentView
         {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.End,
            Padding = 2,
            BackgroundColor = Colors.Lighter,
            Content = this.readingText
         });
         this.Margin = 0;
         this.Padding = 0;
         this.Spacing = 0;

         Messaging.Subscribe(Messaging.Keys.PageTapped, async (param) => await this.ShowStats());
      }
      #endregion

      #region ReadingPage
      public static readonly BindableProperty ReadingPageProperty =
         BindableProperty.Create("ReadingPage", typeof(short), typeof(PageStatsView), (short)0,
         propertyChanged: OnReadingPageChanged, defaultBindingMode: BindingMode.TwoWay);
      public short ReadingPage
      {
         get { return (short)GetValue(ReadingPageProperty); }
         set { SetValue(ReadingPageProperty, value); }
      }
      private static void OnReadingPageChanged(BindableObject bindable, object oldValue, object newValue)
      {
         var self = (bindable as PageStatsView);
         self.readingText.Text = ((short)newValue).ToString();
         Task.Run(() => self.ShowStats());
      }
      #endregion

      #region ReadingPercent
      public static readonly BindableProperty ReadingPercentProperty =
         BindableProperty.Create("ReadingPercent", typeof(double), typeof(PageStatsView), (double)0,
         propertyChanged: OnReadingPercentChanged, defaultBindingMode: BindingMode.TwoWay);
      public double ReadingPercent
      {
         get { return (double)GetValue(ReadingPercentProperty); }
         set { SetValue(ReadingPercentProperty, value); }
      }
      private static void OnReadingPercentChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as PageStatsView).readingProgress.Progress = (double)newValue; }
      #endregion

      #region ShowStats
      private async Task ShowStats()
      {
         try
         {
            await this.FadeTo(1.0, 250, Easing.SinIn)
               .ContinueWith((task1) =>
               {
                  Task.Delay(1000)
                     .ContinueWith(async (task) =>
                     {
                        await this.FadeTo(0.0, 250, Easing.SinIn);
                     });
               });
         }
         catch { }
      }
      #endregion

   }
}