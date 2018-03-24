using System;
using System.Threading.Tasks;

namespace ComicsShelf.Startup
{
   public class StartupVM : Helpers.ViewModels.DataVM<StartupData>
   {

      #region New
      public StartupVM()
      {
         this.Title = R.Strings.AppTitle;
         this.ViewType = typeof(StartupPage);

         this.Data = new StartupData();
         this.Initialize += this.OnInitialize;
      }
      #endregion

      #region OnInitialize
      private async void OnInitialize()
      {
         try
         {
            this.Data.Progress = 0;

            this.Data.Text = R.Strings.STARTUP_LOADING_SETTINGS_MESSAGE;
            await this.OnInitialize_Settings();

            this.Data.Text = R.Strings.STARTUP_DEFINING_COMICS_PATH_MESSAGE;
            await this.OnInitialize_ComicsPath();

            this.Data.Text = R.Strings.STARTUP_SEARCHING_COMIC_FILES_MESSAGE;
            await this.OnInitialize_Search();

            this.Data.Progress = 1.00;
            this.Data.Text = "";
            this.Data.Details = "";
            if (await App.Message.Confirm("Inicializacao concluída.\nDeseja fechar a aplicação"))
            { System.Environment.Exit(0); }
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

      #region OnInitialize_Settings
      private async Task OnInitialize_Settings()
      {
         try
         {
            await App.Settings.Initialize();
         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region OnInitialize_ComicsPath
      private async Task OnInitialize_ComicsPath()
      {
         try
         {

            /* LOAD CONFIGS DATA */
            var configsTable = App.Settings.Database.Table<Helpers.Settings.Configs>();
            var configs = configsTable.FirstOrDefault();
            if (configs == null)
            {
               configs = new Helpers.Settings.Configs();
               App.Settings.Database.Insert(configs);
            }

            /* VALIDATE COMICS PATH */
            var fileSystem = Helpers.FileSystem.Get();
            configs.ComicsPath = await fileSystem.GetComicsPath(configs.ComicsPath);

            /* STORE DATA */
            App.Settings.Database.Update(configs);
            App.Settings.Paths.ComicsPath = configs.ComicsPath;

         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region OnInitialize_Search
      private async Task OnInitialize_Search()
      {
         try
         {

            // INITIALIZE
            this.Data.Progress = 0;
            var fileSystem = Helpers.FileSystem.Get();

            // LOCATE COMICS LIST
            var fileList = await fileSystem.GetFiles(App.Settings.Paths.ComicsPath);
            var fileQuantity = fileList.Length;

            // LOOP THROUGH FILE LIST
            for (int fileIndex = 0; fileIndex < fileQuantity; fileIndex++)
            {
               var filePath = fileList[fileIndex];
               this.Data.Progress = ((double)fileIndex / (double)fileQuantity);
               this.Data.Details = filePath;

               /* DO THE MAGIC */
               await Task.Delay(500);

            }

         }
         catch (Exception ex) { throw; }
      }
      #endregion

   }
}