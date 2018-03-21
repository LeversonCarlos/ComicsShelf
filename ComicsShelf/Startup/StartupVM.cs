using System;

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
            this.Data.Text = R.Strings.STARTUP_LOADING_SETTINGS_MESSAGE;
            await System.Threading.Tasks.Task.Delay(2000);
            this.Data.Progress = 0.30;

            this.Data.Text = R.Strings.STARTUP_DEFINING_COMICS_PATH_MESSAGE;
            await System.Threading.Tasks.Task.Delay(2000);
            this.Data.Progress = 0.50;

            this.Data.Text = R.Strings.STARTUP_SEARCHING_COMIC_FILES_MESSAGE;
            await System.Threading.Tasks.Task.Delay(2000);
            this.Data.Progress = 1.00;

            this.Data.Text = "";
         }
         catch (Exception ex) { throw; }
      }
      #endregion

   }
}