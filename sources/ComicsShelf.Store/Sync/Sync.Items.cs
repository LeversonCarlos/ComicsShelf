using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class Sync
   {
      static string items_sync(string libraryID) => $"{base_sync()}.Library.{libraryID}.Items";

      public static async Task<string[]> GetItems(string libraryID)
      {
         try
         {
            var libraryItemIDsJson = Xamarin.Essentials.Preferences.Get(items_sync(libraryID), "[]");
            var libraryItemIDs = await Base.Helpers.Json.Deserialize<string[]>(libraryItemIDsJson);
            return libraryItemIDs;
         }
         catch (Exception ex) { Helpers.AppInsights.TrackException(ex); return new string[] { }; }
      }

      public static async Task<bool> SetItems(string libraryID, List<ItemStoreVM> items)
      {
         try
         {
            var libraryItemIDs = items.Select(x => x.ID).ToArray();
            var libraryItemIDsJson = await Base.Helpers.Json.Serialize(libraryItemIDs);
            Xamarin.Essentials.Preferences.Set(items_sync(libraryID), libraryItemIDsJson);

            return true;
         }
         catch (Exception ex) { Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
