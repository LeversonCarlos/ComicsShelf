using System;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class CoverFrameView : AbsoluteLayout
   {

      #region New
      public CoverFrameView()
      {
         this.Margin = new Thickness(0, 0, 10, 10);
         this.Padding = 0;
         this.BackgroundColor = Color.White;

         // IMAGE
         this.Image = new Image
         {                 
            VerticalOptions = LayoutOptions.Start,
            Aspect = Aspect.AspectFit
         };
         this.Image.Source = App.HomeData.EmptyCoverImage;
         var imageContainer = new Grid { Children = { this.Image } };
         this.Children.Add(imageContainer);
         AbsoluteLayout.SetLayoutBounds(imageContainer, new Rectangle(0, 0, 1, 1));
         AbsoluteLayout.SetLayoutFlags(imageContainer, AbsoluteLayoutFlags.All);

         this.Label = new Label
         {
            HorizontalOptions = LayoutOptions.Center,
            LineBreakMode = LineBreakMode.MiddleTruncation,
            FontAttributes = FontAttributes.Bold,
            FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label))
         };
         this.LabelContainer = new ContentView
         {
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Fill,
            Content = this.Label,
            Padding = new Thickness(2),
            BackgroundColor = Color.White,
            Opacity = 0.75
         };

         this.ProgressBar = new ProgressBar
         {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.EndAndExpand,
            HeightRequest = 5,
            BackgroundColor = Color.White,
            Opacity = 0.75
         };

         this.OverlayContainer = new StackLayout
         {
            VerticalOptions = LayoutOptions.FillAndExpand,
            Children = { this.LabelContainer, this.ProgressBar },
            InputTransparent = true
         };
         this.Children.Add(this.OverlayContainer);
         AbsoluteLayout.SetLayoutBounds(this.OverlayContainer, new Rectangle(0, 0, 1, 1));
         AbsoluteLayout.SetLayoutFlags(this.OverlayContainer, AbsoluteLayoutFlags.All);

         Messaging.Subscribe<Size>(Messaging.Keys.ScreenSizeChanged, this.OnScreenSizeChanged);
         this.OnScreenSizeChanged(((NavPage)App.Current.MainPage).ScreenSize);
      }
      #endregion

      #region ShowOverlay
      StackLayout OverlayContainer; 
      public static readonly BindableProperty ShowOverlayProperty =
         BindableProperty.Create(nameof(ShowOverlay), typeof(bool), typeof(CoverFrameView), true,
         propertyChanged: OnShowOverlayChanged);
      public bool ShowOverlay
      {
         get { return (bool)GetValue(ShowOverlayProperty); }
         set { SetValue(ShowOverlayProperty, value); }
      }
      private static void OnShowOverlayChanged(BindableObject bindable, object oldValue, object newValue)
      {
         var view = (bindable as CoverFrameView);
         view.LabelContainer.IsVisible = (bool)newValue;
         view.Label.IsVisible = (bool)newValue;
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
      ContentView LabelContainer { get; set; }
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
            this.Image.HeightRequest = this.WidthRequest * 1.53;
         }
         catch { }
      }
      #endregion

   }
}