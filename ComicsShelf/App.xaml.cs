using ComicsShelf.Services;
using Xamarin.Forms;

namespace ComicsShelf
{
   public partial class App : Application
   {

      public App()
      {
         InitializeComponent();
         DependencyService.Register<MockDataStore>();
         DependencyService.Register<Libraries.LibraryStore>();
         DependencyService.Register<Engines.LocalDrive.LocalDriveEngine>();
         MainPage = new AppShell();
      }

      protected override void OnStart()
      {
         try
         {
            var libraryStore = DependencyService.Get<Libraries.LibraryStore>();
            libraryStore.LoadLibraries();
         }
         catch { throw; }
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
