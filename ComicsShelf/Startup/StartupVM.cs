using System;

namespace ComicsShelf.Startup
{
   public class StartupVM : Helpers.ViewModels.DataVM<StartupData>
   {

      #region New
      public StartupVM()
      {
         this.Title = "Startup Screen";
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
            this.Data.Progress = 25;
            this.Data.Text = "Loading Settings";
            await System.Threading.Tasks.Task.Delay(5000);

            this.Data.Progress = 50;
            this.Data.Text = "Loading comics folder";
            await System.Threading.Tasks.Task.Delay(5000);

            this.Data.Progress = 75;
            this.Data.Text = "Searching comics on device";
            await System.Threading.Tasks.Task.Delay(5000);

            this.Data.Progress = 95;
            this.Data.Text = "Showing initial folder";
            await System.Threading.Tasks.Task.Delay(5000);

         }
         catch (Exception ex) { throw; }
      }
      #endregion

   }
}