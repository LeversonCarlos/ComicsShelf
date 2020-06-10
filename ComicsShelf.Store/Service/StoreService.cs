using System;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   public partial class StoreService : IStoreService
   {
      string SyncKey() => "ComicsShelf";
      string LibrarySyncKey(string libraryID) => $"{SyncKey()}.Library.{libraryID}";

      public Task<bool> UpdateItemAsync(LibraryStoreVM library, ItemStoreVM item) => throw new NotImplementedException();

   }
}
