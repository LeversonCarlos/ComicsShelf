using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{
   internal class StartupEngine : BaseEngine, IDisposable
   {

      #region Execute
      public static async void Execute()
      {
         try
         {
            Console.WriteLine("StartupEngine: Start");
            using (var engine = new StartupEngine())
            {
               App.RootFolder = new Home.HomeData { Text = "Root" };
               await engine.LoadSettings();
               await Helpers.ViewModels.NavVM.PushAsync<Home.HomeVM>(true, App.RootFolder);

               var validateLibraryPath = await engine.ValidateLibraryPath();
               if (!validateLibraryPath)
               { await Helpers.ViewModels.NavVM.PushAsync<Tools.ToolsVM>(false); }
               else
               { Task.Run(() => SearchEngine.Execute()); }

            }
            Console.WriteLine("StartupEngine: Finish");
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

      #region LoadSettings
      private async Task LoadSettings()
      {
         try
         {
            this.Notify(R.Strings.STARTUP_ENGINE_LOADING_SETTINGS_DATABASE_MESSAGE);

            App.Settings = new Helpers.Settings.Settings();
            await App.Settings.InitializePath();

            App.Database = new Database.Connector();
            await App.Database.InitializeConnector(App.Settings.Paths.DatabasePath);
         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region ValidateLibraryPath
      private async Task<bool> ValidateLibraryPath()
      {
         try
         {
            this.Notify(R.Strings.STARTUP_ENGINE_VALIDATING_LIBRARY_PATH_MESSAGE);

            /* LOAD CONFIGS DATA */
            Database.Configs configs = null;
            await Task.Run(()=>
            {
               var configsTable = App.Database.Table<Database.Configs>();
               configs = configsTable.FirstOrDefault();
               if (configs == null)
               {
                  configs = new Database.Configs();
                  App.Database.Insert(configs);
               }
            });

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