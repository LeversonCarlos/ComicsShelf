using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Services
{
   partial class LibraryService
   {

      internal static async Task UpdateLibrary(string libraryID)
      {
         var library = DependencyService.Get<Store.ILibraryStore>().GetLibrary(libraryID);
         using (var service = new LibraryService(library))
         {
            await service.UpdateLibrary();
         }
      }

      SemaphoreSlim updateLibrarySemaphore = new SemaphoreSlim(1, 1);
      internal async Task<bool> UpdateLibrary()
      {
         try
         {
            await updateLibrarySemaphore.WaitAsync();
            Helpers.AppCenter.TrackEvent("Library.OnUpdating");
            if (!await this.Statistics()) { return false; }
            if (!await this.SaveSyncData()) { return false; }
            if (!await this.SaveData()) { return false; }
            this.Notify.Send(this.Library, false);
            Helpers.AppCenter.TrackEvent("Library.OnUpdated");
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
         finally { updateLibrarySemaphore.Release(); }
      }

   }
}
