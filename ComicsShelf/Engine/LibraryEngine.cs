using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{
   internal class Library : BaseEngine
   {

      #region AddNew
      internal static async Task<Helpers.Database.Library> AddNew()
      {
         try
         {
            using (var fileSystem = Helpers.FileSystem.Get())
            {

               // DEFINE LIBRARY PATH
               var library = new Helpers.Database.Library();
               library.LibraryPath = await fileSystem.GetLibraryPath();

               // VALIDATE 
               await fileSystem.ValidateLibraryPath(library);
               if (!library.Available)
               { await App.ShowMessage(R.Strings.LIBRARY_INVALID_FOLDER_MESSAGE); return null; }

               // RESULT
               App.Database.Insert(library);
               await Refresh();
               return library;

            }
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
               .Select(x => x.LibraryPath)
               .ToArray();
            App.Settings.Paths.LibraryPath = App.Settings.Paths.LibrariesPath.FirstOrDefault();
         });
         App.HomeData.ClearAll();
         Task.Factory.StartNew(Engine.Search.Execute, TaskCreationOptions.LongRunning);
      }
      #endregion

   }
}
