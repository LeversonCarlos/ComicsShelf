using System;
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

   }
}
