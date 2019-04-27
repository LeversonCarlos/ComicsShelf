using ComicsShelf.Libraries;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.LocalDrive
{
   partial class LocalDriveEngine 
   {

      public async Task<bool> Validate(LibraryModel library)
      {
         if (!await Helpers.Permissions.HasStoragePermission()) { return false; }
         if (!await this.FileSystem.Validate(library.Key)) { return false; }
         return true;
      }

   }
}
