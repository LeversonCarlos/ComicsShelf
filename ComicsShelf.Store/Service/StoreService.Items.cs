using System.Collections.Generic;

namespace ComicsShelf.Store
{
   partial class StoreService
   {
      readonly Dictionary<string, List<ItemStoreVM>> LibraryItems = new Dictionary<string, List<ItemStoreVM>>();

      public ItemStoreVM[] GetLibraryItems(LibraryStoreVM library) => this.LibraryItems[library.ID].ToArray();

   }
}
