using ComicsShelf.Drive;
using ComicsShelf.Screens.Reading;
using ComicsShelf.Screens.Splash;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Services.Hub
{
   public class HubService
   {

      public void Register()
      {
         DependencyService.Register<LocalDrive>();
         DependencyService.Register<OneDrive>();
         DependencyService.Register<IStoreService, StoreService>();
      }

      public void Start()
      {
         try
         {
            Helpers.Cover.Init();

            Helpers.Notify.LibraryAdd(Application.Current, async library => await Engine.RefreshData.Service.Execute(library));
            Helpers.Notify.LibraryRemove(Application.Current, async library => await Engine.AnalysisData.Service.Execute(null));
            Helpers.Notify.ItemsUpdate(Application.Current, async itemsList => await Engine.AnalysisData.Service.Execute(itemsList));
            Helpers.Notify.SectionsUpdate(Application.Current, async itemList => await Engine.CoverExtraction.Service.Execute());

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
