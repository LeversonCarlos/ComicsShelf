using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Library
{
   internal class LibraryEngine 
   {

      #region NewLibrary
      internal static async Task<Helpers.Database.Library> NewLibrary(LibraryTypeEnum libraryType)
      {
         try
         {

            // INITIALIZE
            var library = new Helpers.Database.Library { Type = libraryType };

            // USE SERVICE IMPLEMENTATION
            using (var libraryService = LibraryService.Get(library))
            {
               if (!await libraryService.AddLibrary(library))
               { await App.ShowMessage(R.Strings.LIBRARY_INVALID_FOLDER_MESSAGE); return null; }
            }

            // RESULT
            App.Database.Insert(library);
            await RefreshLibrary();
            return library;

         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }
      #endregion

      #region RemoveLibrary
      internal static async Task RemoveLibrary(Helpers.Database.Library library)
      {
         try
         {
            App.Database.Delete(library);
            await RefreshLibrary();
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

      #region RefreshLibrary
      internal static async Task RefreshLibrary()
      {
         await Task.Run(() =>
         {
            var libraries = App.Database.Table<Helpers.Database.Library>();
            App.Settings.Paths.Libraries = libraries
               .Where(x => x.Available == true)
               .Where(x => x.LibraryPath != "")
               .ToArray();
         });
         App.HomeData.ClearAll();
         Task.Factory.StartNew(ComicsShelf.Engine.Search.Execute, TaskCreationOptions.LongRunning);
      }
      #endregion

   }
}
