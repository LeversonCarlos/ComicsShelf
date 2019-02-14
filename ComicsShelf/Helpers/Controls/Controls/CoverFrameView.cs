using System;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class CoverFrameView : Frame
   {

      #region New
      public CoverFrameView()
      {
         this.Margin = new Thickness(0, 0, 10, 10);
         this.Padding = 0;
         this.HasShadow = false;
         this.BackgroundColor = Color.White;
         this.BorderColor = Color.LightGray;
         this.CornerRadius = 0;

         this.Image = new Image
         {
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(5, 5, 5, 0),
            Aspect = Aspect.AspectFit
         };
         this.Image.Source = App.HomeData.EmptyCoverImage;

         this.Label = new Label
         {
            HorizontalOptions = LayoutOptions.Center,
            LineBreakMode = LineBreakMode.MiddleTruncation,
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(5,0), 
            FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label))
         };

         this.ProgressBar = new ProgressBar
         { HorizontalOptions = LayoutOptions.Fill, HeightRequest = 5 };

         this.Content = new StackLayout
         {
            Margin = 0,
            Padding = 0,
            Spacing = 2,
            Children = { this.Image, this.Label, this.ProgressBar }
         };

         Messaging.Subscribe<Size>(Messaging.Keys.ScreenSizeChanged, this.OnScreenSizeChanged);
         this.OnScreenSizeChanged(((NavPage)App.Current.MainPage).ScreenSize);
      }
      #endregion

      #region UseFrame
      public static readonly BindableProperty UseFrameProperty =
         BindableProperty.Create(nameof(UseFrame), typeof(bool), typeof(CoverFrameView), true,
         propertyChanged: OnUseFrameChanged);
      public bool UseFrame
      {
         get { return (bool)GetValue(UseFrameProperty); }
         set { SetValue(UseFrameProperty, value); }
      }
      private static void OnUseFrameChanged(BindableObject bindable, object oldValue, object newValue)
      {
         var view = (bindable as CoverFrameView);
         if ((bool)newValue)
         {
            view.Label.IsVisible = true;
            view.Image.Margin = new Thickness(5, 5, 5, 0);
            ((StackLayout)view.Content).Spacing = 2;
         }
         else
         {
            view.Label.IsVisible = false;
            view.Image.Margin = 0;
            ((StackLayout)view.Content).Spacing = 0;
         }
      }
      #endregion

      #region ImageSource
      Image Image { get; set; }
      public static readonly BindableProperty ImageSourceProperty =
         BindableProperty.Create("ImageSource", typeof(ImageSource), typeof(CoverFrameView), null,
         propertyChanged: OnImageSourceChanged);
      public ImageSource ImageSource
      {
         get { return (ImageSource)GetValue(ImageSourceProperty); }
         set { SetValue(ImageSourceProperty, value); }
      }
      private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as CoverFrameView).Image.Source = (ImageSource)newValue; }
      #endregion

      #region Text
      Label Label { get; set; }
      public static readonly BindableProperty TextProperty =
         BindableProperty.Create("Text", typeof(string), typeof(CoverFrameView), string.Empty,
         propertyChanged: OnTextChanged);
      public string Text
      {
         get { return (string)GetValue(TextProperty); }
         set { SetValue(TextProperty, value); }
      }
      private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as CoverFrameView).Label.Text = (string)newValue; }
      #endregion

      #region Progress
      ProgressBar ProgressBar { get; set; }
      public static readonly BindableProperty ProgressProperty =
         BindableProperty.Create("Progress", typeof(double), typeof(CoverFrameView), (double)0,
         propertyChanged: OnProgressChanged);
      public double Progress
      {
         get { return (double)GetValue(ProgressProperty); }
         set { SetValue(ProgressProperty, value); }
      }
      private static void OnProgressChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as CoverFrameView).ProgressBar.Progress = (double)newValue; }
      #endregion

      #region OnScreenSizeChanged
      private void OnScreenSizeChanged(Size screenSize)
      {
         try
         {
            var fileColumns = (int)Math.Ceiling(screenSize.Width / (double)160);
            var screenMargin = 0;
            if (Device.RuntimePlatform == Device.UWP) { screenMargin = 13 * 2; }
            var frameMargins = (double)((fileColumns + 1) * 10);
            this.WidthRequest = (screenSize.Width - screenMargin - frameMargins) / (double)fileColumns;
         }
         catch { }
      }
      #endregion

   }
}