using ComicsShelf.Drive;
using ComicsShelf.Screens.Reading;
using ComicsShelf.Screens.Splash;
using ComicsShelf.Store;
using System;
using Xamarin.Forms;

namespace ComicsShelf.Services.Hub
{
   public class HubService
   {

      public void Start()
      {
         try
         {
            DependencyService.Register<LocalDrive>();
            DependencyService.Register<OneDrive>();
            DependencyService.Register<IStoreService, StoreService>();

            Helpers.Cover.Init();

            Helpers.Notify.LibraryAdd(library => Engine.RefreshData.Service.Execute(library));
            Helpers.Notify.LibraryRemove(library => Engine.AnalysisData.Service.Execute(null));
            Helpers.Notify.ItemsUpdate(itemsList => Engine.AnalysisData.Service.Execute(itemsList));
            Helpers.Notify.SectionsUpdate(itemList => Engine.CoverExtraction.Service.Execute());

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

   }
}
