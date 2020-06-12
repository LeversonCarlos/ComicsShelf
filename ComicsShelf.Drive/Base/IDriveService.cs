using ComicsShelf.Store;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   public interface IDriveService
   {

      Task<bool> ValidateLibrary(LibraryStoreVM library);

      Task<LibraryStoreVM> AddLibrary();
      Task<bool> RemoveLibrary(LibraryStoreVM library);

      Task<ItemStoreVM[]> SearchFiles(LibraryStoreVM library);

      Task<bool> ExtractCover(LibraryStoreVM library, ItemStoreVM comicFile);
      // Task<List<ComicPageVM>> ExtractPages(LibraryStoreVM library, ItemStoreVM comicFile);

      string EscapeFileID(string fileID);
   }
}
