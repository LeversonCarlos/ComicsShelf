using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ComicsShelf
{
   public partial class App : Application
   {

      public App()
      {
         InitializeComponent();
         MainPage = new Helpers.Controls.Pages.NavPage();
      }


      private Helpers.Settings.Settings _Settings = null;
      internal static Helpers.Settings.Settings Settings
      {
         get { return ((App)Application.Current)._Settings; }
         set { ((App)Application.Current)._Settings = value; }
      }

      private Helpers.Database.dbContext _Database = null;
      internal static Helpers.Database.dbContext Database
      {
         get { return ((App)Application.Current)._Database; }
         set { ((App)Application.Current)._Database = value; }
      }

      private Views.Home.HomeData _HomeData = null;
      public static Views.Home.HomeData HomeData
      {
         get { return ((App)Application.Current)._HomeData; }
         set { ((App)Application.Current)._HomeData = value; }
      }


      protected override async void OnStart()
      { await Engine.Startup.Execute(); }

      protected override void OnSleep()
      { /* Handle when your app sleeps */ }

      protected override void OnResume()
      { /* Handle when your app resumes */ }

   }
}