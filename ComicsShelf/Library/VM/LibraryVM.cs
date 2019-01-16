using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Library
{
   public class LibraryVM : Helpers.DataVM<LibraryData>
   {

      #region New
      public LibraryVM()
      {
         this.Title = R.Strings.LIBRARY_MAIN_TITLE; ;
         this.ViewType = typeof(LibraryView);
         this.Data = new LibraryData();
         this.AddLibraryTappedCommand = new Command(async (item) => await this.AddLibraryTapped(item));
         this.RemoveLibraryTappedCommand = new Command(async (item) => await this.RemoveLibraryTapped(item));
         this.AddOneDriveLibraryCommand = new Command(async (item) => await this.AddOneDriveLibrary(item));
         this.LinkTappedCommand = new Command(async (item) => await this.LinkTapped(item));
      }
      #endregion

      #region AddLibrary 

      public Command AddLibraryTappedCommand { get; set; }
      private async Task AddLibraryTapped(object item)
      {
         this.IsBusy = true;
         await this.AddLibrary(vTwo.Libraries.TypeEnum.FileSystem);
         this.IsBusy = false;
      }

      public Command AddOneDriveLibraryCommand { get; set; }
      private async Task AddOneDriveLibrary(object item)
      {
         this.IsBusy = true;
         await this.AddLibrary(vTwo.Libraries.TypeEnum.OneDrive);
         this.IsBusy = false;
      }

      private async Task AddLibrary(vTwo.Libraries.TypeEnum libraryType)
      {
         try
         {

            // INITIALIZE
            var library = new vTwo.Libraries.Library { Type = libraryType };

            // USE SERVICE IMPLEMENTATION
            var libraryService = LibraryService.Get(library);
            if (!await libraryService.AddLibrary(library))
            { await App.ShowMessage(R.Strings.LIBRARY_INVALID_FOLDER_MESSAGE); return; }

            // SAVE LIBRARY CONFIG
            App.Settings.Libraries.Add(library);
            await App.Settings.SaveLibraries();
            this.Data.Libraries.Add(new LibraryDataItem(library));

            // SCHEDULE LIBRARY REFRESH
            Engine.Search.Refresh();

            /*
            if (this.Data.Libraries.Count == 1)
            { await Helpers.NavVM.PushAsync<Views.Home.HomeVM>(true, App.HomeData); }
            */

         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("AddLibrary", ex); await App.ShowMessage(ex); }
      }

      #endregion

      #region RemoveLibrary

      public Command RemoveLibraryTappedCommand { get; set; }
      private async Task RemoveLibraryTapped(object item)
      {
         this.IsBusy = true;
         await this.RemoveLibrary(item as LibraryDataItem);
         this.IsBusy = false;
      }

      internal async Task RemoveLibrary(LibraryDataItem item)
      {
         try
         {

            // USE SERVICE IMPLEMENTATION
            var library = item.Library;
            var libraryService = LibraryService.Get(library);
            if (!await libraryService.RemoveLibrary(library)) { return; }

            // REMOVE FILES AND THE LIBRARY ITSELF
            var comicFiles = App.Database.Table<Helpers.Database.ComicFile>()
               .Where(x => x.LibraryPath == library.LibraryID)
               .ToList();
            await Task.Run(() => {
               foreach (var comicFile in comicFiles)
               { App.Database.Delete(comicFile); }
            });
            App.Settings.Libraries.Remove(library);
            await App.Settings.SaveLibraries();
            this.Data.Libraries.Remove(item);

            // SCHEDULE LIBRARY REFRESH
            Engine.Search.Refresh();
            // this.Data.RefreshLibraries();

         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("RemoveLibrary", ex); await App.ShowMessage(ex); }
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