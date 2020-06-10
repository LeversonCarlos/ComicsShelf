using System;
using System.Data;
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

            var libraryIDs = await this.GetSyncLibraryIDs();

            var storeLibraryTasks = libraryIDs
               .Select(libraryID => this.GetSyncLibrary(libraryID))
               .ToArray();
            this.LibraryList = await Task.WhenAll(storeLibraryTasks);

            foreach (var library in this.LibraryList)
            {
               var libraryItemIDs = await this.GetSyncLibraryItemsIDs(library.ID);
               var libraryItemTasks = libraryItemIDs
                  .Select(itemID => this.GetSyncLibraryItem(itemID))
                  .ToArray();
               var libraryItems = await Task.WhenAll(libraryItemTasks);
               this.LibraryItems.Add(library.ID, libraryItems.ToList());
            }

            var notifyTasks = this.LibraryList
               .Select(x => new Task(() => this.NotifyLibraryAdd(x)))
               .ToArray();
            await Task.WhenAll(notifyTasks);

            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
