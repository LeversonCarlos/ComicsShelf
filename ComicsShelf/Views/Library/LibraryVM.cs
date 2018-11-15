using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Views.Library
{
   public class LibraryVM : Helpers.DataVM<LibraryData>
   {

      #region New
      public LibraryVM()
      {
         this.Title = R.Strings.LIBRARY_MAIN_TITLE; ;
         this.ViewType = typeof(LibraryView);
         this.Data = new LibraryData { LibraryPath = App.Settings.Paths.LibraryPath };
         this.AddLibraryTappedCommand = new Command(async (item) => await this.AddLibraryTapped(item));
         this.RemoveLibraryTappedCommand = new Command(async (item) => await this.RemoveLibraryTapped(item));
         this.LinkTappedCommand = new Command(async (item) => await this.LinkTapped(item));
      }
      #endregion

      #region AddLibraryTapped
      public Command AddLibraryTappedCommand { get; set; }
      private async Task AddLibraryTapped(object item)
      {
         try
         {
            var library = await Engine.Library.NewLibrary();
            if (library == null) { return; }

            this.Data.Libraries.Add(new LibraryDataItem(library));

            if (this.Data.Libraries.Count == 1)
            { await Helpers.NavVM.PushAsync<Views.Home.HomeVM>(true, App.HomeData); }
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

      #region RemoveLibraryTapped
      public Command RemoveLibraryTappedCommand { get; set; }
      private async Task RemoveLibraryTapped(object item)
      {
         try
         {
            var library = (item as LibraryDataItem);
            App.Database.Delete(library.Library);
            this.Data.Libraries.Remove(library);
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

      #region LinkTapped
      public Command LinkTappedCommand { get; set; }
      private async Task LinkTapped(object item)
      {
         try
         { Device.OpenUri(new Uri("http://www.comicbookplus.com")); }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

   }
}