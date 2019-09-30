using System;
using System.Threading.Tasks;

namespace ComicsShelf.Services
{
   partial class LibraryService
   {

      internal async Task InitializeLibrary()
      {
         try
         {
            Helpers.AppCenter.TrackEvent("Libraries.OnInitializing", $"LibraryType:{this.Library.Type.ToString()}");
            this.Notify.Send(this.Library, $"{this.Library.Description}: {R.Strings.STARTUP_ENGINE_LOADING_DATABASE_MESSAGE}");
            if (!await this.LoadData()) { return; }
            if (!await this.NotifyData()) { return; }
            if (!await this.Statistics()) { return; }
            this.Notify.Send(this.Library, false);
            Helpers.AppCenter.TrackEvent("Libraries.OnInitialized", $"LibraryType:{this.Library.Type.ToString()}");
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
         finally { GC.Collect(); }
      }

   }
}
