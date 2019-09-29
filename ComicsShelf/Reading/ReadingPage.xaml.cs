using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Reading
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ReadingPage : ContentPage
   {

      public ReadingPage()
      {
         InitializeComponent();
      }
      private ReadingVM ReadingVM() => (this.BindingContext as ReadingVM);

      protected override void OnAppearing()
      {
         base.OnAppearing();
         Helpers.AppCenter.TrackEvent("ReadingPage.OnAppearing");
         NavigationPage.SetHasNavigationBar(this, false);
         Messaging.Subscribe<ComicFiles.ComicPageSize>(ComicFiles.ComicPageSize.PageSizeChanged, this.OnOrientationChanged);
      }

      private void OnOrientationChanged(ComicFiles.ComicPageSize pageSize)
      { this.ReadingVM().PageSize = pageSize; }

      protected override async void OnDisappearing()
      {
         try
         {
            NavigationPage.SetHasNavigationBar(this, true);
            Messaging.Unsubscribe<ComicFiles.ComicPageSize>(ComicFiles.ComicPageSize.PageSizeChanged);

            if (this.BindingContext != null)
            {
               var comicFileVM = this.ReadingVM().ComicFile;

               var libraryID = comicFileVM.ComicFile.LibraryKey;
               await Services.LibraryService.UpdateLibrary(libraryID);

               //foreach (var page in comicFileVM.Pages) { page.IsVisible = false; }
               //comicFileVM.Pages.Clear();
            }

            Helpers.AppCenter.TrackEvent("ReadingPage.OnDisappearing");
            base.OnDisappearing();
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
         finally { GC.Collect(); }
      }

   }
}