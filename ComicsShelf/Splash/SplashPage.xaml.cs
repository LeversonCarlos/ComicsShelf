using ComicsShelf.ComicFiles;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Splash
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class SplashPage : ContentPage
   {

      public SplashPage()
      {
         InitializeComponent();
      }

      protected override void OnAppearing()
      {
         try
         {
            base.OnAppearing();

            Messaging.Subscribe<ComicPageSize>(ComicPageSize.PageSizeChanged, this.OnOrientationChanged);
            Messaging.Subscribe<ComicFileVM>("OnComicFileOpening", this.OnComicFileOpening);
            Messaging.Subscribe<ComicFileVM>("OnComicFileOpened", this.OnComicFileOpened);

            var currentFile = (this.BindingContext as SplashVM).CurrentFile;
            this.filesCollectionView.ScrollTo(currentFile);

         }
         catch (Exception ex)
         {
            Helpers.AppCenter.TrackEvent(ex);
            Device.BeginInvokeOnMainThread(async () => await App.Navigation().PopToRootAsync());
         }
      }

      private void OnOrientationChanged(ComicFiles.ComicPageSize pageSize)
      { (this.BindingContext as SplashVM).PageSize = pageSize; }

      private async void OnComicFileOpening(ComicFileVM value)
      {
         try
         {
            await Task.Run(async () =>
            {
               await this.backgroundImage.FadeTo(0.8, 250, Easing.SinOut);
               await Task.WhenAll(
                  this.backgroundImage.FadeTo(0.05, 50000, Easing.SinInOut),
                  this.backgroundImage.RelScaleTo(20, 50000, Easing.SinInOut)
               );
            });
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

      private async void OnComicFileOpened(ComicFileVM value)
      {
         try
         {
            ViewExtensions.CancelAnimations(this.backgroundImage);
            await Task.Run(async () =>
            {
               await Task.WhenAll(
                  this.backgroundImage.FadeTo(0.2, 250, Easing.SinIn),
                  this.backgroundImage.RelScaleTo(1, 250, Easing.SinIn)
               );
               this.backgroundImage.Opacity = 0.2;
               this.backgroundImage.Scale = 1;
            });
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

      protected override void OnDisappearing()
      {
         base.OnDisappearing();
         Messaging.Unsubscribe<ComicPageSize>(ComicPageSize.PageSizeChanged);
         Messaging.Unsubscribe<ComicFileVM>("OnComicFileOpening");
         Messaging.Unsubscribe<ComicFileVM>("OnComicFileOpened");
         ViewExtensions.CancelAnimations(this.backgroundImage);
         this.backgroundImage.Opacity = 0.2;
         this.backgroundImage.Scale = 1;
         this.filesCollectionView.BindingContext = null;
      }

   }
}