using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.ComicFiles
{
   [QueryProperty("LibraryID", "libraryID")]
   [QueryProperty("ComicKey", "comicKey")]
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ReadingView : ContentPage
   {

      public ReadingView()
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

            var comicFile = libraryService.ComicFiles[this.LibraryID]
               .Where(x => x.ComicFile.Available && x.ComicFile.Key == this.ComicKey)
               .FirstOrDefault();
            if (comicFile == null) { await App.ShowMessage("ComicKey wasnt found on library collection"); return; }

            this.BindingContext = new ReadingVM(comicFile);
            if (!Xamarin.Essentials.OrientationSensor.IsMonitoring)
            { Xamarin.Essentials.OrientationSensor.Start(Xamarin.Essentials.SensorSpeed.Default); }
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

      protected override async void OnDisappearing()
      {
         try
         {
            Xamarin.Essentials.OrientationSensor.Stop();
            await (this.BindingContext as ReadingVM).ComicFile.UpdateLibrary();
            base.OnDisappearing();
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
         finally { GC.Collect(); }
      }

   }
}