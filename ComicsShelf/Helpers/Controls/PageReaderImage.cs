using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace ComicsShelf.Helpers.Controls
{
   public class PageReaderImage : ScrollView
   {
      PanGestureRecognizer panGesture;

      #region New
      public PageReaderImage()
      {

         this.Image = new FFImageLoading.Forms.CachedImage
         {
            Aspect = Aspect.AspectFill,
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            VerticalOptions = LayoutOptions.CenterAndExpand,
            InputTransparent = false, 
            RetryCount = 10,
            RetryDelay = 250
         };
         this.Image.Error += this.Image_Error;
         this.Image.Success += this.Image_Success;

         this.Orientation = ScrollOrientation.Horizontal;        
         this.Content = this.Image;

         var tapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
         tapGesture.Tapped += this.OnDoubleTapped;
         // this.Image.GestureRecognizers.Add(tapGesture);

         panGesture = new  PanGestureRecognizer();
         panGesture.TouchPoints = 1;
         panGesture.PanUpdated += this.OnPanUpdated;
         // this.Image.GestureRecognizers.Add(panGesture);

      }
      #endregion


      #region ScreenSize
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
         /*
         if (this.ImageSize == Size.Zero) { return; }
         if (this.ScreenSize.Height >= this.ScreenSize.Width)
         { this.Orientation = ScrollOrientation.Horizontal; }
         else
         { this.Orientation = ScrollOrientation.Vertical; }
         this.Image.ReloadImage();
         this.Image.AnchorX = 0.5;
         this.Image.AnchorY = 0.5;
         this.ForceLayout();
         */
      }
      #endregion

      #region Image

      FFImageLoading.Forms.CachedImage Image { get; set; }
      Size ImageSize { get; set; }

      private void Image_Success(object sender, FFImageLoading.Forms.CachedImageEvents.SuccessEventArgs e)
      {
         this.ImageSize = new Size(e.ImageInformation.OriginalWidth, e.ImageInformation.OriginalHeight);
         System.GC.Collect();
      }

      private async void Image_Error(object sender, FFImageLoading.Forms.CachedImageEvents.ErrorEventArgs e)
      { await App.Message.Show(e.ToString()); }

      #endregion

      #region ImageSource
      public static readonly BindableProperty ImageSourceProperty =
         BindableProperty.Create("ImageSource", typeof(ImageSource), typeof(PageReaderImage), null,
         propertyChanged: OnImageSourceChanged, defaultBindingMode: BindingMode.TwoWay);
      public ImageSource ImageSource
      {
         get { return (ImageSource)GetValue(ImageSourceProperty); }
         set { SetValue(ImageSourceProperty, value); }
      }
      private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
      {
         try
         {
            var VIEW = bindable as PageReaderImage;
            var SOURCE = newValue as ImageSource;
            VIEW.Image.Source = SOURCE;
         }
         catch { }
      }
      #endregion

      #region IsSwipeEnabled
      public static readonly BindableProperty IsSwipeEnabledProperty =
         BindableProperty.Create("IsSwipeEnabled", typeof(bool), typeof(PageReaderImage), true,
         propertyChanged: OnIsSwipeEnabledChanged, defaultBindingMode: BindingMode.TwoWay);
      public bool IsSwipeEnabled
      {
         get { return (bool)GetValue(IsSwipeEnabledProperty); }
         set { SetValue(IsSwipeEnabledProperty, value); }
      }
      private static void OnIsSwipeEnabledChanged(BindableObject bindable, object oldValue, object newValue)
      {
         try
         {
            var VIEW = bindable as PageReaderImage;
            var IsSwipeEnabled = (bool)newValue;
         }
         catch { }
      }
      #endregion

      #region OnDoubleTapped
      private void OnDoubleTapped(object sender, EventArgs e)
      {
         if (this.Image.Scale == 1)
         {
            this.IsSwipeEnabled = false;
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
            this.IsSwipeEnabled = true;
            //this.Orientation = ScrollOrientation.Horizontal;
            this.InputTransparent = false;
            this.Image.HorizontalOptions = LayoutOptions.CenterAndExpand;
            this.Image.VerticalOptions = LayoutOptions.CenterAndExpand;
            this.Image.ScaleTo(1, 250, Easing.CubicInOut);
            //this.Image.TranslateTo(0.5, 0.5, 250, Easing.CubicInOut);
            this.Image.GestureRecognizers.Remove(panGesture);
         }
      }
      #endregion

      #region OnPanUpdated

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

      #endregion    

   }
}