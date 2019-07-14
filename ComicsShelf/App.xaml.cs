using Xamarin.Forms;

namespace ComicsShelf
{
   public partial class App : Application
   {

      public App()
      {
         InitializeComponent();
         DependencyService.Register<Libraries.LibraryStore>();
         DependencyService.Register<Libraries.LibraryService>();
         DependencyService.Register<Engines.LocalDrive.LocalDriveEngine>();
         DependencyService.Register<Engines.OneDrive.OneDriveEngine>();
         DependencyService.Register<Engines.OneDrive.OneDriveConnector>();
         MainPage = new AppShell();
      }

      protected override async void OnStart()
      {
         Helpers.AppCenter.Initialize();
         Controls.CoverView.InitDefaultSize();
         await Helpers.DefaultCover.LoadDefaultCover();
         await DependencyService.Get<Libraries.LibraryStore>().LoadLibraries();
      }

      protected override void OnSleep()
      {
         // Handle when your app sleeps
      }

      protected override void OnResume()
      {
         // Handle when your app resumes
      }
   }
}
