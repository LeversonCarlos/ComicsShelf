namespace ComicsShelf.Libraries
{
   public class LibraryVM : Helpers.BaseVM
   {

      internal readonly LibraryModel Library;
      public LibraryVM(LibraryModel value)
      {
         this.Library = value;
         this.Title = value.Description;
      }

   }
}
