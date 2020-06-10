using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class StoreService
   {
      string LibraryItemSyncKey(string ItemID) => $"{SyncKey()}.Item.{ItemID}";

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
