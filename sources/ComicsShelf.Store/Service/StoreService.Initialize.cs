using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class StoreService
   {

      public async Task<bool> InitializeAsync()
      {
         try
         {

            var libraryIDs = await Sync.GetLibraries();

            var dataTasks = libraryIDs
               .Select(libraryID => Sync.GetLibrary(libraryID))
               .ToArray();
            this.LibraryList = (await Task.WhenAll(dataTasks))
               .Where(x => x != null)
               .ToArray();

            foreach (var library in this.LibraryList)
            {

               var itemIDs = await Sync.GetItems(library.ID);

               var itemTasks = itemIDs
                  .Select(itemID => Sync.GetItem(itemID))
                  .ToArray();
               var itemList = (await Task.WhenAll(itemTasks))
                  .Where(x => x != null)
                  .ToArray();

               this.ItemList.Add(library.ID, new SortedList<string, ViewModels.ItemVM>(itemList.ToDictionary(k => k.ID, v => v)));

               Helpers.Notify.LibraryAdd(library);
               Helpers.Notify.ItemsUpdate(itemList);
            }

            return true;
         }
         catch (Exception ex) { Helpers.Message.Show(ex); return false; }
      }

   }
}
