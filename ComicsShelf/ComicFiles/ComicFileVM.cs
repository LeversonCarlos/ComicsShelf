using System;

namespace ComicsShelf.ComicFiles
{
   public enum HasCacheEnum : short { Unknown = -1, No = 0, Yes = 1 }

   public class ComicFileVM : Helpers.Observables.ObservableObject
   {

      public ComicFile ComicFile { get; private set; }
      public ComicFileVM(ComicFile comicFile)
      {
         this.Set(comicFile);
      }

      internal void Set(ComicFile comicFile)
      {
         this.ComicFile = comicFile;
         this._Readed = comicFile.Readed;
         this._Rating = comicFile.Rating;
         this._ReadingDate = comicFile.ReadingDate;
         this._ReadingPage = comicFile.ReadingPage;
         this._ReadingPercent = comicFile.ReadingPercent;
         this._CoverPath = Libraries.LibraryConstants.DefaultCover;
         this._CachePath = string.Empty;
      }

      string _CoverPath;
      public string CoverPath
      {
         get { return this._CoverPath; }
         set
         {
            this.SetProperty(ref this._CoverPath, value);
            this.ComicFile.CoverPath = value;
         }
      }

      string _CachePath;
      public string CachePath
      {
         get { return this._CachePath; }
         set
         {
            this.SetProperty(ref this._CachePath, value);
            this.HasCache = (string.IsNullOrEmpty(value) ? HasCacheEnum.No : HasCacheEnum.Yes);
         }
      }

      HasCacheEnum _HasCache;
      public HasCacheEnum HasCache
      {
         get { return this._HasCache; }
         set { this.SetProperty(ref this._HasCache, value); }
      }

      short _Rating;
      public short Rating
      {
         get { return this._Rating; }
         set
         {
            this.SetProperty(ref this._Rating, value);
            this.ComicFile.Rating = value;
         }
      }

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

      DateTime _ReadingDate;
      public DateTime ReadingDate
      {
         get { return this._ReadingDate; }
         set
         {
            this.SetProperty(ref this._ReadingDate, value);
            this.ComicFile.ReadingDate = value;
         }
      }

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

      double _ReadingPercent;
      public double ReadingPercent
      {
         get { return this._ReadingPercent; }
         set
         {
            this.SetProperty(ref this._ReadingPercent, value);
            this.ComicFile.ReadingPercent = value;
         }
      }

   }
}
