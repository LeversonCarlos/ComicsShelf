using ComicsShelf.ComicFiles;
using System.Windows.Input;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class CoverView : AbsoluteLayout
   {

      public CoverView()
      {
         this.Margin = 0;
         this.Padding = 0;
         this.WidthRequest = CoverView.DefaultWidth; ;
         this.HeightRequest = CoverView.DefaultHeight;
         this.VerticalOptions = LayoutOptions.FillAndExpand;
         this.Padding = new Thickness(0, 0, 5, 5);

         this.Image = new Image
         {
            VerticalOptions = LayoutOptions.Fill,
            Aspect = Aspect.AspectFill
         };
         var imageContainer = new Grid { Children = { this.Image } };
         this.Children.Add(imageContainer);
         AbsoluteLayout.SetLayoutBounds(imageContainer, new Rectangle(0, 0, 1, 1));
         AbsoluteLayout.SetLayoutFlags(imageContainer, AbsoluteLayoutFlags.All);

         this.ProgressBar = new ProgressBar
         {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.EndAndExpand,
            BackgroundColor = Color.White,
            HeightRequest = 5
         };
         var overlayContainer = new StackLayout
         {
            VerticalOptions = LayoutOptions.EndAndExpand,
            Children = { this.ProgressBar },
            Opacity = 0.75,
            InputTransparent = true
         };
         this.Children.Add(overlayContainer);
         AbsoluteLayout.SetLayoutBounds(overlayContainer, new Rectangle(0, 0, 1, 1));
         AbsoluteLayout.SetLayoutFlags(overlayContainer, AbsoluteLayoutFlags.All);

         this.GestureRecognizers.Add(new TapGestureRecognizer
         {
            Command = this.OpenTransition,
            NumberOfTapsRequired = 1
         });

      }


      Image Image { get; set; }
      public static readonly BindableProperty ImageSourceProperty =
         BindableProperty.Create("ImageSource", typeof(ImageSource), typeof(CoverView), null,
         propertyChanged: OnImageSourceChanged);
      public ImageSource ImageSource
      {
         get { return (ImageSource)GetValue(ImageSourceProperty); }
         set { SetValue(ImageSourceProperty, value); }
      }
      private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as CoverView).Image.Source = (ImageSource)newValue; }


      ProgressBar ProgressBar { get; set; }
      public static readonly BindableProperty ProgressProperty =
         BindableProperty.Create("Progress", typeof(double), typeof(CoverView), (double)0,
         propertyChanged: OnProgressChanged);
      public double Progress
      {
         get { return (double)GetValue(ProgressProperty); }
         set { SetValue(ProgressProperty, value); }
      }
      private static void OnProgressChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as CoverView).ProgressBar.Progress = (double)newValue; }


      public static readonly BindableProperty CacheStatusProperty =
         BindableProperty.Create("CacheStatus", typeof(CacheStatusEnum), typeof(CoverView), CacheStatusEnum.Unknown,
            propertyChanged: OnCacheStatusChanged);
      public CacheStatusEnum CacheStatus
      {
         get { return (CacheStatusEnum)GetValue(CacheStatusProperty); }
         set { SetValue(CacheStatusProperty, value); }
      }
      private static void OnCacheStatusChanged(BindableObject bindable, object oldValue, object newValue)
      {
         var opacity = ((CacheStatusEnum)newValue == CacheStatusEnum.Yes ? 1 : 0.7);
         (bindable as CoverView).FadeTo(opacity);
      }


      public static readonly BindableProperty OpenCommandProperty =
         BindableProperty.Create("OpenCommand", typeof(ICommand), typeof(CoverView), null);
      public ICommand OpenCommand
      {
         get { return (ICommand)GetValue(OpenCommandProperty); }
         set { SetValue(OpenCommandProperty, value); }
      }


      private ICommand OpenTransition
      {
         get
         {
            return new Command(async () =>
            {
               await this.ScaleTo(0.85, 150, Easing.SpringIn);
               await this.ScaleTo(1.00, 100, Easing.SpringOut);
               Device.BeginInvokeOnMainThread(() => this.OpenCommand?.Execute(this.BindingContext));
            });
         }
      }


      public static double DefaultHeight { get; set; }
      public static double DefaultWidth { get; set; }
      public static void InitDefaultSize()
      {
         try
         {
            var displayInfo = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
            var displayWidth = System.Math.Min(displayInfo.Width, displayInfo.Height) / displayInfo.Density;
            var itemsPerLine = (Device.Idiom == TargetIdiom.Phone ? 3 : 5);
            CoverView.DefaultWidth = ((int)(displayWidth - 10) / itemsPerLine) - 5;
            CoverView.DefaultHeight = ((int)CoverView.DefaultWidth * 1.53);
         }
         catch (System.Exception) { throw; }
      }

   }
}