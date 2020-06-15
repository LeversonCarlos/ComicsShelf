using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class BaseDrive<T>
   {

      public async Task<bool> RemoveLibrary(ItemVM library)
      {
         try {

            // CLEAR CACHE
            // TODO

            await Task.CompletedTask;
            return true;
         }
         catch (Exception ex) { Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
