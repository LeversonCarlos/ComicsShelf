using System.Threading.Tasks;

namespace ComicsShelf.Library.Implementation
{
   partial class FileSystemService
   {

      public async Task<bool> Validate(Helpers.Database.Library library)
      {
         return await this.FileSystem.ValidateLibraryPath(library);
      }

   }
}