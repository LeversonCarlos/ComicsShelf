using Xamarin.Forms;

namespace ComicsShelf.Startup
{
   public class StartupVM : Helpers.ViewModels.DataVM<StartupData>
   {

      public StartupVM()
      {
         this.Title = R.Strings.AppTitle;
         this.ViewType = typeof(StartupPage);
         this.Data = new StartupData();
         this.Initialize += this.OnInitialize;
      }

      private void OnInitialize()
      {
         MessagingCenter.Subscribe<StartupData>(this, "Startup", (data) =>
         {
            this.Data.Text = data.Text;
            this.Data.Details = data.Details;
            this.Data.Progress = data.Progress;
         });
      }

   }
}