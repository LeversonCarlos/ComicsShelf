using System.Threading.Tasks;

namespace ComicsShelf.Library.Implementation
{
   internal class OneDrive : ILibraryService
   {

      public async Task<bool> Validate(Helpers.Database.Library library)
      {
         return true;
      }

      public async Task<bool> AddLibrary(Helpers.Database.Library library)
      {
         library.LibraryPath = "OneDrive";
         library.LibraryText = "OneDrive";
         return true;
      }

      public void Dispose()
      {
         // throw new NotImplementedException();
      }

   }
}