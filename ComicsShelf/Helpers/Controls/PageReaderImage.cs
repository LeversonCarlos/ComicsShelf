using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace ComicsShelf.Helpers.Controls
{
   public class PageReaderImage : ScrollView
   {   

      #region New
      public PageReaderImage()
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

         this.Orientation = ScrollOrientation.Horizontal;
         this.Content = this.Image;

         var pinchGesture = new PinchGestureRecognizer();
         pinchGesture.PinchUpdated += this.OnPinchUpdated;
         this.Content.GestureRecognizers.Add(pinchGesture);
        
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

      #region OnPinchUpdated

      double currentScale = 1;
      double startScale = 1;
      double xOffset = 0;
      double yOffset = 0;

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

      private void OnPinchUpdated_Started(object sender, PinchGestureUpdatedEventArgs e)
      {
         startScale = this.Content.Scale;
         this.Content.AnchorX = 0;
         this.Content.AnchorY = 0;
      }

      private void OnPinchUpdated_Running(object sender, PinchGestureUpdatedEventArgs e)
      {
         currentScale += (e.Scale - 1) * startScale;
         currentScale = Math.Max(1, currentScale);       
         System.Diagnostics.Debug.WriteLine($"currentScale:{currentScale}");

         // The ScaleOrigin is in relative coordinates to the wrapped user interface element, so get the X pixel coordinate.
         double renderedX = Content.X + xOffset;
         double deltaX = renderedX / Width;
         double deltaWidth = Width / (Content.Width * startScale);
         double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

         // The ScaleOrigin is in relative coordinates to the wrapped user interface element, so get the Y pixel coordinate.
         double renderedY = Content.Y + yOffset;
         double deltaY = renderedY / Height;
         double deltaHeight = Height / (Content.Height * startScale);
         double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

         // Calculate the transformed element pixel coordinates.
         double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
         double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

         // Apply translation based on the change in origin.
         this.Content.TranslationX = targetX.Clamp(-Content.Width * (currentScale - 1), 0);
         this.Content.TranslationY = targetY.Clamp(-Content.Height * (currentScale - 1), 0);

         // Apply scale factor
         this.Content.Scale = currentScale;
      }

      private void OnPinchUpdated_Completed(object sender, PinchGestureUpdatedEventArgs e)
      {
         xOffset = this.Content.TranslationX;
         yOffset = this.Content.TranslationY;

         if (Content.Scale == 1) {
            this.IsSwipeEnabled = true;
            this.Orientation = ScrollOrientation.Horizontal;
         }
         else {
            this.IsSwipeEnabled = false;
            this.Orientation = ScrollOrientation.Both;
         }
      }

      #endregion      

   }
}