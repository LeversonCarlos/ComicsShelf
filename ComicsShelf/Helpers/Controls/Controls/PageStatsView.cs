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
         this.readingText = new Label
         {
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            HorizontalTextAlignment = TextAlignment.Center,
            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), 
            FontAttributes = FontAttributes.Bold
         };
         this.Children.Add(new ContentView
         {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.End,
            Padding = 10,
            BackgroundColor = Colors.Lighter,
            Content = this.readingText
         });
         this.Margin = 0;
         this.Padding = 0;
         this.Spacing = 0;
         this.Opacity = 0;

         Messaging.Subscribe(Messaging.Keys.PageTapped, async (param) => await this.ShowStats());
      }
      #endregion

      #region TotalPages
      public static readonly BindableProperty TotalPagesProperty =
         BindableProperty.Create("TotalPages", typeof(short), typeof(PageStatsView), (short)0);
      public short TotalPages
      {
         get { return (short)GetValue(TotalPagesProperty); }
         set { SetValue(TotalPagesProperty, value); }
      }
      #endregion

      #region ReadingPage
      public static readonly BindableProperty ReadingPageProperty =
         BindableProperty.Create("ReadingPage", typeof(short), typeof(PageStatsView), (short)0,
         propertyChanged: OnReadingPageChanged);
      public short ReadingPage
      {
         get { return (short)GetValue(ReadingPageProperty); }
         set { SetValue(ReadingPageProperty, value); }
      }
      private static void OnReadingPageChanged(BindableObject bindable, object oldValue, object newValue)
      {
         var self = (bindable as PageStatsView);
         var pageStatsText = R.Strings.FILE_PAGESTATS_TEXT;
         pageStatsText = string.Format(pageStatsText, self.ReadingPage, self.TotalPages);
         self.readingText.Text = pageStatsText;
         Task.Run(() => self.ShowStats());
      }
      #endregion

      #region ReadingPercent
      public static readonly BindableProperty ReadingPercentProperty =
         BindableProperty.Create("ReadingPercent", typeof(double), typeof(PageStatsView), (double)0,
         propertyChanged: OnReadingPercentChanged);
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
            if (this.ReadingPage == 0) { return; }
            await this.FadeTo(1.0, 250, Easing.SinOut)
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