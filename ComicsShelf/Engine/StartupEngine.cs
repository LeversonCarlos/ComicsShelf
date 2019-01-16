using SQLite;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{
   internal class Startup: BaseEngine
   {

      public static async Task Execute()
      {
         try
         {
            AppCenter.Initialize();
            var engine = new Startup();
            {
               engine.CheckPermissions(
                  async () =>
                  {
                     engine.Initialize();
                     await engine.LoadSettings();
                     await engine.LoadDatabase();
                     await engine.ShowHomeView();
                     await engine.InitializeLibrary();
                  });
            }
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      private void CheckPermissions(Action action)
      {
         this.Notify(R.Strings.STARTUP_ENGINE_CHECK_PERMISSIONS_MESSAGE);
         Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
         {
            this.FileSystem.CheckPermissions(action, () => { Environment.Exit(0); });
         });
      }

      private void Initialize()
      {
         try
         {
            App.HomeData = new Views.Home.HomeData();

            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            App.HomeData.EmptyCoverImage = Xamarin.Forms.ImageSource.FromResource("ComicsShelf.Helpers.Controls.Controls.CoverFrameView.Empty.png", executingAssembly);
            executingAssembly = null;
         }
         catch (Exception) { throw; }
      }

      private async Task LoadSettings()
      {
         try
         {
            this.Notify(R.Strings.STARTUP_ENGINE_LOADING_SETTINGS_MESSAGE);
            App.Settings = new vTwo.Settings.Engine();
         }
         catch (Exception) { throw; }
      }

      private async Task LoadDatabase()
      {
         try
         {
            this.Notify(R.Strings.STARTUP_ENGINE_LOADING_DATABASE_MESSAGE);
            App.Database = new Helpers.Database.dbContext();
            await App.Database.Initialize();
         }
         catch (Exception) { throw; }
      }

      private async Task ShowHomeView()
      {
         try
         {
            this.Notify(R.Strings.STARTUP_ENGINE_LOADING_DATABASE_MESSAGE);
            await Helpers.NavVM.PushAsync<Views.Home.HomeVM>(true, App.HomeData);
         }
         catch (Exception) { throw; }
      }

      private async Task InitializeLibrary()
      {
         try
         {
            this.Notify(R.Strings.STARTUP_ENGINE_VALIDATING_LIBRARY_PATH_MESSAGE);

            /* VALIDATE LIBRARIES */
            var librariesChanged = false;
            foreach (var library in App.Settings.Libraries)
            {
               var libraryService = Library.LibraryService.Get(library);
               var previousState = library.Available;
               await libraryService.Validate(library);
               if (library.Available != previousState)
               { librariesChanged=true; }
            }
            if (librariesChanged)
            { await App.Settings.SaveLibraries(); }

            // START SEARCH ENGINE
            Search.Refresh();

         }
         catch (Exception) { throw; }
      }

   }
}
