using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class StoreService
   {
      string LibraryItemsSyncKey(string libraryID) => $"{SyncKey()}.Library.{libraryID}.Items";
      string LibraryItemSyncKey(string ItemID) => $"{SyncKey()}.Item.{ItemID}";

      private async Task<string[]> GetSyncLibraryItemsIDs(string libraryID)
      {
         try
         {
            var libraryItemIDsJson = Xamarin.Essentials.Preferences.Get(LibraryItemsSyncKey(libraryID), "[]");
            var libraryItemIDs = await Base.Helpers.Json.Deserialize<string[]>(libraryItemIDsJson);
            return libraryItemIDs;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return new string[] { }; }
      }

      private async Task<bool> SetSyncLibraryItemsIDs(string libraryID)
      {
         try
         {
            var libraryItemIDs = this.LibraryItems[libraryID].Select(x=> x.ID).ToArray();
            var libraryItemIDsJson = await Base.Helpers.Json.Serialize(libraryItemIDs);
            Xamarin.Essentials.Preferences.Set(LibrariesSyncKey(), libraryItemIDsJson);
            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

      private async Task<ItemStoreVM> GetSyncLibraryItem(string itemID)
      {
         try
         {
            var libraryItemJson = Xamarin.Essentials.Preferences.Get(LibraryItemSyncKey(itemID), "");
            if (string.IsNullOrEmpty(libraryItemJson)) { return null; }

            var libraryItem = await Base.Helpers.Json.Deserialize<ItemStoreVM>(libraryItemJson);

            return libraryItem;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return null; }
      }

      private async Task<bool> SetSyncLibraryItem(ItemStoreVM libraryItem)
      {
         try
         {
            var libraryItemJson = await Base.Helpers.Json.Serialize(libraryItem);
            Xamarin.Essentials.Preferences.Set(LibrarySyncKey(libraryItem.ID), libraryItemJson);
            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
