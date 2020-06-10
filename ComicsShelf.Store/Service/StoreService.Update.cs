using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class StoreService
   {

      public async Task<bool> UpdateItemAsync(ItemStoreVM item)
      {
         try
         {

            if (!await Sync.SetItem(item)) { return false; }

            if (!this.ItemList[item.LibraryID].Any(x => x.ID == item.ID))
            {
               this.ItemList[item.LibraryID].Add(item);
               await Sync.SetItems(item.LibraryID, this.ItemList[item.LibraryID]);
            }

            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
