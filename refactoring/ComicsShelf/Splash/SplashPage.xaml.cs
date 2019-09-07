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

            if (!Xamarin.Essentials.OrientationSensor.IsMonitoring)
            { Xamarin.Essentials.OrientationSensor.Start(Xamarin.Essentials.SensorSpeed.Default); }

            /*
            TODO
            var currentFile = (this.BindingContext as SplashVM).CurrentFile;
            this.filesCollectionView.ScrollTo(currentFile);
            */

            Messaging.Subscribe<ComicFileVM>("OnComicFileOpening", this.OnComicFileOpening);
            Messaging.Subscribe<ComicFileVM>("OnComicFileOpened", this.OnComicFileOpened);

         }
         catch (Exception ex)
         {
            Helpers.AppCenter.TrackEvent(ex);
            Device.BeginInvokeOnMainThread(async () => await App.NavigationPopToRootAsync());
         }
      }

      private void OnComicFileOpening(ComicFileVM value)
      {
         try
         {
            Task.Run(async () =>
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

      private void OnComicFileOpened(ComicFileVM value)
      {
         try
         {
            ViewExtensions.CancelAnimations(this.backgroundImage);
            Task.Run(async () =>
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
         Xamarin.Essentials.OrientationSensor.Stop();
         Messaging.Unsubscribe("", "OnComicFileOpening");
         Messaging.Unsubscribe("", "OnComicFileOpened");
         base.OnDisappearing();
      }

   }
}