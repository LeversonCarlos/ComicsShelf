namespace ComicsShelf.ComicFiles
{

   public class ComicPageVM : Helpers.BaseVM
   {

      public ComicPageVM() { }

      public short Index { get; set; }
      public string Text { get; set; }
      public string Path { get; set; }
      public ComicPageSize PageSize { get; set; }

      bool _IsVisible;
      public bool IsVisible
      {
         get { return this._IsVisible; }
         set { this.SetProperty(ref this._IsVisible, value); }
      }

   }

}
