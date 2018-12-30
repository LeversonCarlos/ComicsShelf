using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{
   internal class Library : BaseEngine
   {

      #region AddNew
      internal static async Task<Helpers.Database.Library> AddNew(ComicsShelf.Library.LibraryTypeEnum libraryType)
      {
         try
         {

            // INITIALIZE
            var library = new Helpers.Database.Library { Type = libraryType };

            // USE SERVICE IMPLEMENTATION
            using (var libraryService = ComicsShelf.Library.Service.Get(library))
            {
               if (!await libraryService.AddLibrary(library))
               { await App.ShowMessage(R.Strings.LIBRARY_INVALID_FOLDER_MESSAGE); return null; }
            }

            // RESULT
            App.Database.Insert(library);
            await Refresh();
            return library;

         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }
      #endregion

      #region Remove
      internal static async Task Remove(Helpers.Database.Library library)
      {
         try
         {
            App.Database.Delete(library);
            await Refresh();
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

      #region Refresh
      internal static async Task Refresh()
      {
         await Task.Run(() =>
         {
            var libraries = App.Database.Table<Helpers.Database.Library>();
            App.Settings.Paths.LibrariesPath = libraries
               .Where(x => x.Available == true)
               .Where(x => x.LibraryPath != "")
               .Select(x => x.LibraryPath)
               .ToArray();
         });
         App.HomeData.ClearAll();
         Task.Factory.StartNew(Engine.Search.Execute, TaskCreationOptions.LongRunning);
      }
      #endregion

   }
}
