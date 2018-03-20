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
            (object sender, System.EventArgs e) =>
            {
               this.MainPage.Navigation.PushAsync(new Startup.StartupPage());
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
