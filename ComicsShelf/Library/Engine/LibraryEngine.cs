using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Library
{
   internal class LibraryEngine 
   {

      #region NewLibrary
      internal static async Task<vTwo.Libraries.Library> NewLibrary(vTwo.Libraries.TypeEnum libraryType)
      {
         try
         {

            // INITIALIZE
            var library = new vTwo.Libraries.Library { Type = libraryType };

            // USE SERVICE IMPLEMENTATION
            var libraryService = LibraryService.Get(library);
            if (!await libraryService.AddLibrary(library))
            { await App.ShowMessage(R.Strings.LIBRARY_INVALID_FOLDER_MESSAGE); return null; }

            // RESULT
            App.Settings.Libraries.Add(library);
            await App.Settings.SaveLibraries();
            await RefreshLibrary();
            return library;

         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }
      #endregion

      #region RemoveLibrary
      internal static async Task RemoveLibrary(vTwo.Libraries.Library library)
      {
         try
         {

            // USE SERVICE IMPLEMENTATION
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

            // REFRESH 
            await RefreshLibrary();
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

      #region RefreshLibrary
      internal static async Task RefreshLibrary()
      {
         App.HomeData.ClearAll();
         Task.Factory.StartNew(async () => await Engine.Search.Execute(true), TaskCreationOptions.LongRunning);
      }
      #endregion

   }
}
