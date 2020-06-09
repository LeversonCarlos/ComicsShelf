using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   public interface IStoreService
   {

      Task<bool> InitializeAsync();

      LibraryStoreVM[] GetLibraries();
      LibraryStoreVM GetLibrary(string libraryID);

      Task<bool> AddLibraryAsync(LibraryStoreVM library);
      Task<bool> RemoveLibraryAsync(LibraryStoreVM library);

      Task<ItemStoreVM[]> GetLibraryItems(LibraryStoreVM library);
      Task<bool> UpdateItemAsync(LibraryStoreVM library, ItemStoreVM item);
      
   }
}
