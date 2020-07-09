using ComicsShelf.Drive;
using ComicsShelf.Screens.Reading;
using ComicsShelf.Screens.Splash;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using Xamarin.Forms;

namespace ComicsShelf.Services.Hub
{
   public partial class HubService
   {

      public void Register()
      {
         DependencyService.Register<LocalDrive>();
         DependencyService.Register<OneDrive>();
         DependencyService.Register<IStoreService, StoreService>();
         Screens.Home.Extentions.Initialize();
         Screens.Settings.Extentions.Initialize();
      }

      public void Start()
      {
         try
         {
            Helpers.Cover.Init();

            Helpers.Notify.ItemsUpdate(itemsList => Engine.AnalysisData.Service.Execute(itemsList));

            Routing.RegisterRoute(SplashExtentions.GetRoute(), SplashExtentions.GetPageType());
            Routing.RegisterRoute(ReadingExtentions.GetRoute(), ReadingExtentions.GetPageType());

            DependencyService.Get<IStoreService>().Initialize();
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }

      public void Sleep()
      {
         Helpers.Notify.AppSleep();
      }

      public void Resume()
      {
         Engine.CoverExtraction.Service.Execute();
      }

      internal void SectionsUpdate(SectionVM[] sections)
      {
         Screens.Home.Extentions.Instance?.ApplySections(sections);
         Engine.CoverExtraction.Service.Execute();
      }

   }
}
