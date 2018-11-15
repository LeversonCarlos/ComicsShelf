using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{
   internal class Library : BaseEngine
   {

      #region Execute
      public static async Task<bool> Execute()
      {
         try
         {
            var result = false;
            using (var engine = new Library())
            {
               result = await engine.DefineLibraryPath();
               if (result) {
                  App.HomeData.ClearAll();
                  await Engine.Search.Execute();
               }
            }
            return result;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }
      #endregion

      #region DefineLibraryPath
      private async Task<bool> DefineLibraryPath()
      {
         try
         {

            /* LIBRARIES */
            var libraries = App.Database.Table<Helpers.Database.Library>();

            /* LOAD DATA */
            var library = libraries.FirstOrDefault();
            if (library == null)
            {
               library = new Helpers.Database.Library { Available = false };
               App.Database.Insert(library);
            }

            /* DEFINE COMICS PATH */
            library.LibraryPath = await this.FileSystem.GetLibraryPath();
            await this.FileSystem.ValidateLibraryPath(library);

            /* STORE DATA */
            if (library.Available)
            {
               App.Database.Update(library);
               App.Settings.Paths.LibraryPath = library.LibraryPath;
            }

            return library.Available;
         }
         catch (Exception ex) { throw; }
      }
      #endregion


      #region NewLibrary
      internal static async Task<Helpers.Database.Library> NewLibrary()
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
               { await App.ShowMessage("INVALID LIBRARY PATH"); return null; }

               // RESULT
               App.Database.Insert(library);
               await RefreshLibrary();
               return library;

            }
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
      private static async Task RefreshLibrary()
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
         Task.Run(async () => Engine.Search.Execute());
      }
      #endregion

   }
}
