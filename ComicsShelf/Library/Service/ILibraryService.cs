using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComicsShelf.Library
{
   internal interface ILibraryService
   {
      Task<bool> Validate(vTwo.Libraries.Library library);
      Task<bool> AddLibrary(vTwo.Libraries.Library library);
      Task<bool> RemoveLibrary(vTwo.Libraries.Library library);
      Task<List<Helpers.Database.ComicFile>> SearchFilesAsync(vTwo.Libraries.Library library);
      Task<bool> ExtractCoverAsync(vTwo.Libraries.Library library, Helpers.Database.ComicFile comicFile);
      Task ExtractPagesAsync(vTwo.Libraries.Library library, Views.File.FileData fileData);
   }
}