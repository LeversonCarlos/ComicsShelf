using ComicsShelf.ViewModels;
using System.Linq;

namespace ComicsShelf.Store
{
   partial class StoreService
   {
      LibraryVM[] LibraryList { get; set; } = new LibraryVM[] { };

      public LibraryVM[] GetLibraries() => this.LibraryList;

      public LibraryVM GetLibrary(string libraryID) =>
         this.LibraryList
            .Where(x => x.ID == libraryID)
            .FirstOrDefault();

   }
}
