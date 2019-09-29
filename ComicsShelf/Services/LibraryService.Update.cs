using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Services
{
   partial class LibraryService
   {

      internal static async Task UpdateLibrary(string libraryID)
      {
         Helpers.AppCenter.TrackEvent("Library.OnUpdating");
         var library = DependencyService.Get<Store.ILibraryStore>().GetLibrary(libraryID);
         using (var service = new LibraryService(library))
         {
            await service.UpdateLibrary();
         }
         Helpers.AppCenter.TrackEvent("Library.OnUpdated");
      }

      internal async Task<bool> UpdateLibrary()
      {
         try
         {
            if (!await this.Statistics()) { return false; }
            if (!await this.SaveSyncData()) { return false; }
            if (!await this.SaveData()) { return false; }
            this.Notify.Send(this.Library, false);
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

   }
}
