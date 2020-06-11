using Xamarin.Forms;

namespace ComicsShelf
{
   public partial class App : Application
   {

      public App()
      {
         InitializeComponent();
         this.MainPage = new Main.MainPage();
         DependencyService.Register<Engines.LocalDrive.LocalDriveEngine>();
         DependencyService.Register<Engines.OneDrive.OneDriveEngine>();
         DependencyService.Register<Store.ILibraryStore, Store.LibraryStore>();
      }

      protected override async void OnStart()
      {
         Helpers.AppCenter.Initialize();
         Helpers.AppCenter.TrackEvent("App.OnStart");
         Controls.CoverView.InitDefaultSize();
         await Helpers.DefaultCover.LoadDefaultCover();
         await DependencyService.Get<Store.ILibraryStore>().LoadLibrariesAsync();
      }

      protected override void OnSleep()
      { Helpers.AppCenter.TrackEvent("App.OnSleep"); this.DoSleep(); }

      protected override void OnResume()
      { Helpers.AppCenter.TrackEvent("App.OnResume"); this.DoResume(); }

   }
}
