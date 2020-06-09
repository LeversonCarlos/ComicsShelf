using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class StoreService
   {

      LibraryStoreVM[] LibraryList { get; set; }

      public async Task<bool> InitializeAsync()
      {
         try
         {

            var libraryIDs = await this.LoadStoreIDs();

            var storeLibraryTasks = libraryIDs
               .Select(libraryID => this.LoadStoreLibrary(libraryID))
               .ToArray();
            this.LibraryList = await Task.WhenAll(storeLibraryTasks);

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
