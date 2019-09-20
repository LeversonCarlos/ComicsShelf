using Xamarin.Essentials;
using Xamarin.Forms;

namespace ComicsShelf
{
   public partial class App : Application
   {

      public App()
      {
         InitializeComponent();
         this.MainPage = new Main.InitialPage();
         DependencyService.Register<Engines.OneDrive.OneDriveConnector>();
         DependencyService.Register<Engines.LocalDrive.LocalDriveEngine>();
         DependencyService.Register<Engines.OneDrive.OneDriveEngine>();
         DependencyService.Register<Store.ILibraryStore, Store.LibraryStore>();
         this.MainPage = new Main.MainPage();
      }

      protected async override void OnStart()
      {
         Controls.CoverView.InitDefaultSize();
         await Helpers.DefaultCover.LoadDefaultCover();
         await DependencyService.Get<Store.ILibraryStore>().LoadLibrariesAsync();
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
