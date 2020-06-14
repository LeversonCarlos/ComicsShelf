using ComicsShelf.Store;
using System.Collections.Generic;

namespace ComicsShelf.Engine
{
   partial class Engine
   {
      readonly Dictionary<string, List<ItemStoreVM>> ItemList = new Dictionary<string, List<ItemStoreVM>>();

      public ItemStoreVM[] GetLibraryItems(LibraryStoreVM library) => this.ItemList[library.ID].ToArray();

   }
}
