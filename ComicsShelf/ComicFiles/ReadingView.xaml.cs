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

      private void LoadBindingContext()
      {
         if (string.IsNullOrEmpty(this.LibraryID) || string.IsNullOrEmpty(this.ComicKey)) { return; }
         var libraryService = DependencyService.Get<Libraries.LibraryService>();
         this.BindingContext = new ReadingVM(libraryService.ComicFiles[this.LibraryID]
            .Where(x => x.ComicFile.Available && x.ComicFile.Key == this.ComicKey)
            .FirstOrDefault());
      }

      protected override async void OnDisappearing()
      {
         await (this.BindingContext as ReadingVM).ComicFile.UpdateLibrary();
         base.OnDisappearing();
      }

   }
}