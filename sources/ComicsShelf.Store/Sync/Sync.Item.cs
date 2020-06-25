using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class Sync
   {
      static string item_sync(string ItemID) => $"{base_sync()}.Item.{ItemID}";

      public static async Task<ItemVM> GetItem(string itemID)
      {
         try
         {
            var libraryItemJson = Xamarin.Essentials.Preferences.Get(item_sync(itemID), "");
            if (string.IsNullOrEmpty(libraryItemJson)) { return null; }

            var libraryItem = await Helpers.Json.Deserialize<ItemVM>(libraryItemJson);

            return libraryItem;
         }
         catch (Exception ex) { Helpers.App.ShowMessage(ex); return null; }
      }

      public static async Task<bool> SetItem(ItemVM libraryItem)
      {
         try
         {
            var libraryItemJson = await Helpers.Json.Serialize(libraryItem);
            Xamarin.Essentials.Preferences.Set(item_sync(libraryItem.ID), libraryItemJson);
            return true;
         }
         catch (Exception ex) { Helpers.App.ShowMessage(ex); return false; }
      }

      public static void RemoveItem(string itemID)
      {
         try
         {
            Xamarin.Essentials.Preferences.Remove(item_sync(itemID));
         }
         catch (Exception ex) { Helpers.App.ShowMessage(ex); }
      }

   }
}
