using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{
   internal class Library : BaseEngine
   {

      #region AddNew
      internal static async Task<Helpers.Database.Library> AddNew(Helpers.Database.LibraryTypeEnum libraryType)
      {
         try
         {

            // INITIALIZE
            var library = new Helpers.Database.Library { Type = libraryType };

            // FILESYSTEM LIBRARY
            if (libraryType == Helpers.Database.LibraryTypeEnum.FileSystem)
            {
               if (!await AddFileSystem(library))
               { await App.ShowMessage(R.Strings.LIBRARY_INVALID_FOLDER_MESSAGE); return null; }
            }

            // ONEDRIVE LIBRARY
            if (libraryType == Helpers.Database.LibraryTypeEnum.OneDrive)
            {
               if (!await AddFileSystem(library))
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

      #region AddFileSystem
      private static async Task<bool> AddFileSystem(Helpers.Database.Library library)
      {
         try
         {
            using (var fileSystem = Helpers.FileSystem.Get())
            {

               // DEFINE LIBRARY PATH
               library.LibraryPath = await fileSystem.GetLibraryPath();

               // VALIDATE 
               await fileSystem.ValidateLibraryPath(library);
               return library.Available;              

            }
         }
         catch (Exception) { throw; }
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
