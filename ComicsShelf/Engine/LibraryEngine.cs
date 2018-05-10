using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{
   internal class LibraryEngine : BaseEngine
   {

      #region Execute
      public static async Task<bool> Execute()
      {
         try
         {
            var result = false;
            Console.WriteLine("LibraryEngine: Start");
            using (var engine = new LibraryEngine())
            {
               result = await engine.DefineLibraryPath();
               if (result)
               { Xamarin.Forms.Device.BeginInvokeOnMainThread(() => SearchEngine.Execute()); }
            }
            Console.WriteLine("LibraryEngine: Finish");
            return result;
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); return false; }
      }
      #endregion

      #region DefineLibraryPath
      private async Task<bool> DefineLibraryPath()
      {
         try
         {

            /* LOAD CONFIGS DATA */
            Database.Configs configs = null;
            await Task.Run(() =>
            {
               var configsTable = App.Database.Table<Database.Configs>();
               configs = configsTable.FirstOrDefault();
               if (configs == null)
               {
                  configs = new Database.Configs();
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
