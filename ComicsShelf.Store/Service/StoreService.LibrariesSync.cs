using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class StoreService
   {

      private async Task<LibraryStoreVM> GetSyncLibrary(string libraryID)
      {
         try
         {
            var libraryJSON = Xamarin.Essentials.Preferences.Get(LibrarySyncKey(libraryID), "");
            if (string.IsNullOrEmpty(libraryJSON)) { return null; }

            var library = await Base.Helpers.Json.Deserialize<LibraryStoreVM>(libraryJSON);

            return library;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return null; }
      }

      private async Task<bool> SetSyncLibrary(LibraryStoreVM library)
      {
         try
         {
            var libraryJson = await Base.Helpers.Json.Serialize(library);
            Xamarin.Essentials.Preferences.Set(LibrarySyncKey(library.ID), libraryJson);
            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

      private async Task<bool> RemoveSyncLibrary(LibraryStoreVM library)
      {
         try
         {
            Xamarin.Essentials.Preferences.Remove(LibrarySyncKey(library.ID));
            await Task.CompletedTask;
            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
