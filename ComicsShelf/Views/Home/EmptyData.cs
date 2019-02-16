namespace ComicsShelf.Views.Home
{
   public class EmptyData : Views.Home.LibraryData
   {

      public EmptyData() : base(new Helpers.Database.ComicFolder { Key = "EMPTY", Text = R.Strings.AppTitle })
      {
         this.IsEmptyPage = true;
         this.NotifyData.Text = R.Strings.SEARCH_ENGINE_LOADING_DATABASE_DATA_MESSAGE;
         this.NotifyData.Progress = 0;
         this.NotifyData.IsRunning = true;
      }

   }
}