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

            var libraryJson = await Base.Helpers.Json.Serialize(library);
            Xamarin.Essentials.Preferences.Set($"{LibraryIDs}.{library.ID}", libraryJson);

            this.LibraryList = this.LibraryList
               .Union(new[] { library })
               .ToArray();

            this.NotifyLibraryAdd(library);

            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
