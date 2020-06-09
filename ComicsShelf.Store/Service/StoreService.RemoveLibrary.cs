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

            Xamarin.Essentials.Preferences.Remove($"{LibraryIDs}.{library.ID}");

            this.LibraryList = this.LibraryList
               .Where(x=> x.ID != library.ID)
               .ToArray();

            this.NotifyLibraryRemove(library);

            await Task.CompletedTask;
            return true;
         }
         catch (Exception ex) { Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

      public const string RemoveLibraryMessage = "Library.RemoveLibrary.Message";
      public void NotifyLibraryRemove(LibraryStoreVM library) =>
         Base.Helpers.Messaging.Send(RemoveLibraryMessage, library.ID);

   }
}
