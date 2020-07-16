using ComicsShelf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

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
            var libraryItemIDs = await Helpers.Json.Deserialize<string[]>(libraryItemIDsJson);
            return libraryItemIDs;
         }
         catch (Exception ex) { Helpers.Message.Show(ex); return new string[] { }; }
      }

      public static async Task<bool> SetItems(string libraryID, List<ItemVM> items)
      {
         try
         {
            var libraryItemIDs = items.Select(x => x.ID).ToArray();
            var libraryItemIDsJson = await Helpers.Json.Serialize(libraryItemIDs);
            Xamarin.Essentials.Preferences.Set(items_sync(libraryID), libraryItemIDsJson);

            return true;
         }
         catch (Exception ex) { Helpers.Message.Show(ex); return false; }
      }

      public static void RemoveItems(string libraryID, string[] itemIDs)
      {
         try
         {
            itemIDs.ForEach(itemID => RemoveItem(itemID));
            Xamarin.Essentials.Preferences.Remove(items_sync(libraryID));
         }
         catch (Exception ex) { Helpers.Message.Show(ex); }
      }

   }
}
