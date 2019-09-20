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

      protected override void OnAppearing()
      {
         base.OnAppearing();
         Messaging.Subscribe<ComicFiles.ComicPageSize>(ComicFiles.ComicPageSize.PageSizeChanged, this.OnOrientationChanged);
      }

      private void OnOrientationChanged(ComicFiles.ComicPageSize pageSize)
      { (this.BindingContext as ReadingVM).PageSize = pageSize; }

      protected override void OnDisappearing()
      {
         try
         {
            base.OnDisappearing();
            Messaging.Unsubscribe<ComicFiles.ComicPageSize>(ComicFiles.ComicPageSize.PageSizeChanged);
            if (this.BindingContext != null)
            {
               var libraryID = (this.BindingContext as ReadingVM).ComicFile.ComicFile.LibraryKey;
               Services.LibraryService.UpdateLibrary(libraryID);
            }
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
         finally { GC.Collect(); }
      }

   }
}