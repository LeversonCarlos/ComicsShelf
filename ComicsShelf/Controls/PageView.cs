using System.IO;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class PageView : ScrollView
   {

      public PageView()
      {
         this.Content = new Image
         {
            Aspect = Aspect.AspectFit,
            InputTransparent = false
         };
         this.Orientation = ScrollOrientation.Horizontal;

         ((Image)this.Content).GestureRecognizers.Add(new TapGestureRecognizer
         {
            Command = new Command((param) =>
            { this.ImageZoom = (this.ImageZoom == 1.0 ? 2.0 : 1.0); this.OnImageResize(); }),
            NumberOfTapsRequired = 2
         });

         // Xamarin.Essentials.DeviceDisplay.KeepScreenOn
         // Xamarin.Essentials

         // Messaging.Subscribe<Size>(Messaging.Keys.ScreenSizeChanged, this.OnScreenSizeChanged);
         this.OnScreenSizeChanged();
      }


      private double ImageZoom { get; set; } = 1;
      public Size ScreenSize { get; set; }
      private void OnScreenSizeChanged()
      {
         this.ScreenSize = new Size(Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width, Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Height);
         this.OnImageResize();
      }


      public ImageSource ImageSource
      {
         get { return (this.Content as Image).Source; }
         set { (this.Content as Image).Source = value; }
      }


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


      private void OnImageRefresh()
      {
         try
         {
            this.ImageZoom = 1.0;
            if (!this.ImageLoaded && this.ImageSource != null)
            { this.ImageSource = null; }
            if (this.ImageLoaded && this.ImageSource == null && !string.IsNullOrEmpty(this.ImagePath))
            { this.ImageSource = ImageSource.FromStream(() => new MemoryStream(File.ReadAllBytes(this.ImagePath))); }
         }
         catch { }
      }


      #region OnImageResize
      private void OnImageResize()
      {
         try
         {
            if (this.ImageSize == null || this.ImageSize.IsZero()) { return; }
            if (this.ScreenSize.Height >= this.ScreenSize.Width)
            {
               if (this.ImageSize.Orientation == ComicFiles.ComicPageSize.OrientationEnum.Portrait)
               {
                  this.HeightRequest = this.ScreenSize.Height * this.ImageZoom;
                  this.WidthRequest = this.ScreenSize.Width * this.ImageZoom;
               }
               else if (this.ImageSize.Orientation == ComicFiles.ComicPageSize.OrientationEnum.Landscape)
               {
                  this.HeightRequest = this.ScreenSize.Height * this.ImageZoom;
                  this.WidthRequest = this.ScreenSize.Height * this.ImageZoom * (this.ImageSize.Width / this.ImageSize.Height);
               }
               this.Orientation = ScrollOrientation.Horizontal;
            }
            else if (this.ScreenSize.Height < this.ScreenSize.Width)
            {
               if (this.ImageSize.Orientation == ComicFiles.ComicPageSize.OrientationEnum.Landscape)
               {
                  this.WidthRequest = this.ScreenSize.Width * this.ImageZoom;
                  this.HeightRequest = this.ScreenSize.Height * this.ImageZoom;
               }
               else if (this.ImageSize.Orientation == ComicFiles.ComicPageSize.OrientationEnum.Portrait)
               {
                  this.WidthRequest = this.ScreenSize.Width * this.ImageZoom;
                  this.HeightRequest = this.ScreenSize.Width * this.ImageZoom * (this.ImageSize.Height / this.ImageSize.Width);
               }
               this.Orientation = ScrollOrientation.Vertical;
            }

            var image = (this.Content as Image);
            image.WidthRequest = this.WidthRequest;
            image.HeightRequest = this.HeightRequest;

            this.FadeTo(0.05, 100, Easing.SinOut)
               .ContinueWith(task1 =>
               {
                  this.LayoutTo(new Rectangle(0, 0, this.WidthRequest, this.HeightRequest), 0, null)
                     .ContinueWith(task2 =>
                     {
                        if (this.ImageZoom != 1.0)
                        {
                           this.Orientation = ScrollOrientation.Both;
                           Device.BeginInvokeOnMainThread(async () =>
                           { await this.ScrollToAsync((this.ScreenSize.Width / 2.0), (this.ScreenSize.Height / 2.0), false); });
                        }
                     })
                     .ContinueWith(task3 =>
                     {
                        this.FadeTo(1, 250, Easing.SinIn);
                     });
               });

         }
         catch { }
      }
      #endregion

   }
}