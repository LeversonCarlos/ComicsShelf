namespace ComicsShelf.Libraries
{
   public class LibraryVM : Helpers.BaseVM
   {

      private readonly LibraryModel Library;
      public LibraryVM(LibraryModel value)
      {
         this.Library = value;
         this.Title = value.Description;
      }

   }
}
