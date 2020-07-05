using ComicsShelf.ViewModels;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   public interface IStoreService
   {

      Task<bool> InitializeAsync();

      LibraryVM[] GetLibraries();
      LibraryVM GetLibrary(string libraryID);

      Task<bool> AddLibraryAsync(enLibraryType libraryType);
      Task<bool> UpdateLibraryAsync(LibraryVM library);
      Task<bool> RemoveLibraryAsync(LibraryVM library);

      ItemVM[] GetLibraryItems(LibraryVM library);
      Task<bool> UpdateItemAsync(ItemVM[] itemList);
      Task<bool> UpdateItemAsync(ItemVM item);

      SectionVM[] GetSections();
      void SetSections(SectionVM[] sections);

   }
}
