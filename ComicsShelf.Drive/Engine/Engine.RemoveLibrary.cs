using ComicsShelf.Store;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class Engine<T>
   {

      public async Task<bool> RemoveLibrary(LibraryStoreVM library)
      {
         try {

            // CLEAR CACHE
            // TODO

            await Task.CompletedTask;
            return true;
         }
         catch (Exception ex) { ComicsShelf.Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
