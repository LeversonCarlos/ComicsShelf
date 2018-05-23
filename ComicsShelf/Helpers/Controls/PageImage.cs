using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace ComicsShelf.Helpers.Controls
{
   public class PageImage : ContentView
   {
      double initialScale = 0;
      double currentScale = 1;
      double startScale = 1;
      double xOffset = 0;
      double yOffset = 0;

      #region New
      public PageImage()
      {

         this.Image = new FFImageLoading.Forms.CachedImage
         {
            Aspect = Aspect.AspectFill,
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            VerticalOptions = LayoutOptions.CenterAndExpand,
            RetryCount = 10,
            RetryDelay = 250
         };
         this.Image.Error += this.Image_Error;
         this.Image.Success += this.Image_Success;

         this.HorizontalOptions = LayoutOptions.CenterAndExpand;
         this.VerticalOptions = LayoutOptions.CenterAndExpand;
         this.Content = this.Image;

         //var pinch = new PinchGestureRecognizer();
         //pinch.PinchUpdated += this.OnPinchUpdated;
         //this.GestureRecognizers.Add(pinch);

         //var pan = new PanGestureRecognizer();
         //pan.PanUpdated += this.OnPanUpdated;
         //this.GestureRecognizers.Add(pan);

         var tap = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
         tap.Tapped += this.OnDoubleTapped;
         this.GestureRecognizers.Add(tap);

      }
      #endregion

      #region ScreenSize
      public static readonly BindableProperty ScreenSizeProperty =
         BindableProperty.Create("ScreenSize", typeof(Size), typeof(PageImage), Size.Zero,
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
            var VIEW = bindable as PageImage;
            var SIZE = (Size)newValue;
            VIEW.ScreenSize = SIZE;
            VIEW.SetInitialZoom();
         }
         catch { }
      }
      #endregion

      #region Image

      FFImageLoading.Forms.CachedImage Image { get; set; }
      Size ImageSize { get; set; }

      private void Image_Success(object sender, FFImageLoading.Forms.CachedImageEvents.SuccessEventArgs e)
      {
         this.ImageSize = new Size(e.ImageInformation.OriginalWidth, e.ImageInformation.OriginalHeight);
         this.SetInitialZoom();
         System.GC.Collect();
      }

      private async void Image_Error(object sender, FFImageLoading.Forms.CachedImageEvents.ErrorEventArgs e)
      { await App.Message.Show(e.ToString()); }

      #endregion

      #region Source
      public static readonly BindableProperty SourceProperty =
         BindableProperty.Create("Source", typeof(ImageSource), typeof(PageImage), null,
         propertyChanged: OnSourceChanged, defaultBindingMode: BindingMode.TwoWay);
      public ImageSource Source
      {
         get { return (ImageSource)GetValue(SourceProperty); }
         set { SetValue(SourceProperty, value); }
      }
      private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
      {
         try
         {
            var VIEW = bindable as PageImage;
            var SOURCE = newValue as ImageSource;
            VIEW.Image.Source = SOURCE;
         }
         catch { }
      }
      #endregion

      #region OnDoubleTapped
      private async void OnDoubleTapped(object sender, EventArgs e)
      {
         if (Content.Scale > initialScale)
         {
            await this.Content.ScaleTo(initialScale, 250, Easing.CubicInOut);
            await this.Content.TranslateTo(0.5, 0.5, 250, Easing.CubicInOut);
            currentScale = initialScale;
            xOffset = Content.TranslationX;
            yOffset = Content.TranslationY;
         }
         else
         {
            this.Content.AnchorX = 0.5;
            this.Content.AnchorY = 0.5;
            await this.Content.ScaleTo(initialScale * 2, 250, Easing.CubicInOut);
         }
      }
      #endregion

      #region OnPanUpdated

      /*
      double startX;
      double startY;

      private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
      {
         switch (e.StatusType)
         {
            case GestureStatus.Started:
               startX = e.TotalX;
               startY = e.TotalY;
               Content.AnchorX = 0;
               Content.AnchorY = 0;
               break;

            case GestureStatus.Running:
               var maxTranslationX = Content.Scale * Content.Width - Content.Width;
               Content.TranslationX = Math.Min(0, Math.Max(-maxTranslationX, xOffset + e.TotalX - startX));

               var maxTranslationY = Content.Scale * Content.Height - Content.Height;
               Content.TranslationY = Math.Min(0, Math.Max(-maxTranslationY, yOffset + e.TotalY - startY));

               break;

            case GestureStatus.Completed:
               xOffset = Content.TranslationX;
               yOffset = Content.TranslationY;
               break;
         }
      }
      */

      #endregion

      #region OnPinchUpdated
      /*

      private async void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
      {
         try
         {
            if (e.Status == GestureStatus.Started)
            { this.OnPinchUpdated_Started(sender, e); }

            if (e.Status == GestureStatus.Running)
            { this.OnPinchUpdated_Running(sender, e); }

            if (e.Status == GestureStatus.Completed)
            { this.OnPinchUpdated_Completed(sender, e); }
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }

      void OnPinchUpdated_Started(object sender, PinchGestureUpdatedEventArgs e)
      {
         startScale = Content.Scale;
         Content.AnchorX = 0;
         Content.AnchorY = 0;
      }

      void OnPinchUpdated_Running(object sender, PinchGestureUpdatedEventArgs e)
      {
         currentScale += (e.Scale - 1) * startScale;
         // currentScale = Math.Max(1, currentScale);       
         currentScale = Math.Max(initialScale, currentScale);
         System.Diagnostics.Debug.WriteLine($"currentScale:{currentScale}");

         // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
         // so get the X pixel coordinate.
         double renderedX = Content.X + xOffset;
         double deltaX = renderedX / Width;
         double deltaWidth = Width / (Content.Width * startScale);
         double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

         // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
         // so get the Y pixel coordinate.
         double renderedY = Content.Y + yOffset;
         double deltaY = renderedY / Height;
         double deltaHeight = Height / (Content.Height * startScale);
         double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

         // Calculate the transformed element pixel coordinates.
         double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
         double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

         // Apply translation based on the change in origin.
         Content.TranslationX = targetX.Clamp(-Content.Width * (currentScale - 1), 0);
         Content.TranslationY = targetY.Clamp(-Content.Height * (currentScale - 1), 0);

         // Apply scale factor
         Content.Scale = currentScale;
      }

      void OnPinchUpdated_Completed(object sender, PinchGestureUpdatedEventArgs e)
      {
         xOffset = Content.TranslationX;
         yOffset = Content.TranslationY;
      }

      */
      #endregion

      #region SetInitialZoom
      void SetInitialZoom()
      {
         try
         {
            if (this.ScreenSize == Size.Zero || this.ImageSize == Size.Zero) { return; }
            initialScale = 1;
            /*
            if (this.ScreenSize.Height > this.ScreenSize.Width)
            {
               if (this.ImageSize.Height > this.Image.Width) {
                  zoomScale = this.ScreenSize.Height / this.ImageSize.Height;
               }
               else {
                  zoomScale = this.ScreenSize.Height / this.ImageSize.Height;
               }
            }
            else
            {
               if (this.ImageSize.Height > this.Image.Width) {
                  zoomScale = this.ScreenSize.Width / this.ImageSize.Width;
               }
               else {
                  zoomScale = this.ScreenSize.Width / this.ImageSize.Width;
               }
            }
            */

            // this.ScaleTo(initialScale, 250, Easing.CubicInOut);

            // var screenPoint = new Point(this.ScreenSize.Width / 2, this.ScreenSize.Height / 2);
            // var pinchArgs = new PinchGestureUpdatedEventArgs(GestureStatus.Started, initialScale, screenPoint);
            // this.OnPinchUpdated_Started(this, pinchArgs);
            // this.OnPinchUpdated_Running(this, pinchArgs);
            // this.OnPinchUpdated_Completed(this, pinchArgs);
         }
         catch { }
      }
      #endregion

      #region Clamp
      /*
      private T Clamp<T>(T value, T minimum, T maximum) where T : IComparable
      {
         if (value.CompareTo(minimum) < 0)
            return minimum;
         else if (value.CompareTo(maximum) > 0)
            return maximum;
         else
            return value;
      }
      */
      #endregion

   }
}