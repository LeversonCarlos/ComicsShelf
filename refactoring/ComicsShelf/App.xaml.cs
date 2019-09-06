using Xamarin.Essentials;
using Xamarin.Forms;

namespace ComicsShelf
{
   public partial class App : Application
   {

      //TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
      //To debug on Android emulators run the web backend against .NET Core not IIS
      //If using other emulators besides stock Google images you may need to adjust the IP address
      public static string AzureBackendUrl =
          DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000" : "http://localhost:5000";
      public static bool UseMockDataStore = true;

      public App()
      {
         InitializeComponent();
         this.MainPage = new Main.InitialPage();

         if (UseMockDataStore)
         { DependencyService.Register<Store.ILibraryStore, Store.Mock.MockLibraryStore>(); }
         // else { DependencyService.Register<AzureDataStore>(); }

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
