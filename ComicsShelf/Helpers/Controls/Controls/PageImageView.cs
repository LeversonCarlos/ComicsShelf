using System.IO;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class PageImageView : ScrollView
   {

      #region New
      public PageImageView()
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

      }
      #endregion


      #region ScreenSize
      public static readonly BindableProperty ScreenSizeProperty =
         BindableProperty.Create("ScreenSize", typeof(Size), typeof(PageImageView), Size.Zero,
         propertyChanged: OnScreenSizeChanged, defaultBindingMode: BindingMode.TwoWay);
      public Size ScreenSize
      {
         get { return (Size)GetValue(ScreenSizeProperty); }
         set { SetValue(ScreenSizeProperty, value); }
      }
      private static void OnScreenSizeChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as PageImageView).OnImageResize(); }
      #endregion

      #region ImageZoom
      double _ImageZoom = 1;
      public double ImageZoom
      {
         get { return this._ImageZoom; }
         set { this._ImageZoom = value; }
      }
      #endregion

      #region ImageSource
      public ImageSource ImageSource
      {
         get { return (this.Content as Image).Source; }
         set { (this.Content as Image).Source = value; }
      }
      #endregion

      #region ImagePath
      public static readonly BindableProperty ImagePathProperty =
         BindableProperty.Create("ImagePath", typeof(string), typeof(PageImageView), string.Empty,
         propertyChanged: OnImagePathChanged, defaultBindingMode: BindingMode.TwoWay);
      public string ImagePath
      {
         get { return (string)GetValue(ImagePathProperty); }
         set { SetValue(ImagePathProperty, value); }
      }
      private static void OnImagePathChanged(BindableObject bindable, object oldValue, object newValue)
      { /*(bindable as ImageView).Redraw();*/ }
      #endregion

      #region ImageLoaded
      public static readonly BindableProperty ImageLoadedProperty =
         BindableProperty.Create("ImageLoaded", typeof(bool), typeof(PageImageView), false,
         propertyChanged: OnImageLoadedChanged, defaultBindingMode: BindingMode.TwoWay);
      public bool ImageLoaded
      {
         get { return (bool)GetValue(ImageLoadedProperty); }
         set { SetValue(ImageLoadedProperty, value); }
      }
      private static void OnImageLoadedChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as PageImageView).OnImageRefresh(); }
      #endregion

      #region ImageSize
      public static readonly BindableProperty ImageSizeProperty =
         BindableProperty.Create("ImageSize", typeof(PageSize), typeof(PageImageView), PageSize.Zero,
         propertyChanged: OnImageSizeChanged, defaultBindingMode: BindingMode.TwoWay);
      public PageSize ImageSize
      {
         get { return (PageSize)GetValue(ImageSizeProperty); }
         set { SetValue(ImageSizeProperty, value); }
      }
      private static void OnImageSizeChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as PageImageView).OnImageResize(); }
      #endregion


      #region OnImageRefresh
      private void OnImageRefresh()
      {
         try
         {
            this.ImageZoom = 1.0;
            if (!this.ImageLoaded && this.ImageSource != null)
            { this.ImageSource = null; }
            if (this.ImageLoaded && this.ImageSource == null)
            { this.ImageSource = ImageSource.FromStream(() => new MemoryStream(File.ReadAllBytes(this.ImagePath))); }
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
            if (this.ScreenSize.Height >= this.ScreenSize.Width)
            {
               if (this.ImageSize.Orientation == PageSize.OrientationEnum.Portrait)
               {
                  this.HeightRequest = this.ScreenSize.Height * this.ImageZoom;
                  this.WidthRequest = this.ScreenSize.Width * this.ImageZoom;
               }
               else if (this.ImageSize.Orientation == PageSize.OrientationEnum.Landscape)
               {
                  this.HeightRequest = this.ScreenSize.Height * this.ImageZoom;
                  this.WidthRequest = this.ScreenSize.Height * this.ImageZoom * (this.ImageSize.Width / this.ImageSize.Height);
               }
               this.Orientation = ScrollOrientation.Horizontal;
            }
            else if (this.ScreenSize.Height < this.ScreenSize.Width)
            {
               if (this.ImageSize.Orientation == PageSize.OrientationEnum.Landscape)
               {
                  this.WidthRequest = this.ScreenSize.Width * this.ImageZoom;
                  this.HeightRequest = this.ScreenSize.Height * this.ImageZoom;
               }
               else if (this.ImageSize.Orientation == PageSize.OrientationEnum.Portrait)
               {
                  this.WidthRequest = this.ScreenSize.Width * this.ImageZoom;
                  this.HeightRequest = this.ScreenSize.Width * this.ImageZoom * (this.ImageSize.Height / this.ImageSize.Width);
               }
               this.Orientation = ScrollOrientation.Vertical;
            }

            var image = (this.Content as Image);
            image.WidthRequest = this.WidthRequest;
            image.HeightRequest = this.HeightRequest;

            this.LayoutTo(new Rectangle(0, 0, this.WidthRequest, this.HeightRequest), easing: Easing.SinOut).ContinueWith(task =>
            {
               if (this.ImageZoom != 1.0)
               {
                  this.Orientation = ScrollOrientation.Both;
                  Device.BeginInvokeOnMainThread(async () => { 
                     await this.ScrollToAsync((this.ScreenSize.Width / 2), (this.ScreenSize.Height / 2), true);
                  });
               }
            });


         }
         catch { }
      }
      #endregion

   }
}