using System;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   public partial class StoreService : IStoreService
   {

      public LibraryStoreVM[] GetLibraries() => throw new NotImplementedException();
      public LibraryStoreVM GetLibrary(string libraryID) => throw new NotImplementedException();

      public Task<ItemStoreVM[]> GetLibraryItems(LibraryStoreVM library) => throw new NotImplementedException();
      public Task<bool> UpdateItemAsync(LibraryStoreVM library, ItemStoreVM item) => throw new NotImplementedException();

   }
}
