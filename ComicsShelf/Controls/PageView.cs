using System.IO;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class PageView : ContentView
   {

      public PageView()
      {

         this.ImageView = new Image
         {
            BackgroundColor = Color.Yellow,
            Aspect = Aspect.AspectFit,
            InputTransparent = false
         };
         this.ImageView.GestureRecognizers.Add(new TapGestureRecognizer
         {
            Command = new Command((param) =>
            { this.ImageZoom = (this.ImageZoom == 1.0 ? 2.0 : 1.0); this.OnImageResize(); }),
            NumberOfTapsRequired = 2
         });

         this.ScrollView = new ScrollView
         {
            Orientation = ScrollOrientation.Horizontal,
            Content = this.ImageView
         };

         // Xamarin.Essentials.DeviceDisplay.KeepScreenOn
         // Xamarin.Essentials
         // Messaging.Subscribe<Size>(Messaging.Keys.ScreenSizeChanged, this.OnScreenSizeChanged);
         this.OnImageResize();
      }

      #region Components

      public ScrollView ScrollView
      {
         get { return this.Content as ScrollView; }
         set { this.Content = value; }
      }
      private double ImageZoom { get; set; } = 1;
      public Image ImageView { get; set; }

      #endregion

      #region ImagePath
      public static readonly BindableProperty ImagePathProperty =
         BindableProperty.Create("ImagePath", typeof(string), typeof(PageView), string.Empty,
         propertyChanged: OnImagePathChanged, defaultBindingMode: BindingMode.TwoWay);
      public string ImagePath
      {
         get { return (string)GetValue(ImagePathProperty); }
         set { SetValue(ImagePathProperty, value); }
      }
      private static void OnImagePathChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as PageView).OnImageRefresh(); }
      #endregion

      #region ImageLoaded
      public static readonly BindableProperty ImageLoadedProperty =
         BindableProperty.Create("ImageLoaded", typeof(bool), typeof(PageView), false,
         propertyChanged: OnImageLoadedChanged, defaultBindingMode: BindingMode.TwoWay);
      public bool ImageLoaded
      {
         get { return (bool)GetValue(ImageLoadedProperty); }
         set { SetValue(ImageLoadedProperty, value); }
      }
      private static void OnImageLoadedChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as PageView).OnImageRefresh(); }
      #endregion

      #region ImageSize
      public static readonly BindableProperty ImageSizeProperty =
         BindableProperty.Create("ImageSize", typeof(ComicFiles.ComicPageSize), typeof(PageView), ComicFiles.ComicPageSize.Zero,
         propertyChanged: OnImageSizeChanged, defaultBindingMode: BindingMode.TwoWay);
      public ComicFiles.ComicPageSize ImageSize
      {
         get { return (ComicFiles.ComicPageSize)GetValue(ImageSizeProperty); }
         set { SetValue(ImageSizeProperty, value); }
      }
      private static void OnImageSizeChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as PageView).OnImageResize(); }
      #endregion

      #region OnImageRefresh
      private void OnImageRefresh()
      {
         try
         {
            this.ImageZoom = 1.0;
            if (!this.ImageLoaded && this.ImageView.Source != null)
            { this.ImageView.Source = null; }
            if (this.ImageLoaded && this.ImageView.Source == null && !string.IsNullOrEmpty(this.ImagePath))
            { this.ImageView.Source = ImageSource.FromStream(() => new MemoryStream(File.ReadAllBytes(this.ImagePath))); }
         }
         catch { }
      }
      #endregion

      #region OnImageResize
      private void OnImageResize()
      {
         try
         {
            if (this.ImageSize == null || this.ImageSize.IsZero()) { return; }

            var displayInfo = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
            var displayHeight = displayInfo.Height / displayInfo.Density;
            var displayWidth = displayInfo.Width / displayInfo.Density;
            this.HeightRequest = displayHeight;
            this.WidthRequest = displayWidth;

            // PORTRAIT SCREEN
            if (displayInfo.Orientation == Xamarin.Essentials.DisplayOrientation.Portrait)
            {
               if (this.ImageSize.Orientation == ComicFiles.ComicPageSize.OrientationEnum.Portrait)
               {
                  this.ScrollView.HeightRequest = displayHeight * this.ImageZoom;
                  this.ScrollView.WidthRequest = displayWidth * this.ImageZoom;
               }
               else if (this.ImageSize.Orientation == ComicFiles.ComicPageSize.OrientationEnum.Landscape)
               {
                  this.ScrollView.HeightRequest = displayHeight * this.ImageZoom;
                  this.ScrollView.WidthRequest = displayHeight * this.ImageZoom * (this.ImageSize.Width / this.ImageSize.Height);
               }
               this.ScrollView.Orientation = ScrollOrientation.Horizontal;
            }

            // LANDSCAPE SCREEN
            if (displayInfo.Orientation == Xamarin.Essentials.DisplayOrientation.Landscape)
            {
               if (this.ImageSize.Orientation == ComicFiles.ComicPageSize.OrientationEnum.Landscape)
               {
                  this.ScrollView.WidthRequest = displayWidth * this.ImageZoom;
                  this.ScrollView.HeightRequest = displayHeight * this.ImageZoom;
               }
               else if (this.ImageSize.Orientation == ComicFiles.ComicPageSize.OrientationEnum.Portrait)
               {
                  this.ScrollView.WidthRequest = displayWidth * this.ImageZoom;
                  this.ScrollView.HeightRequest = displayWidth * this.ImageZoom * (this.ImageSize.Height / this.ImageSize.Width);
               }
               this.ScrollView.Orientation = ScrollOrientation.Vertical;
            }

            // INNER IMAGE SIZE
            this.ImageView.WidthRequest = this.ScrollView.WidthRequest;
            this.ImageView.HeightRequest = this.ScrollView.HeightRequest;

            /*
            this.FadeTo(0.05, 100, Easing.SinOut)
               .ContinueWith(task1 =>
               {
                  this.LayoutTo(new Rectangle(0, 0, this.WidthRequest, this.HeightRequest), 0, null)
                     .ContinueWith(task2 =>
                     {
                        if (this.ImageZoom != 1.0)
                        {
                           this.ScrollView.Orientation = ScrollOrientation.Both;
                           Device.BeginInvokeOnMainThread(async () =>
                           { await this.ScrollView.ScrollToAsync((displayWidth / 2.0), (displayHeight / 2.0), false); });
                        }
                     })
                     .ContinueWith(task3 =>
                     {
                        this.FadeTo(1, 250, Easing.SinIn);
                     });
               });
               */

         }
         catch { }
      }
      #endregion

   }
}