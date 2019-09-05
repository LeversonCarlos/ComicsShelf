using ComicsShelf.Helpers.Observables;

namespace ComicsShelf.Library
{
   public class LibraryVM: BaseVM
   {

      public readonly Store.LibraryModel Library;
      public LibraryVM(Store.LibraryModel Library)
      {
         this.Library = Library;
      }

   }
}
