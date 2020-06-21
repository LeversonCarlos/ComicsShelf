using ComicsShelf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class StoreService
   {

      public async Task<bool> UpdateItemAsync(ItemVM[] itemList)
      {
         try
         {
            if (itemList?.Length == 0) { return true; }
            var changedItemIDs = new List<string>();
            var libraryID = itemList.Select(x => x.LibraryID).FirstOrDefault();
            var library = this.GetLibrary(libraryID);
            var libraryItemIDs = this.ItemList[libraryID].Select(x => x.Key).ToArray();

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
                  this.ItemList[libraryID][updateItem.ID].SetData(updateItem);
                  changedItemIDs.Add(updateItem.ID);
               }
            }

            // CHECK FOR NEW ITEMS
            var newItems = itemList
               .Where(x => !libraryItemIDs.Contains(x.ID))
               .ToArray();
            foreach (var newItem in newItems)
            {
               if (!await Sync.SetItem(newItem)) { return false; }
               this.ItemList[libraryID].Add(newItem.ID, newItem);
               changedItemIDs.Add(newItem.ID);
            }

            // UPDATE THE LIBRARY's ITEM IDs
            if (changedItemIDs.Any())
            {
               await Sync.SetItems(libraryID, this.ItemList[libraryID].Values.ToList());
               var notifyItems = this.ItemList[libraryID]
                  .Where(x => changedItemIDs.Contains(x.Key))
                  .Select(x => x.Value)
                  .ToArray();
               Notifyers.Notify.ItemsUpdate(library, notifyItems);
            }

            return true;
         }
         catch (Exception ex) { Helpers.App.ShowMessage(ex); return false; }
      }

   }
}
