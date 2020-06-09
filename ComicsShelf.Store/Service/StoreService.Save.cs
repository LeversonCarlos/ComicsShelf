using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class StoreService
   {

      private async Task<bool> SaveLibraryAsync(LibraryStoreVM library)
      {
         try
         {
            var libraryJson = await Base.Helpers.Json.Serialize(library);
            Xamarin.Essentials.Preferences.Set($"{LibrariesKey}.{library.ID}", libraryJson);
            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

      private async Task<bool> RemoveLibraryFromStore(LibraryStoreVM library)
      {
         try
         {
            Xamarin.Essentials.Preferences.Remove($"{LibrariesKey}.{library.ID}");
            await Task.CompletedTask;
            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

      private async Task<bool> SaveLibrariesAsync()
      {
         try
         {
            var libraryIDs = this.LibraryList.Select(x => x.ID).ToArray();
            var libraryIDsJson = await Base.Helpers.Json.Serialize(libraryIDs);
            Xamarin.Essentials.Preferences.Set(LibrariesKey, libraryIDsJson);
            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
