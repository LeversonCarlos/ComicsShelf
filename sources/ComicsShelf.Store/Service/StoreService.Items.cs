using ComicsShelf.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace ComicsShelf.Store
{
   partial class StoreService
   {

      readonly Dictionary<string, SortedList<string, ItemVM>> ItemList = new Dictionary<string, SortedList<string, ItemVM>>();
      public ItemVM[] GetLibraryItems(LibraryVM library) => (this.ItemList.ContainsKey(library.ID) ? this.ItemList[library.ID]?.Values?.ToArray() : null) ?? new ItemVM[] { };

      SectionVM[] SectionList { get; set; }
      public SectionVM[] GetSections() => SectionList ?? new SectionVM[] { };
      public void SetSections(SectionVM[] sections) => SectionList = sections;

   }
}
