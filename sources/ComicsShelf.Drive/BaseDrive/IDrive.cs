using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   public interface IDrive
   {

      Task<bool> ValidateLibrary(ItemVM library);

      Task<ItemVM> AddLibrary();
      Task<bool> RemoveLibrary(ItemVM library);

      Task<ItemVM[]> SearchItems(ItemVM library);

      Task<bool> ExtractCover(ItemVM library, ItemVM libraryItem);
      // Task<List<ComicPageVM>> ExtractPages(DirectoryVM library, FileVM libraryItem);

      string EscapeFileID(string fileID);
   }
}
