using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace ComicsShelf.Store
{
   partial class StoreService
   {

      public Task<bool> UpdateItemAsync(ItemStoreVM item) => UpdateItemAsync(new ItemStoreVM[] { item });

      public async Task<bool> UpdateItemAsync(ItemStoreVM[] itemList)
      {
         try
         {
            var libraryChanged = false;
            var libraryID = itemList.Select(x => x.LibraryID).FirstOrDefault();
            var libraryItems = this.ItemList[libraryID];
            var libraryItemIDs = libraryItems.Select(x => x.ID).ToArray();

            // CHECK FOR CHANGED ITEMS
            var currentItems = itemList
               .Where(x => libraryItemIDs.Contains(x.ID))
               .ToArray();
            foreach (var currentItem in currentItems)
            {
               var updateItem = itemList.Where(x => x.ID == currentItem.ID).FirstOrDefault();
               if (!Equals(currentItem, updateItem))
               {
                  if (!await Sync.SetItem(updateItem)) { return false; }
                  libraryChanged = true;
               }
            }

            // CHECK FOR NEW ITEMS
            var newItems = itemList
               .Where(x => !libraryItemIDs.Contains(x.ID))
               .ToArray();
            foreach (var item in newItems)
            {
               if (!await Sync.SetItem(item)) { return false; }
               libraryItems.Add(item);
               libraryChanged = true;
            }

            // UPDATE THE LIBRARY's ITEM IDs
            if (libraryChanged)
            { await Sync.SetItems(libraryID, this.ItemList[libraryID]); }

            return true;
         }
         catch (Exception ex) { Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
