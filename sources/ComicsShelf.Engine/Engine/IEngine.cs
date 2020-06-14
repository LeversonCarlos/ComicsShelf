using ComicsShelf.Store;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{
   public interface IEngine
   {

      Task<bool> InitializeAsync();

      LibraryStoreVM[] GetLibraries();
      LibraryStoreVM GetLibrary(string libraryID);

      Task<bool> AddLibraryAsync(enLibraryType libraryType);
      Task<bool> RemoveLibraryAsync(LibraryStoreVM library);

      ItemStoreVM[] GetLibraryItems(LibraryStoreVM library);

      Task<bool> UpdateItemAsync(ItemStoreVM item);
      Task<bool> UpdateItemAsync(ItemStoreVM[] itemList);

   }
}
