using ComicsShelf.ViewModels;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   public interface IDrive
   {

      Task<bool> ValidateLibrary(LibraryVM library);

      Task<LibraryVM> AddLibrary();
      Task<bool> RemoveLibrary(LibraryVM library);

      Task<ItemVM[]> SearchItems(LibraryVM library);

      Task<bool> ExtractCover(LibraryVM library, ItemVM libraryItem);
      Task<PageVM[]> ExtractPages(LibraryVM library, ItemVM libraryItem);

      string EscapeFileID(string fileID);
   }
}
