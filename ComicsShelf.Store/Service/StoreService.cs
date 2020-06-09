using System;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   public class StoreService : IStoreService
   {

      public Task<bool> InitializeAsync() => throw new NotImplementedException();

      public LibraryStoreVM[] GetLibraries() => throw new NotImplementedException();
      public LibraryStoreVM GetLibrary(string libraryID) => throw new NotImplementedException();

      public Task<bool> AddLibraryAsync(LibraryStoreVM library) => throw new NotImplementedException();
      public Task<bool> RemoveLibraryAsync(LibraryStoreVM library) => throw new NotImplementedException();

      public Task<ItemStoreVM[]> GetLibraryItems(LibraryStoreVM library) => throw new NotImplementedException();
      public Task<bool> UpdateItemAsync(LibraryStoreVM library, ItemStoreVM item) => throw new NotImplementedException();

   }
}
