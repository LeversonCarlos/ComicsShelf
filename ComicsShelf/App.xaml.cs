using Xamarin.Forms;

namespace ComicsShelf
{
   public partial class App : Application
	{
      public App()
      {
         InitializeComponent();
         var initialPage = new ContentPage();
         initialPage.Appearing +=
            async (object sender, System.EventArgs e) =>
            {
               await Helpers.ViewModels.NavVM.PushAsync<Startup.StartupVM>(true);
            };
         MainPage = new NavigationPage(initialPage);
      }

      protected override void OnStart ()
		{ /* Handle when your app starts */ }

		protected override void OnSleep ()
		{ /* Handle when your app sleeps */ }

		protected override void OnResume ()
		{ /* Handle when your app resumes */ }

	}
}
