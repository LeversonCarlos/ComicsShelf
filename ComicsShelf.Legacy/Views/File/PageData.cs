namespace ComicsShelf.Views.File
{
   public class PageData : Helpers.Observables.ObservableObject
   {

      #region Page
      short _Page;
      public short Page
      {
         get { return this._Page; }
         set { this.SetProperty(ref this._Page, value); }
      }
      #endregion

      #region Path
      string _Path;
      public string Path
      {
         get { return this._Path; }
         set { this.SetProperty(ref this._Path, value); }
      }
      #endregion

      #region Size
      Helpers.Controls.PageSize _Size;
      public Helpers.Controls.PageSize Size
      {
         get { return this._Size; }
         set { this.SetProperty(ref this._Size, value); }
      }
      #endregion

      #region IsVisible
      bool _IsVisible;
      public bool IsVisible
      {
         get { return this._IsVisible; }
         set { this.SetProperty(ref this._IsVisible, value); }
      }
      #endregion

   }
}