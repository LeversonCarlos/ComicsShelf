using System.IO;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class PageImage : ScrollView
   {
      // PanGestureRecognizer panGesture;

      #region New
      public PageImage()
      {
         this.Content = new Image
         {
            Aspect = Aspect.AspectFit,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand,
            InputTransparent = false
         };
         this.Orientation = ScrollOrientation.Horizontal;

         // var tapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
         // tapGesture.Tapped += this.OnDoubleTapped;
         // this.Image.GestureRecognizers.Add(tapGesture);

         // panGesture = new  PanGestureRecognizer();
         // panGesture.TouchPoints = 1;
         // panGesture.PanUpdated += this.OnPanUpdated;
         // this.Image.GestureRecognizers.Add(panGesture);

      }
      #endregion


      #region ScreenSize
      /*
       Size ImageSize { get; set; }

      public static readonly BindableProperty ScreenSizeProperty =
         BindableProperty.Create("ScreenSize", typeof(Size), typeof(PageReaderImage), Size.Zero,
         propertyChanged: OnScreenSizeChanged, defaultBindingMode: BindingMode.TwoWay);
      public Size ScreenSize
      {
         get { return (Size)GetValue(ScreenSizeProperty); }
         set { SetValue(ScreenSizeProperty, value); }
      }
      private static void OnScreenSizeChanged(BindableObject bindable, object oldValue, object newValue)
      {
         try
         {
            var VIEW = bindable as PageReaderImage;
            var SIZE = (Size)newValue;
            VIEW.ReviewScreenOrientation();
         }
         catch { }
      }
      private void ReviewScreenOrientation()
      {
         return;
         // / *
         if (this.ImageSize == Size.Zero) { return; }
         if (this.ScreenSize.Height >= this.ScreenSize.Width)
         { this.Orientation = ScrollOrientation.Horizontal; }
         else
         { this.Orientation = ScrollOrientation.Vertical; }
         this.Image.ReloadImage();
         this.Image.AnchorX = 0.5;
         this.Image.AnchorY = 0.5;
         this.ForceLayout();
         // * /
      }
      */
      #endregion


      #region ImagePath
      public static readonly BindableProperty ImagePathProperty =
         BindableProperty.Create("ImagePath", typeof(string), typeof(PageImage), string.Empty,
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
         BindableProperty.Create("ImageLoaded", typeof(bool), typeof(PageImage), false,
         propertyChanged: OnImageLoadedChanged, defaultBindingMode: BindingMode.TwoWay);
      public bool ImageLoaded
      {
         get { return (bool)GetValue(ImageLoadedProperty); }
         set { SetValue(ImageLoadedProperty, value); }
      }
      private static void OnImageLoadedChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as PageImage).ImageRefresh(); }
      #endregion

      #region ImageSource
      public ImageSource ImageSource {
         get { return (this.Content as Image).Source; }
         set { (this.Content as Image).Source = value; }
      }
      #endregion

      #region ImageRefresh
      private void ImageRefresh()
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