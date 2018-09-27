namespace ComicsShelf.Views.Thumbnail
{
   public abstract class ThumbnailData : Helpers.Observables.ObservableObject
   {

      #region FullText
      string _FullText;
      public string FullText
      {
         get { return this._FullText; }
         set { this.SetProperty(ref this._FullText, value); }
      }
      #endregion

      #region SmallText
      string _SmallText;
      public string SmallText
      {
         get { return this._SmallText; }
         set { this.SetProperty(ref this._SmallText, value); }
      }
      #endregion

      #region CoverPath
      string _CoverPath;
      public string CoverPath
      {
         get { return this._CoverPath; }
         set { this.SetProperty(ref this._CoverPath, value, AlwaysInvokePropertyChanged: true); }
      }
      #endregion

      #region FullPath
      string _FullPath;
      public string FullPath
      {
         get { return this._FullPath; }
         set { this.SetProperty(ref this._FullPath, value); }
      }
      #endregion

   }
}