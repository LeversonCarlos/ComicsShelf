using System;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   public partial class StoreService : IStoreService
   {

      public Task<bool> UpdateItemAsync(LibraryStoreVM library, ItemStoreVM item) => throw new NotImplementedException();

   }
}
