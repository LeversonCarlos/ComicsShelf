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

      private void LoadBindingContext()
      {
         if (string.IsNullOrEmpty(this.LibraryID) || string.IsNullOrEmpty(this.ComicKey)) { return; }
         var libraryService = DependencyService.Get<Libraries.LibraryService>();
         this.BindingContext = new SplashVM(libraryService.ComicFiles[this.LibraryID]
            .Where(x => x.ComicFile.Available && x.ComicFile.Key == this.ComicKey)
            .FirstOrDefault());
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();
         this.backgroundImage.Opacity = 0.2;
         this.backgroundImage.Scale = 1;

         var currentFile = (this.BindingContext as SplashVM).CurrentFile;
         this.filesCollectionView.ScrollTo(currentFile);

         Messaging.Subscribe<ComicFileVM>("OnComicFileOpening", this.OnComicFileOpening);
         Messaging.Subscribe<ComicFileVM>("OnComicFileOpened", this.OnComicFileOpened);
      }

      private void OnComicFileOpening(ComicFileVM value)
      {
         Task.Run(async () =>
         {
            await this.backgroundImage.FadeTo(0.8, 100, Easing.SinOut);
            await Task.WhenAll(
               this.backgroundImage.FadeTo(0.2, 10000, Easing.SinOut),
               this.backgroundImage.RelScaleTo(20, 10000, Easing.SinOut)
            );
         });
      }

      private void OnComicFileOpened(ComicFileVM value)
      {
         ViewExtensions.CancelAnimations(this.backgroundImage);
         this.backgroundImage.Opacity = 0.2;
         this.backgroundImage.Scale = 1;
      }

      protected override void OnDisappearing()
      {
         Messaging.Unsubscribe("", "OnComicFileOpening");
         Messaging.Unsubscribe("", "OnComicFileOpened");
         base.OnDisappearing();
      }

   }
}