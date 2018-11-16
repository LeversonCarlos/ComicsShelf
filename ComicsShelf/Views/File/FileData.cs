namespace ComicsShelf.Views.File
{
   public class FileData : Helpers.Observables.ObservableObject
   {

      #region New
      public Helpers.Database.ComicFile ComicFile { get; set; }
      internal FileData(Helpers.Database.ComicFile _ComicFile)
      {
         this.ComicFile = _ComicFile;
         this.FullText = this.ComicFile.FullText;
         this.SmallText = this.ComicFile.SmallText;
         this.FullPath = this.ComicFile.FullPath;
         this.Readed = this.ComicFile.Readed;
         this.ReadingPage = this.ComicFile.ReadingPage;
         this.ReadingPercent = this.ComicFile.ReadingPercent;
         this.ReadingOpacity = this.ComicFile.ReadingOpacity;
         this.Rating = this.ComicFile.Rating;
      }
      #endregion


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

      #region Pages
      Helpers.Observables.ObservableList<PageData> _Pages;
      public Helpers.Observables.ObservableList<PageData> Pages
      {
         get { return this._Pages; }
         set { this.SetProperty(ref this._Pages, value); }
      }
      #endregion


      #region Readed
      bool _Readed;
      public bool Readed
      {
         get { return this._Readed; }
         set
         {
            this.SetProperty(ref this._Readed, value);
            this.ComicFile.Readed = value;
         }
      }
      #endregion

      #region ReadingDate
      string _ReadingDate;
      public string ReadingDate
      {
         get { return this._ReadingDate; }
         set
         {
            this.SetProperty(ref this._ReadingDate, value);
            this.ComicFile.ReadingDate = value;
         }
      }
      #endregion

      #region ReadingPage
      short _ReadingPage;
      public short ReadingPage
      {
         get { return this._ReadingPage; }
         set
         {
            this.SetProperty(ref this._ReadingPage, value);
            this.ComicFile.ReadingPage = value;
         }
      }
      #endregion

      #region ReadingPercent
      double _ReadingPercent;
      public double ReadingPercent
      {
         get { return this._ReadingPercent; }
         set
         {
            this.SetProperty(ref this._ReadingPercent, value);
            this.ComicFile.ReadingPercent = value;
            this.ReadingOpacity = (value == 100 ? 0.5 : 1);
         }
      }
      #endregion

      #region ReadingOpacity
      double _ReadingOpacity;
      public double ReadingOpacity
      {
         get { return this._ReadingOpacity; }
         set {
            this.SetProperty(ref this._ReadingOpacity, value);
            this.ComicFile.ReadingOpacity = value;
         }
      }
      #endregion

      #region Rating
      int _Rating;
      public int Rating
      {
         get { return this._Rating; }
         set
         {
            this.SetProperty(ref this._Rating, value);
            this.ComicFile.Rating = value;
         }
      }
      #endregion     

   }
}