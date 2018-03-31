using System.Runtime.CompilerServices;

namespace ComicsShelf.File
{
   public class FileData : Helpers.Observables.ObservableObject, iFileData
   {

      #region New
      public FileData()
      {
         // this.Pages = new Helpers.Observables.ObservableList<PageData>();
      }
      #endregion

      #region Text
      string _Text;
      public string Text
      {
         get { return this._Text; }
         set { this.SetProperty(ref this._Text, value); }
      }
      #endregion

      #region CoverPath
      string _CoverPath;
      public string CoverPath
      {
         get { return this._CoverPath; }
         set { this.SetProperty(ref this._CoverPath, value); }
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

      #region PersistentData
      bool PersistentDataLoading;
      Helpers.Settings.Comics _PersistentData;
      public Helpers.Settings.Comics PersistentData
      {
         get { return this._PersistentData; }
         set
         {
            this._PersistentData = value;
            this.PersistentDataLoading = true;
            this.Readed = value.Readed;
            this.PersistentDataLoading = false;
         }
      }
      #endregion     

      #region Readed

      public string ReadedText { get { return R.Strings.FILE_COMIC_ALREADY_READED_LABEL; } }

      bool _Readed;
      public bool Readed
      {
         get { return this._Readed; }
         set
         {
            this.SetProperty(ref this._Readed, value);
            if (this.PersistentDataLoading) { return; }
            this.PersistentData.Readed = value;
            this.ReadingDate = (value ? App.Settings.GetDatabaseDate() : null);
            this.ReadingPercent = (short)(value ? 100 : 0);
            this.ReadingPage = (short)0;
            App.Settings.Database.Update(this.PersistentData);
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
            if (this.PersistentDataLoading) { return; }
            this.PersistentData.ReadingDate = value;
            App.Settings.Database.Update(this.PersistentData);
         }
      }
      #endregion

      #region ReadingPercent
      short _ReadingPercent;
      public short ReadingPercent
      {
         get { return this._ReadingPercent; }
         set
         {
            this.SetProperty(ref this._ReadingPercent, value);
            if (this.PersistentDataLoading) { return; }
            this.PersistentData.ReadingPercent = value;
            App.Settings.Database.Update(this.PersistentData);
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
            if (this.PersistentDataLoading) { return; }
            this.PersistentData.ReadingPage = value;
            App.Settings.Database.Update(this.PersistentData);
         }
      }
      #endregion

   }
}