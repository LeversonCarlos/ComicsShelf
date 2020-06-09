using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class StoreService
   {

      public async Task<bool> RemoveLibraryAsync(LibraryStoreVM library)
      {
         try
         {

            if (!await this.RemoveLibraryFromStore(library)) { return false; }

            this.LibraryList = this.LibraryList
               .Where(x => x.ID != library.ID)
               .ToArray();
            if (!await this.SaveLibrariesAsync()) { return false; }

            this.NotifyLibraryRemove(library);

            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
