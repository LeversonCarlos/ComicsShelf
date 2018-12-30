using System.Threading.Tasks;

namespace ComicsShelf.Library.Implementation
{
   internal class FileSystemService : IService
   {

      Helpers.iFileSystem FileSystem { get; set; }
      public FileSystemService()
      {
         this.FileSystem = Helpers.FileSystem.Get();
      }

      public async Task<bool> Validate(Helpers.Database.Library library)
      {
         return await this.FileSystem.ValidateLibraryPath(library);
      }

      public async Task<bool> AddLibrary(Helpers.Database.Library library)
      {
         library.LibraryPath = await this.FileSystem.GetLibraryPath();
         return await this.Validate(library);
      }

      public void Dispose()
      {
         this.FileSystem.Dispose();
         this.FileSystem = null;
      }

   }
}