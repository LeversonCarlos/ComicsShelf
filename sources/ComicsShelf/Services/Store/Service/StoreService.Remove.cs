using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class StoreService
   {

      public async Task<bool> RemoveLibraryAsync(LibraryVM library)
      {
         try
         {

            if (!await Sync.RemoveLibrary(library)) { return false; }
            this.ItemList[library.ID].Clear();
            this.ItemList.Remove(library.ID);

            this.LibraryList = this.LibraryList
               .Where(x => x.ID != library.ID)
               .ToArray();
            if (!await Sync.SetLibraries(this.LibraryList)) { return false; }

            Helpers.Notify.LibraryRemove(library);

            return true;
         }
         catch (Exception ex) { Helpers.Message.Show(ex); return false; }
      }

   }
}
