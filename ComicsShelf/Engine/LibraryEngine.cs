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
            System.Diagnostics.Debug.WriteLine("Library Engine Start");
            using (var engine = new Library())
            {
               result = await engine.DefineLibraryPath();
               if (result) {
                  App.HomeData.ClearAll();
                  await Engine.Search.Execute();
               }
            }
            System.Diagnostics.Debug.WriteLine("Library Engine Finish");
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

            /* LOAD CONFIGS DATA */
            Helpers.Database.Configs configs = null;
            await Task.Run(() =>
            {
               var configsTable = App.Database.Table<Helpers.Database.Configs>();
               configs = configsTable.FirstOrDefault();
               if (configs == null)
               {
                  configs = new Helpers.Database.Configs();
                  App.Database.Insert(configs);
               }
            });

            /* DEFINE COMICS PATH */
            configs.LibraryPath = await this.FileSystem.GetLibraryPath();

            /* VALIDATE COMICS PATH */
            var validateLibraryPath = await this.FileSystem.ValidateLibraryPath(configs.LibraryPath);

            /* STORE DATA */
            if (validateLibraryPath)
            {
               await Task.Run(() =>
               {
                  App.Database.Update(configs);
                  App.Settings.Paths.LibraryPath = configs.LibraryPath;
               });
            }

            return validateLibraryPath;
         }
         catch (Exception ex) { throw; }
      }
      #endregion

   }
}
