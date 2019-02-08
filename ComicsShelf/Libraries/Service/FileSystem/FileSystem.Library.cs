using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Libraries.Implementation
{
   partial class FileSystemService
   {

      public async Task<bool> Validate(Library library)
      {
         try
         {
            if (!await Helpers.Permissions.HasStoragePermission()) { return false; }
            if (!await this.FileSystem.ValidateLibraryPath(library)) { return false; }
            return true;
         }
         catch (Exception) { throw; }
      }

      public async Task<bool> AddLibrary(Library library)
      {
         library.LibraryID = await this.FileSystem.GetLibraryPath();
         return await this.Validate(library);
      }

      public async Task<bool> RemoveLibrary(Library library)
      {
         return true;
      }

   }
}