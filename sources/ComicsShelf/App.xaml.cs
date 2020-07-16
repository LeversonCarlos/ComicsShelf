using ComicsShelf.Services.Hub;
using Xamarin.Forms;

namespace ComicsShelf
{
   public partial class App : Application
   {

      public App()
      {
         InitializeComponent();
         MainPage = new Screens.Shells.ShellPage();
         DependencyService.Register<HubService>();
         DependencyService.Get<HubService>().Register();
      }

      protected override void OnStart() =>
         DependencyService.Get<HubService>().Start();

      protected override void OnSleep() =>
         DependencyService.Get<HubService>().Sleep();

      protected override void OnResume() =>
         DependencyService.Get<HubService>().Resume();

   }
}