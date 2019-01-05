using System.Threading.Tasks;

namespace ComicsShelf.Library.Implementation
{
   partial class FileSystemService
   {

      public async Task<bool> AddLibrary(Helpers.Database.Library library)
      {
         library.LibraryPath = await this.FileSystem.GetLibraryPath();
         return await this.Validate(library);
      }

      public async Task<bool> RemoveLibrary(Helpers.Database.Library library)
      {
         return true;
      }

   }
}