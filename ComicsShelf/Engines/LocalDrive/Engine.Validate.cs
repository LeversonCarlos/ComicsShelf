using ComicsShelf.Libraries;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.LocalDrive
{
   partial class LocalDriveEngine 
   {

      public async Task<bool> Validate(LibraryModel library)
      {
         if (!await this.HasStoragePermission()) { return false; }
         if (!await this.FileSystem.Validate(library.LibraryKey)) { return false; }
         return true;
      }

      public async Task<bool> HasStoragePermission()
      { return await Helpers.Permissions.HasStoragePermission(); }

   }
}
