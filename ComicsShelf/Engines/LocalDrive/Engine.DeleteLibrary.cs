using ComicsShelf.Store;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.LocalDrive
{
   partial class LocalDriveEngine
   {

      public async Task<bool> DeleteLibrary(LibraryModel library)
      {
         return await Task.FromResult(true);
      }

   }
}
