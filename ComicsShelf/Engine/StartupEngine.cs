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
                     await engine.LoadInitialView();
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
         catch (Exception ex) { throw; }
      }

      private async Task LoadSettings()
      {
         try
         {
            this.Notify(R.Strings.STARTUP_ENGINE_LOADING_SETTINGS_MESSAGE);
            App.Settings = new Helpers.Settings.Settings();
            await App.Settings.Initialize();
         }
         catch (Exception ex) { throw; }
      }

      private async Task LoadDatabase()
      {
         try
         {
            this.Notify(R.Strings.STARTUP_ENGINE_LOADING_DATABASE_MESSAGE);
            App.Database = new Helpers.Database.dbContext();
            await App.Database.Initialize();
         }
         catch (Exception ex) { throw; }
      }

      private async Task LoadInitialView()
      {
         try
         {
            this.Notify(R.Strings.STARTUP_ENGINE_LOADING_DATABASE_MESSAGE);

            // SHOW HOME VIEW
            await Helpers.NavVM.PushAsync<Views.Home.HomeVM>(true, App.HomeData);

            // SHOW LIBRARY SELECTOR IF HASNT A LIBRARY DEFINED YET
            // if (!await this.ValidateLibraryPath())
            // { await Helpers.NavVM.PushAsync<Views.Library.LibraryVM>(true); return; }

            // START SEARCH ENGINE
            await Library.LibraryEngine.RefreshLibrary();

         }
         catch (Exception ex) { throw; }
      }

      private async Task<bool> ValidateLibraryPath()
      {
         try
         {
            this.Notify(R.Strings.STARTUP_ENGINE_VALIDATING_LIBRARY_PATH_MESSAGE);

            /* LIBRARIES */
            TableQuery<Helpers.Database.Library> libraries = null;
            await Task.Run(() => { libraries = App.Database.Table<Helpers.Database.Library>(); });

            /* VALIDATE LIBRARIES */
            await Task.Run(async () =>
            {
               foreach (var library in libraries)
               {
                  var libraryService = Library.LibraryService.Get(library);
                  var previousState = library.Available;
                  await libraryService.Validate(library);
                  if (library.Available != previousState)
                  { App.Database.Update(library); }
               }
            });

            /* RESULT */
            var hasLibriries = false;
            await Task.Run(() =>
            {
               App.Settings.Paths.Libraries = libraries
                  .Where(x => x.Available == true)
                  .Where(x => x.LibraryPath != "")
                  .ToArray();
               hasLibriries = App.Settings.Paths.Libraries.Length != 0;
            });
            return hasLibriries;

         }
         catch (Exception) { throw; }
      }

   }
}
