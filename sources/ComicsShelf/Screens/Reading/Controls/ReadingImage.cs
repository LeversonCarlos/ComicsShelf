using ComicsShelf.ViewModels;
using System;
using System.IO;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Reading
{
   public class ReadingImage : Image
   {

      public static readonly BindableProperty ImagePathProperty =
         BindableProperty.Create("ImagePath", typeof(string), typeof(ReadingImage), string.Empty,
         propertyChanged: OnImagePathChanged);
      public string ImagePath
      {
         get { return (string)GetValue(ImagePathProperty); }
         set { SetValue(ImagePathProperty, value); }
      }
      static void OnImagePathChanged(BindableObject bindable, object oldValue, object newValue) =>
         (bindable as ReadingImage).OnImageRefresh();

      public static readonly BindableProperty ImageLoadedProperty =
         BindableProperty.Create("ImageLoaded", typeof(bool), typeof(ReadingImage), false,
         propertyChanged: OnImageLoadedChanged);
      public bool ImageLoaded
      {
         get { return (bool)GetValue(ImageLoadedProperty); }
         set { SetValue(ImageLoadedProperty, value); }
      }
      static void OnImageLoadedChanged(BindableObject bindable, object oldValue, object newValue) =>
         (bindable as ReadingImage).OnImageRefresh();

      void OnImageRefresh()
      {
         if (!this.ImageLoaded && this.Source != null)
            this.Source = null;
         if (this.ImageLoaded && this.Source == null && !string.IsNullOrEmpty(this.ImagePath))
            this.Source = ImageSource.FromStream(() => new MemoryStream(File.ReadAllBytes(this.ImagePath)));
      }


      public static readonly BindableProperty ImageSizeProperty =
         BindableProperty.Create("ImageSize", typeof(PageSizeVM), typeof(ReadingImage), PageSizeVM.Zero,
         propertyChanged: OnImageSizeChanged);
      public PageSizeVM ImageSize
      {
         get { return (PageSizeVM)GetValue(ImageSizeProperty); }
         set { SetValue(ImageSizeProperty, value); }
      }
      private static void OnImageSizeChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as ReadingImage).OnImageResize(); }

      public static readonly BindableProperty ScreenSizeProperty =
         BindableProperty.Create("ScreenSize", typeof(PageSizeVM), typeof(ReadingImage), PageSizeVM.Zero,
         propertyChanged: OnScreenSizeChanged, defaultBindingMode: BindingMode.TwoWay);
      public PageSizeVM ScreenSize
      {
         get { return (PageSizeVM)GetValue(ScreenSizeProperty); }
         set { SetValue(ScreenSizeProperty, value); }
      }
      private static void OnScreenSizeChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as ReadingImage).OnImageResize(); }

      private void OnImageResize()
      {
         try
         {
            if (this.ImageSize == null || this.ImageSize.IsZero()) return;
            if (this.ScreenSize == null || this.ScreenSize.IsZero()) return;
            var imageZoom = 1;

            // PORTRAIT SCREEN
            if (this.ScreenSize.Orientation == PageSizeVM.OrientationEnum.Portrait)
            {
               if (this.ImageSize.Orientation == PageSizeVM.OrientationEnum.Portrait)
               {
                  this.HeightRequest = this.ScreenSize.Height * imageZoom;
                  this.WidthRequest = this.ScreenSize.Width * imageZoom;
               }
               else if (this.ImageSize.Orientation == PageSizeVM.OrientationEnum.Landscape)
               {
                  this.HeightRequest = this.ScreenSize.Height * imageZoom;
                  this.WidthRequest = this.ScreenSize.Height * imageZoom * (this.ImageSize.Width / this.ImageSize.Height);
               }
            }

            // LANDSCAPE SCREEN
            if (this.ScreenSize.Orientation == PageSizeVM.OrientationEnum.Landscape)
            {
               if (this.ImageSize.Orientation == PageSizeVM.OrientationEnum.Landscape)
               {
                  this.WidthRequest = this.ScreenSize.Width * imageZoom;
                  this.HeightRequest = this.ScreenSize.Height * imageZoom;
               }
               else if (this.ImageSize.Orientation == PageSizeVM.OrientationEnum.Portrait)
               {
                  this.WidthRequest = this.ScreenSize.Width * imageZoom;
                  this.HeightRequest = this.ScreenSize.Width * imageZoom * (this.ImageSize.Height / this.ImageSize.Width);
               }
            }

            this.HeightRequest = Math.Round(this.HeightRequest, 0);
            this.HeightRequest = Math.Round(this.HeightRequest, 0);

         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }


   }
}
