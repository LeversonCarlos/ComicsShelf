using System.Collections.Generic;

namespace ComicsShelf.Store
{
   partial class StoreService
   {
      readonly Dictionary<string, List<ItemStoreVM>> ItemList = new Dictionary<string, List<ItemStoreVM>>();

      public ItemStoreVM[] GetLibraryItems(LibraryStoreVM library) => this.ItemList[library.ID].ToArray();

   }
}
