namespace ComicsShelf.ComicFiles
{

   public class ComicPageVM : Helpers.BaseVM
   {

      public ComicPageVM()
      {
      }

      short _Index;
      public short Index
      {
         get { return this._Index; }
         set { this.SetProperty(ref this._Index, value); }
      }

      string _Text;
      public string Text
      {
         get { return this._Text; }
         set { this.SetProperty(ref this._Text, value); }
      }

      string _Path;
      public string Path
      {
         get { return this._Path; }
         set { this.SetProperty(ref this._Path, value); }
      }

      bool _IsVisible;
      public bool IsVisible
      {
         get { return this._IsVisible; }
         set { this.SetProperty(ref this._IsVisible, value); }
      }

   }
}
