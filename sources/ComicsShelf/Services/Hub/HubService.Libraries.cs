using ComicsShelf.Drive;
using ComicsShelf.Screens.Reading;
using ComicsShelf.Screens.Splash;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using Xamarin.Forms;

namespace ComicsShelf.Services.Hub
{
   partial class HubService
   {

      public void LibraryAdd(LibraryVM library)
      {
         try
         {
            Screens.Settings.Extentions.Instance?.AddLibrary(library);
            Engine.RefreshData.Service.Execute(library);
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }

      public void LibraryRemove(LibraryVM library)
      {
         try
         {
            Screens.Settings.Extentions.Instance?.RemoveLibrary(library);
            Engine.AnalysisData.Service.Execute(null);
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }

   }
}
