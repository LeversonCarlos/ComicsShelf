using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.ComicFiles
{
   [QueryProperty("LibraryID", "libraryID")]
   [QueryProperty("ComicKey", "comicKey")]
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class SplashView : ContentPage
   {
      public SplashView()
      {
         InitializeComponent();
      }

      string _LibraryID;
      public string LibraryID
      {
         get { return this._LibraryID; }
         set { this._LibraryID = Uri.UnescapeDataString(value); this.LoadBindingContext(); }
      }

      string _ComicKey;
      public string ComicKey
      {
         get { return this._ComicKey; }
         set { this._ComicKey = Uri.UnescapeDataString(value); this.LoadBindingContext(); }
      }

      private async void LoadBindingContext()
      {
         try
         {
            if (string.IsNullOrEmpty(this.LibraryID) || string.IsNullOrEmpty(this.ComicKey)) { return; }
            var libraryService = DependencyService.Get<Libraries.LibraryService>();
            if (!libraryService.ComicFiles.ContainsKey(this.LibraryID)) { await App.ShowMessage("LibraryID wasnt found on library collection"); return; }

            var currentFile = libraryService.ComicFiles[this.LibraryID]
               .Where(x => x.ComicFile.Available && x.ComicFile.Key == this.ComicKey)
               .FirstOrDefault();
            if (currentFile == null) { await App.ShowMessage("ComicKey wasnt found on library collection"); return; }

            this.BindingContext = new SplashVM(currentFile);
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

      protected override void OnAppearing()
      {
         try
         {
            base.OnAppearing();
            if (!Xamarin.Essentials.OrientationSensor.IsMonitoring)
            { Xamarin.Essentials.OrientationSensor.Start(Xamarin.Essentials.SensorSpeed.Default); }

            var currentFile = (this.BindingContext as SplashVM).CurrentFile;
            this.filesCollectionView.ScrollTo(currentFile);

            Messaging.Subscribe<ComicFileVM>("OnComicFileOpening", this.OnComicFileOpening);
            Messaging.Subscribe<ComicFileVM>("OnComicFileOpened", this.OnComicFileOpened);
         }
         catch (Exception ex)
         {
            Helpers.AppCenter.TrackEvent(ex);
            Device.BeginInvokeOnMainThread(async () => await AppShell.Current.Navigation.PopAsync());
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