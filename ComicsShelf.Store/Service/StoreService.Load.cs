using System;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class StoreService
   {
      const string LibrariesKey = "ComicsShelf.LibraryIDs";

      private async Task<string[]> LoadStoreIDs()
      {
         try
         {
            var libraryIDsJSON = Xamarin.Essentials.Preferences.Get(LibrariesKey, "[]");
            var libraryIDs = await Base.Helpers.Json.Deserialize<string[]>(libraryIDsJSON);
            return libraryIDs;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return new string[] { }; }
      }

      private async Task<LibraryStoreVM> LoadStoreLibrary(string libraryID)
      {
         try
         {
            var libraryJSON = Xamarin.Essentials.Preferences.Get($"{LibrariesKey}.{libraryID}", "");
            if (string.IsNullOrEmpty(libraryJSON)) { return null; }

            var library = await Base.Helpers.Json.Deserialize<LibraryStoreVM>(libraryJSON);

            return library;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return null; }
      }

   }
}
