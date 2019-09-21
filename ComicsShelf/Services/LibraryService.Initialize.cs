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
            this.Notify.Send(this.Library, $"{this.Library.Description}: {R.Strings.STARTUP_ENGINE_LOADING_DATABASE_MESSAGE}");
            if (!await this.LoadData()) { return; }
            if (!await this.NotifyData()) { return; }
            if (!await this.Statistics()) { return; }
            this.Notify.Send(this.Library, false);
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
         finally { GC.Collect(); }
      }

   }
}
