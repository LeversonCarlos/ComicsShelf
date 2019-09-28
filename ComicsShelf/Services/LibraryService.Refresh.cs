using System;
using System.Threading.Tasks;

namespace ComicsShelf.Services
{
   partial class LibraryService
   {

      internal static void RefreshLibrary(Store.LibraryModel library)
      {
         Task.Run(async () =>
         {
            using (var service = new LibraryService(library))
            {
               if (!await service.RefreshLibrary())
               {
                  await Task.Delay(15 * 1000);
                  RefreshLibrary(library);
               }
            }
         });
      }

      private async Task<bool> RefreshLibrary()
      {
         try
         {
            Helpers.AppCenter.TrackEvent("Library.OnRefreshing", $"LibraryType:{this.Library.Type.ToString()}");
            if (!await this.LoadSyncData()) { return false; }
            if (!await this.NotifyData()) { return false; }
            if (!await this.Statistics()) { return false; }

            if (!await this.SearchData()) { return false; }
            if (!await this.LoadSyncData()) { return false; }
            if (!await this.NotifyData()) { return false; }
            if (!await this.Statistics()) { return false; }

            if (!await this.SaveSyncData()) { return false; }
            if (!await this.SaveData()) { return false; }

            if (!await this.ExtractData()) { return false; }
            if (!await this.Statistics()) { return false; }

            if (!await this.SaveSyncData()) { return false; }
            if (!await this.SaveData()) { return false; }
            Helpers.AppCenter.TrackEvent("Library.OnRefreshed", $"LibraryType:{this.Library.Type.ToString()}");
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
         finally { this.Notify.Send(this.Library, false); GC.Collect(); }
      }

   }
}
