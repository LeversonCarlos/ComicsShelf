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
         try
         {
            library.LibraryID = await this.FileSystem.GetLibraryPath();
            return await this.Validate(library);
         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("FileSystemService.AddLibrary", ex); return false; }
      }

      public async Task<bool> RemoveLibrary(Library library)
      {
         return await Task.FromResult(true);
      }

   }
}