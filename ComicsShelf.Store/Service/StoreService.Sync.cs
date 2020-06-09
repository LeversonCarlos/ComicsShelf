using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class StoreService
   {
      const string SyncKey = "ComicsShelf.LibraryIDs";

      private async Task<string[]> GetSyncedIDs()
      {
         try
         {
            var libraryIDsJSON = Xamarin.Essentials.Preferences.Get(SyncKey, "[]");
            var libraryIDs = await Base.Helpers.Json.Deserialize<string[]>(libraryIDsJSON);
            return libraryIDs;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return new string[] { }; }
      }

      private async Task<LibraryStoreVM> GetSyncedLibrary(string libraryID)
      {
         try
         {
            var libraryJSON = Xamarin.Essentials.Preferences.Get($"{SyncKey}.{libraryID}", "");
            if (string.IsNullOrEmpty(libraryJSON)) { return null; }

            var library = await Base.Helpers.Json.Deserialize<LibraryStoreVM>(libraryJSON);

            return library;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return null; }
      }

      private async Task<bool> SetSyncedLibrary(LibraryStoreVM library)
      {
         try
         {
            var libraryJson = await Base.Helpers.Json.Serialize(library);
            Xamarin.Essentials.Preferences.Set($"{SyncKey}.{library.ID}", libraryJson);
            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

      private async Task<bool> RemoveSyncedLibrary(LibraryStoreVM library)
      {
         try
         {
            Xamarin.Essentials.Preferences.Remove($"{SyncKey}.{library.ID}");
            await Task.CompletedTask;
            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

      private async Task<bool> SetSyncedLibraries()
      {
         try
         {
            var libraryIDs = this.LibraryList.Select(x => x.ID).ToArray();
            var libraryIDsJson = await Base.Helpers.Json.Serialize(libraryIDs);
            Xamarin.Essentials.Preferences.Set(SyncKey, libraryIDsJson);
            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
