﻿using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{
   internal class Startup: BaseEngine
   {

      public static async Task Execute()
      {
         try
         {
            var engine = new Startup();
            {
               engine.CheckPermissions(
                  async () =>
                  {
                     App.HomeData = new Views.Home.HomeData();
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

            // SHOW LIBRARY SELECTOR IF HASNT A LIBRARY DEFINED YET
            if (!await this.ValidateLibraryPath())
            { await Helpers.NavVM.PushAsync<Views.Library.LibraryVM>(true); return; }

            // SHOW HOME VIEW
            // TODO: REMOVE THIS INVOKE, JUST PUSH THE VIEW AND, STARTS THE FOLLOWING SEARCH.EXECUTE ON A BACKGROUND NOT BLOCKING THREAD
            // Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
            await Helpers.NavVM.PushAsync<Views.Home.HomeVM>(true, App.HomeData);
            // );

            // START SEARCH ENGINE
            await Task.Factory.StartNew(Engine.Search.Execute, TaskCreationOptions.LongRunning);

         }
         catch (Exception ex) { throw; }
      }

      private async Task<bool> ValidateLibraryPath()
      {
         try
         {
            this.Notify(R.Strings.STARTUP_ENGINE_VALIDATING_LIBRARY_PATH_MESSAGE);

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

   }
}
