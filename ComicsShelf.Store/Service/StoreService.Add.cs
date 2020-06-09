using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class StoreService
   {

      public async Task<bool> AddLibraryAsync(LibraryStoreVM library)
      {
         try
         {

            if (!await this.SetSyncedLibrary(library)) { return false; }

            this.LibraryList = this.LibraryList
               .Union(new[] { library })
               .ToArray();
            if (!await this.SetSyncedLibraries()) { return false; }

            this.NotifyLibraryAdd(library);

            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
