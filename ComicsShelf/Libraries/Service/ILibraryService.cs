using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComicsShelf.Libraries
{
   internal interface ILibraryService
   {
      Task<bool> Validate(Library library);
      Task<bool> AddLibrary(Library library);
      Task<bool> RemoveLibrary(Library library);
      Task<List<Helpers.Database.ComicFile>> SearchFilesAsync(Library library);
      Task<bool> ExtractCoverAsync(Library library, Helpers.Database.ComicFile comicFile);
      Task ExtractPagesAsync(Library library, Views.File.FileData fileData);
   }
}