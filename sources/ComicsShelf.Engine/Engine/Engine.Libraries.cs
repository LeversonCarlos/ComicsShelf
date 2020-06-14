using ComicsShelf.Store;
using System.Linq;

namespace ComicsShelf.Engine
{
   partial class Engine
   {
      LibraryStoreVM[] LibraryList { get; set; } = new LibraryStoreVM[] { };

      public LibraryStoreVM[] GetLibraries() => this.LibraryList;

      public LibraryStoreVM GetLibrary(string libraryID) =>
         this.LibraryList
         .Where(x => x.ID == libraryID)
         .FirstOrDefault();

   }
}
