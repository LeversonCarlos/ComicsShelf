using System.Threading.Tasks;

namespace ComicsShelf.Library.Implementation
{
   partial class FileSystemService
   {

      public async Task<bool> Validate(vTwo.Libraries.Library library)
      {
         return await this.FileSystem.ValidateLibraryPath(library);
      }

      public async Task<bool> AddLibrary(vTwo.Libraries.Library library)
      {
         library.LibraryID = await this.FileSystem.GetLibraryPath();
         return await this.Validate(library);
      }

      public async Task<bool> RemoveLibrary(vTwo.Libraries.Library library)
      {
         return true;
      }

   }
}