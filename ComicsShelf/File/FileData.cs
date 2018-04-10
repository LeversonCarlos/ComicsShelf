﻿namespace ComicsShelf.File
{
   public enum enumFileRate : short { None = -1, Zero = 0, One = 1, Two = 2, Three = 3, Four = 4, Five = 5 }

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

      #region Pages
      Helpers.Observables.ObservableList<FilePageData> _Pages;
      public Helpers.Observables.ObservableList<FilePageData> Pages
      {
         get { return this._Pages; }
         set { this.SetProperty(ref this._Pages, value); }
      }
      #endregion


      #region PersistentData
      bool PersistentDataLoading;
      Database.ComicFiles _PersistentData;
      public Database.ComicFiles PersistentData
      {
         get { return this._PersistentData; }
         set
         {
            this._PersistentData = value;
            this.PersistentDataLoading = true;
            this.Readed = value.Readed;
            this.ReadingDate = value.ReadingDate;
            this.ReadingPercent = value.ReadingPercent;
            this.ReadingPage = value.ReadingPage;
            this.Rate = enumFileRate.None;
            if (value.Rate.HasValue)
            { this.Rate = (enumFileRate)value.Rate; }
            this.PersistentDataLoading = false;
         }
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
            if (this.PersistentDataLoading) { return; }
            if (this.PersistentData.Readed == value) { return; }
            this.PersistentData.Readed = value;

            this.ReadingDate = (value ? App.Database.GetDate() : null);
            this.ReadingPercent = (double)(value ? 1 : 0);
            if (value) { this.ReadingPage = (short)0; }
            App.Database.Update(this.PersistentData);
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
            if (this.PersistentDataLoading) { return; }
            this.PersistentData.ReadingPercent = value;
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
            if (this.PersistentData.ReadingPage == value) { return; }
            this.PersistentData.ReadingPage = value;

            if (this.Pages != null && this.Pages.Count != 0) {
               this.ReadingPercent = ((double)value / (double)this.Pages.Count);
               this.ReadingDate = App.Database.GetDate();
               this.Readed = (value == (this.Pages.Count - 1));
            }
            App.Database.Update(this.PersistentData);
         }
      }
      #endregion

      #region Rate
      enumFileRate _Rate;
      public enumFileRate Rate
      {
         get { return this._Rate; }
         set
         {
            this.SetProperty(ref this._Rate, value);
            if (this.PersistentDataLoading) { return; }
            this.PersistentData.Rate = null;
            if (value != enumFileRate.None) { this.PersistentData.Rate = (short)value; }
            App.Database.Update(this.PersistentData);
         }
      }
      #endregion


      public string ReadedText { get { return R.Strings.FILE_COMIC_ALREADY_READED_LABEL; } }
      public string OpenTappedText { get { return R.Strings.FILE_OPEN_COMIC_LABEL; } }

   }

   public class FilePageData : Helpers.Observables.ObservableObject
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

   }

}