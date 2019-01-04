namespace ComicsShelf.Library
{
   public class LibraryDataItem : Helpers.Observables.ObservableObject
   {

      #region New
      internal Helpers.Database.Library Library { get; set; }
      internal LibraryDataItem(Helpers.Database.Library _Library)
      {
         this.Library = _Library;
         this._LibraryPath = this.Library.LibraryPath;
         this._LibraryText = this.Library.LibraryText;
         this._LibraryType = this.Library.Type;
         this._FileCount = this.Library.FileCount;
      }
      #endregion

      #region LibraryPath
      string _LibraryPath;
      public string LibraryPath
      {
         get { return this._LibraryPath; }
         set
         {
            this.SetProperty(ref this._LibraryPath, value);

            this.Library.LibraryPath = value;
            App.Database.Update(this.Library);
         }
      }
      #endregion

      #region LibraryText
      string _LibraryText;
      public string LibraryText
      {
         get { return this._LibraryText; }
         set
         {
            this.SetProperty(ref this._LibraryText, value);

            this.Library.LibraryText = value;
            App.Database.Update(this.Library);
         }
      }
      #endregion

      #region LibraryType
      ComicsShelf.Library.LibraryTypeEnum _LibraryType;
      public ComicsShelf.Library.LibraryTypeEnum LibraryType
      {
         get { return this._LibraryType; }
         set
         {
            this.SetProperty(ref this._LibraryType, value);

            this.Library.Type = value; 
            App.Database.Update(this.Library);
         }
      }
      #endregion

      #region FileCount
      int _FileCount;
      public int FileCount
      {
         get { return this._FileCount; }
         set
         {
            this.SetProperty(ref this._FileCount, value);

            this.Library.FileCount = value;
            App.Database.Update(this.Library);
         }
      }
      #endregion

   }
}