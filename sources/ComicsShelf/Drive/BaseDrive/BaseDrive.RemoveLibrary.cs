using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class BaseDrive<T>
   {

      public async Task<bool> RemoveLibrary(LibraryVM library)
      {
         try {

            // CLEAR CACHE
            // TODO

            await Task.CompletedTask;
            return true;
         }
         catch (Exception ex) { Helpers.Message.Show(ex); return false; }
      }

   }
}
