using System.IO;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class PageImageView : ScrollView
   {
      // PanGestureRecognizer panGesture;

      #region New
      public PageImageView()
      {
         this.Content = new Image
         {
            Aspect = Aspect.AspectFit,
            InputTransparent = false
         };
         this.Orientation = ScrollOrientation.Horizontal;

         // panGesture = new  PanGestureRecognizer();
         // panGesture.TouchPoints = 1;
         // panGesture.PanUpdated += this.OnPanUpdated;
         // this.Image.GestureRecognizers.Add(panGesture);

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
                  this.HeightRequest = this.ScreenSize.Height;
                  this.WidthRequest = this.ScreenSize.Width;
               }
               else if (this.ImageSize.Orientation == PageSize.OrientationEnum.Landscape)
               {
                  this.HeightRequest = this.ScreenSize.Height;
                  this.WidthRequest = this.ScreenSize.Height * (this.ImageSize.Width / this.ImageSize.Height);
               }
               this.Orientation = ScrollOrientation.Horizontal;
            }
            else if (this.ScreenSize.Height < this.ScreenSize.Width)
            {
               if (this.ImageSize.Orientation == PageSize.OrientationEnum.Landscape)
               {
                  this.WidthRequest = this.ScreenSize.Width;
                  this.HeightRequest = this.ScreenSize.Height;
               }
               else if (this.ImageSize.Orientation == PageSize.OrientationEnum.Portrait)
               {
                  this.WidthRequest = this.ScreenSize.Width;
                  this.HeightRequest = this.ScreenSize.Width * (this.ImageSize.Height / this.ImageSize.Width);
               }
               this.Orientation = ScrollOrientation.Vertical;
            }

            var image = (this.Content as Image);
            image.WidthRequest = this.WidthRequest;
            image.HeightRequest = this.HeightRequest;
            this.LayoutTo(new Rectangle(0, 0, this.WidthRequest, this.HeightRequest));

         }
         catch { }
      }
      #endregion


      #region OnDoubleTapped
      /*
      private void OnDoubleTapped(object sender, EventArgs e)
      {
         if (this.Image.Scale == 1)
         {
            this.ImageLoaded = false;
            // this.Orientation = ScrollOrientation.Both;
            this.InputTransparent = true;
            this.Image.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.Image.VerticalOptions = LayoutOptions.FillAndExpand;
            this.Image.ScaleTo(3, 250, Easing.CubicInOut);
            //this.Image.TranslateTo(0.5, 0.5, 250, Easing.CubicInOut);
            this.Image.GestureRecognizers.Add(panGesture);
         }
         else
         {
            this.ImageLoaded = true;
            //this.Orientation = ScrollOrientation.Horizontal;
            this.InputTransparent = false;
            this.Image.HorizontalOptions = LayoutOptions.CenterAndExpand;
            this.Image.VerticalOptions = LayoutOptions.CenterAndExpand;
            this.Image.ScaleTo(1, 250, Easing.CubicInOut);
            //this.Image.TranslateTo(0.5, 0.5, 250, Easing.CubicInOut);
            this.Image.GestureRecognizers.Remove(panGesture);
         }
      }
      */
      #endregion

      #region OnPanUpdated
      /*

   double StartX; double StartY;
   double xOffset; double yOffset;

   private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
   {
      switch (e.StatusType)
      {
         case GestureStatus.Started:
            // xOffset = this.Image.TranslationX;
            // yOffset = this.Image.TranslationY;
            xOffset = (1 - this.Image.AnchorX) * this.ImageSize.Width;
            yOffset = (1 - this.Image.AnchorY) * this.ImageSize.Height;
            System.Diagnostics.Debug.WriteLine($"STARTED: translationX{this.Image.TranslationX}, translationY={this.Image.TranslationY}, anchorX{this.Image.AnchorX}, anchorY{this.Image.AnchorY}, totalX{e.TotalX}, totalY{e.TotalY}");
            break;
         case GestureStatus.Running:
            // this.Image.TranslationX = xOffset + e.TotalX;
            // this.Image.TranslationY = yOffset + e.TotalY;
            var anchorX = 1 - ((xOffset + e.TotalX) / this.ImageSize.Width);
            var anchorY = 1 - ((yOffset + e.TotalY) / this.ImageSize.Height);
            this.Image.AnchorX = anchorX;
            this.Image.AnchorY = anchorY;
            System.Diagnostics.Debug.WriteLine($"RUNNING: translationX{this.Image.TranslationX}, translationY={this.Image.TranslationY}, anchorX{this.Image.AnchorX}, anchorY{this.Image.AnchorY}, totalX{e.TotalX}, totalY{e.TotalY}");
            break;
      }
   }

   */
      #endregion

   }
}