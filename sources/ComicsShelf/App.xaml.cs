using ComicsShelf.Drive;
using ComicsShelf.Screens.Reading;
using ComicsShelf.Screens.Splash;
using ComicsShelf.Store;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf
{
   public partial class App : Application
   {

      public App()
      {
         InitializeComponent();
         MainPage = new Screens.Startup.StartupPage();
         DependencyService.Register<LocalDrive>();
         DependencyService.Register<OneDrive>();
         DependencyService.Register<IStoreService, StoreService>();
      }

      protected override void OnStart()
      {
         try
         {
            MainPage = new Screens.Shells.ShellPage();
            Helpers.Notify.LibraryAdd(library => Engine.RefreshData.Service.Execute(library));
            Helpers.Notify.LibraryRemove(library => Engine.AnalysisData.Service.Execute(null));
            Helpers.Notify.ItemsUpdate(itemsList => Engine.AnalysisData.Service.Execute(itemsList));
            Helpers.Notify.SectionsUpdate(itemList => Engine.CoverExtraction.Service.Execute());
            Routing.RegisterRoute(SplashExtentions.GetRoute(), SplashExtentions.GetPageType());
            Routing.RegisterRoute(ReadingExtentions.GetRoute(), ReadingExtentions.GetPageType());
            Helpers.Cover.Init();
            Task.Factory.StartNew(() => DependencyService.Get<IStoreService>().InitializeAsync(), TaskCreationOptions.LongRunning);
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }

      protected override void OnSleep()
      {
         Helpers.Notify.AppSleep();
      }

      protected override void OnResume()
      {
         Engine.CoverExtraction.Service.Execute();
      }

   }
}