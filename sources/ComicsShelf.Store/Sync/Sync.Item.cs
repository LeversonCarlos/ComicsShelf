using System;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class Sync
   {
      static string item_sync(string ItemID) => $"{base_sync()}.Item.{ItemID}";

      public static async Task<ItemStoreVM> GetItem(string itemID)
      {
         try
         {
            var libraryItemJson = Xamarin.Essentials.Preferences.Get(item_sync(itemID), "");
            if (string.IsNullOrEmpty(libraryItemJson)) { return null; }

            var libraryItem = await Base.Helpers.Json.Deserialize<ItemStoreVM>(libraryItemJson);

            return libraryItem;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return null; }
      }

      public static async Task<bool> SetItem(ItemStoreVM libraryItem)
      {
         try
         {
            var libraryItemJson = await Base.Helpers.Json.Serialize(libraryItem);
            Xamarin.Essentials.Preferences.Set(item_sync(libraryItem.ID), libraryItemJson);
            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
