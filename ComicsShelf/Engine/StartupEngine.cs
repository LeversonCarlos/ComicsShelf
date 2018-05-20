using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{
   internal class Startup : BaseEngine, IDisposable
   {

      #region Execute
      public static async void Execute()
      {
         try
         {
            System.Diagnostics.Debug.WriteLine("Startup Engine Start");
            using (var engine = new Startup())
            {
               App.RootFolder = new Home.HomeData { Text = R.Strings.AppTitle };
               await engine.LoadAppCenter();
               await engine.LoadSettings();
               await Helpers.ViewModels.NavVM.PushAsync<Home.HomeVM>(true, App.RootFolder);

               var validateLibraryPath = await engine.ValidateLibraryPath();
               if (!validateLibraryPath)
               { await Helpers.ViewModels.NavVM.PushAsync<ComicsShelf.Library.LibraryVM>(false); }
               else
               { Xamarin.Forms.Device.BeginInvokeOnMainThread(() => Engine.Search.Execute()); }

            }
            System.Diagnostics.Debug.WriteLine("Startup Engine Finish");
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

      #region LoadAppCenter
      private async Task LoadAppCenter()
      {
         try
         {
            Task.Run(() =>
            {
               Microsoft.AppCenter.AppCenter.Start(
                  "android=4ebe7891-1962-4e2a-96c4-c37a7c06c104;" +
                  "uwp=21539a63-8335-46ef-8771-c9c001371f87;" +
                  "ios={Your iOS App secret here}",
                  typeof(Microsoft.AppCenter.Analytics.Analytics),
                  typeof(Microsoft.AppCenter.Crashes.Crashes));
            });
         }
         catch (Exception ex) { throw; }
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