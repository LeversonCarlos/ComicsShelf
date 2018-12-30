using System.Collections.Generic;
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

      public async Task<List<Helpers.Database.ComicFile>> SearchFilesAsync(Helpers.Database.Library library)
      {
         return null;
      }

      public async Task ExtractCoverAsync(Helpers.Database.Library library, Helpers.Database.ComicFile comicFile)
      {
      }

      public async Task ExtractPagesAsync(Helpers.Database.Library library, Views.File.FileData fileData)
      {
      }

   }
}