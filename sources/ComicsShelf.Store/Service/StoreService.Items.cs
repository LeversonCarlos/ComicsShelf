using ComicsShelf.ViewModels;
using System.Collections.Generic;

namespace ComicsShelf.Store
{
   partial class StoreService
   {
      readonly Dictionary<string, List<ItemVM>> ItemList = new Dictionary<string, List<ItemVM>>();

      public ItemVM[] GetLibraryItems(LibraryVM library) => this.ItemList[library.ID].ToArray();

   }
}
