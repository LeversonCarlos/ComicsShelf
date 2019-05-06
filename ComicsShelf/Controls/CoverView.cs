﻿using ComicsShelf.ComicFiles;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class CoverView : AbsoluteLayout
   {

      public CoverView()
      {
         this.Margin = 0;
         this.Padding = 0;
         this.VerticalOptions = LayoutOptions.FillAndExpand;

         this.Image = new Image
         {
            VerticalOptions = LayoutOptions.Fill,
            Aspect = Aspect.AspectFill
         };
         var imageContainer = new Grid { Children = { this.Image } };
         this.Children.Add(imageContainer);
         AbsoluteLayout.SetLayoutBounds(imageContainer, new Rectangle(0, 0, 1, 1));
         AbsoluteLayout.SetLayoutFlags(imageContainer, AbsoluteLayoutFlags.All);

         this.ProgressBar = new ProgressBar
         {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.EndAndExpand,
            BackgroundColor = Color.White,
            HeightRequest = 5
         };
         var overlayContainer = new StackLayout
         {
            VerticalOptions = LayoutOptions.EndAndExpand,
            Children = { this.ProgressBar },
            Opacity = 0.75,
            InputTransparent = true
         };
         this.Children.Add(overlayContainer);
         AbsoluteLayout.SetLayoutBounds(overlayContainer, new Rectangle(0, 0, 1, 1));
         AbsoluteLayout.SetLayoutFlags(overlayContainer, AbsoluteLayoutFlags.All);

         this.GestureRecognizers.Add(new TapGestureRecognizer
         {
            Command = this.OpenTransition,
            NumberOfTapsRequired = 1
         });

      }


      Image Image { get; set; }
      public static readonly BindableProperty ImageSourceProperty =
         BindableProperty.Create("ImageSource", typeof(ImageSource), typeof(CoverView), null,
         propertyChanged: OnImageSourceChanged);
      public ImageSource ImageSource
      {
         get { return (ImageSource)GetValue(ImageSourceProperty); }
         set { SetValue(ImageSourceProperty, value); }
      }
      private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as CoverView).Image.Source = (ImageSource)newValue; }


      ProgressBar ProgressBar { get; set; }
      public static readonly BindableProperty ProgressProperty =
         BindableProperty.Create("Progress", typeof(double), typeof(CoverView), (double)0,
         propertyChanged: OnProgressChanged);
      public double Progress
      {
         get { return (double)GetValue(ProgressProperty); }
         set { SetValue(ProgressProperty, value); }
      }
      private static void OnProgressChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as CoverView).ProgressBar.Progress = (double)newValue; }


      public static readonly BindableProperty HasCacheProperty =
         BindableProperty.Create("HasCache", typeof(HasCacheEnum), typeof(CoverView), HasCacheEnum.Unknown,
            propertyChanged: OnHasCacheChanged);
      public HasCacheEnum HasCache
      {
         get { return (HasCacheEnum)GetValue(HasCacheProperty); }
         set { SetValue(HasCacheProperty, value); }
      }
      private static void OnHasCacheChanged(BindableObject bindable, object oldValue, object newValue)
      {
         var opacity = ((HasCacheEnum)newValue == HasCacheEnum.Yes ? 1 : 0.7);
         (bindable as CoverView).FadeTo(opacity);
      }


      public static readonly BindableProperty OpenCommandProperty =
         BindableProperty.Create("OpenCommand", typeof(ICommand), typeof(CoverView), null);
      public ICommand OpenCommand
      {
         get { return (ICommand)GetValue(OpenCommandProperty); }
         set { SetValue(OpenCommandProperty, value); }
      }


      private ICommand OpenTransition
      {
         get
         {
            return new Command(async () =>
            {
               await this.ScaleTo(0.85, 150, Easing.SpringIn);
               await this.ScaleTo(1.00, 100, Easing.SpringOut);
               Device.BeginInvokeOnMainThread(() => this.OpenCommand?.Execute(this.BindingContext));
            });
         }
      }


   }
}