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
            if (this.BindingContext != null) { return; }
            var libraryService = DependencyService.Get<Libraries.LibraryService>();
            if (!libraryService.ComicFiles.ContainsKey(this.LibraryID)) { await App.ShowMessage("LibraryID wasnt found on library collection"); return; }

            var comicFile = libraryService.ComicFiles[this.LibraryID]
               .Where(x => x.ComicFile.Available && x.ComicFile.Key == this.ComicKey)
               .FirstOrDefault();
            if (comicFile == null) { await App.ShowMessage("ComicKey wasnt found on library collection"); return; }

            this.BindingContext = new ReadingVM(comicFile);
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();
         Messaging.Subscribe<ComicPageSize>(ComicPageSize.PageSizeChanged, this.OnOrientationChanged);
      }

      private void OnOrientationChanged(ComicPageSize pageSize)
      { (this.BindingContext as ReadingVM).PageSize = pageSize; }

      protected override async void OnDisappearing()
      {
         try
         {
            base.OnDisappearing();
            Messaging.Unsubscribe<ComicPageSize>(ComicPageSize.PageSizeChanged);

            await (this.BindingContext as ReadingVM).ComicFile.UpdateLibrary();
            this.BindingContext = null;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
         finally { GC.Collect(); }
      }

   }
}