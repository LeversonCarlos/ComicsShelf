using System.Threading.Tasks;

namespace ComicsShelf.Library.Implementation
{
   partial class FileSystemService
   {

      public async Task<bool> Validate(Library library)
      {
         return await this.FileSystem.ValidateLibraryPath(library);
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