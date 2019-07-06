using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class StatsView : StackLayout
   {
      internal const string Messaging_ShowStatsView = "ShowStatsView";

      #region New
      public StatsView()
      {

         this.HorizontalOptions = LayoutOptions.FillAndExpand;
         this.VerticalOptions = LayoutOptions.End;
         this.Margin = 0;
         this.Padding = 0;
         this.Spacing = 0;
         this.Opacity = 1;

         this.ProgressBar = new ProgressBar { HeightRequest = 2, BackgroundColor = Color.Accent };
         this.Children.Add(this.ProgressBar);

         this.FullTitleLabel = new Label
         {
            HorizontalOptions = LayoutOptions.StartAndExpand,
            HorizontalTextAlignment = TextAlignment.Start,
            FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.White
         };
         this.PageLabel = new Label
         {
            HorizontalOptions = LayoutOptions.EndAndExpand,
            HorizontalTextAlignment = TextAlignment.End,
            FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.White
         };

         this.Children.Add(new ContentView
         {
            HorizontalOptions = LayoutOptions.Fill,
            Padding = 1,
            BackgroundColor = Color.Accent,
            Content = new StackLayout
            {
               HorizontalOptions = LayoutOptions.FillAndExpand,
               Margin = new Thickness(10, 0),
               Padding = 0,
               Spacing = 0,
               Orientation = StackOrientation.Horizontal,
               Children = { this.FullTitleLabel, this.PageLabel }
            }
         });

         Messaging.Subscribe(Messaging_ShowStatsView, async (param) => await this.ShowStats());
      }
      #endregion

      #region Components 
      private ProgressBar ProgressBar { get; set; }
      private Label FullTitleLabel { get; set; }
      private Label PageLabel { get; set; }
      #endregion


      #region FullTitle
      public static readonly BindableProperty FullTitleProperty =
         BindableProperty.Create("FullTitle", typeof(string), typeof(StatsView), "", propertyChanged: OnFullTitleChanged);
      public string FullTitle
      {
         get { return (string)GetValue(FullTitleProperty); }
         set { SetValue(FullTitleProperty, value); }
      }
      private static void OnFullTitleChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as StatsView).FullTitleLabel.Text = (string)newValue; }
      #endregion

      #region ReadingPage
      public static readonly BindableProperty ReadingPageProperty =
         BindableProperty.Create("ReadingPage", typeof(short), typeof(StatsView), (short)0,
         propertyChanged: OnReadingPageChanged);
      public short ReadingPage
      {
         get { return (short)GetValue(ReadingPageProperty); }
         set { SetValue(ReadingPageProperty, value); }
      }
      private static void OnReadingPageChanged(BindableObject bindable, object oldValue, object newValue)
      {
         var self = (bindable as StatsView);
         var pageStatsText = R.Strings.FILE_PAGESTATS_TEXT;
         pageStatsText = string.Format(pageStatsText, self.ReadingPage, self.TotalPages);
         self.PageLabel.Text = pageStatsText;
         Task.Run(() => self.ShowStats());
      }
      #endregion

      #region TotalPages
      public static readonly BindableProperty TotalPagesProperty =
         BindableProperty.Create("TotalPages", typeof(short), typeof(StatsView), (short)0);
      public short TotalPages
      {
         get { return (short)GetValue(TotalPagesProperty); }
         set { SetValue(TotalPagesProperty, value); }
      }
      #endregion

      #region ReadingPercent
      public static readonly BindableProperty ReadingPercentProperty =
         BindableProperty.Create("ReadingPercent", typeof(double), typeof(StatsView), (double)0,
         propertyChanged: OnReadingPercentChanged);
      public double ReadingPercent
      {
         get { return (double)GetValue(ReadingPercentProperty); }
         set { SetValue(ReadingPercentProperty, value); }
      }
      private static void OnReadingPercentChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as StatsView).ProgressBar.Progress = (double)newValue; }
      #endregion


      #region ShowStats
      private async Task ShowStats()
      {
         try
         {
            if (this.ReadingPage == 0) { return; }
            await this.FadeTo(0.90, 250, Easing.SinOut)
               .ContinueWith((task1) =>
               {
                  Task.Delay(1000)
                     .ContinueWith(async (task) =>
                     {
                        await this.FadeTo(0.0, 1000, Easing.SinIn);
                     });
               });
         }
         catch { }
      }
      #endregion

   }
}