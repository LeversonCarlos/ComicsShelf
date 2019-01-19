namespace ComicsShelf.Views.Home
{
   public class EmptyData : Views.Home.LibraryData
   {

      public EmptyData() : base(new Helpers.Database.ComicFolder { Key = "EMPTY", Text = R.Strings.AppTitle })
      {
         this.IsEmptyPage = true;
      }

   }
}