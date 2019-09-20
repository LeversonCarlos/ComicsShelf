using System;
using System.IO;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class PageView : ScrollView
   {

      public PageView()
      {
         this.Orientation = ScrollOrientation.Horizontal;

         this.ImageView = new Image
         {
            Aspect = Aspect.AspectFit,
            InputTransparent = false
         };
         this.ImageView.GestureRecognizers.Add(new TapGestureRecognizer
         {
            Command = new Command((param) =>
            { this.ImageZoom = (this.ImageZoom == 1.0 ? 2.0 : 1.0); this.OnImageResize(); }),
            NumberOfTapsRequired = 2
         });

         this.OnImageResize();
      }

      #region Components

      public Image ImageView
      {
         get { return this.Content as Image; }
         set { this.Content = value; }
      }
      private double ImageZoom { get; set; } = 1;

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

      #region PageSize
      public static readonly BindableProperty PageSizeProperty =
         BindableProperty.Create("PageSize", typeof(ComicFiles.ComicPageSize), typeof(PageView), ComicFiles.ComicPageSize.Zero,
         propertyChanged: OnPageSizeChanged, defaultBindingMode: BindingMode.TwoWay);
      public ComicFiles.ComicPageSize PageSize
      {
         get { return (ComicFiles.ComicPageSize)GetValue(PageSizeProperty); }
         set { SetValue(PageSizeProperty, value); }
      }
      private static void OnPageSizeChanged(BindableObject bindable, object oldValue, object newValue)
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
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }
      #endregion

      #region OnImageResize
      private void OnImageResize()
      {
         try
         {
            if (this.ImageSize == null || this.ImageSize.IsZero()) { return; }
            if (this.PageSize == null || this.PageSize.IsZero()) { return; }

            var displayInfo = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
            var displayHeight = displayInfo.Height / displayInfo.Density;
            var displayWidth = displayInfo.Width / displayInfo.Density;

            // PORTRAIT SCREEN
            if (this.PageSize.Orientation == ComicFiles.ComicPageSize.OrientationEnum.Portrait)
            {
               if (this.ImageSize.Orientation == ComicFiles.ComicPageSize.OrientationEnum.Portrait)
               {
                  this.HeightRequest = displayHeight * this.ImageZoom;
                  this.WidthRequest = displayWidth * this.ImageZoom;
               }
               else if (this.ImageSize.Orientation == ComicFiles.ComicPageSize.OrientationEnum.Landscape)
               {
                  this.HeightRequest = displayHeight * this.ImageZoom;
                  this.WidthRequest = displayHeight * this.ImageZoom * (this.ImageSize.Width / this.ImageSize.Height);
               }
               this.Orientation = ScrollOrientation.Horizontal;
            }

            // LANDSCAPE SCREEN
            if (this.PageSize.Orientation == ComicFiles.ComicPageSize.OrientationEnum.Landscape)
            {
               if (this.ImageSize.Orientation == ComicFiles.ComicPageSize.OrientationEnum.Landscape)
               {
                  this.WidthRequest = displayWidth * this.ImageZoom;
                  this.HeightRequest = displayHeight * this.ImageZoom;
               }
               else if (this.ImageSize.Orientation == ComicFiles.ComicPageSize.OrientationEnum.Portrait)
               {
                  this.WidthRequest = displayWidth * this.ImageZoom;
                  this.HeightRequest = displayWidth * this.ImageZoom * (this.ImageSize.Height / this.ImageSize.Width);
               }
               this.Orientation = ScrollOrientation.Vertical;
            }

            // INNER IMAGE SIZE
            this.ImageView.WidthRequest = this.WidthRequest;
            this.ImageView.HeightRequest = this.HeightRequest;

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
                           { await this.ScrollToAsync((displayWidth / 2.0), (displayHeight / 2.0), false); });
                        }
                     })
                     .ContinueWith(task3 =>
                     {
                        this.FadeTo(1, 250, Easing.SinIn);
                     });
               });

         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }
      #endregion

   }
}