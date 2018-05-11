namespace ComicsShelf.Library
{
   public class LibraryData : Engine.StepData
   {

      #region LibraryPath
      string _LibraryPath;
      public string LibraryPath
      {
         get { return this._LibraryPath; }
         set
         {
            this.SetProperty(ref this._LibraryPath, value);
            this.IsLibraryEmpty = string.IsNullOrEmpty(value);
            this.IsLibraryDefined = !string.IsNullOrEmpty(value);
         }
      }
      #endregion

      #region IsLibraryEmpty
      bool _IsLibraryEmpty;
      public bool IsLibraryEmpty
      {
         get { return this._IsLibraryEmpty; }
         set { this.SetProperty(ref this._IsLibraryEmpty, value); }
      }
      #endregion

      #region IsLibraryDefined
      bool _IsLibraryDefined;
      public bool IsLibraryDefined
      {
         get { return this._IsLibraryDefined; }
         set { this.SetProperty(ref this._IsLibraryDefined, value); }
      }
      #endregion

   }
}