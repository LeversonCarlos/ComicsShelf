using ComicsShelf.Helpers.Observables;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.ComicFiles
{
   public enum CacheStatusEnum : short { Unknown = -1, No = 0, Yes = 1 }

   public class ComicFileVM : Helpers.BaseVM
   {

      public ComicFile ComicFile { get; private set; }
      public ComicFileVM(ComicFile comicFile)
      {
         this.ComicFile = comicFile;
         this.ComicFile.Available = true;
         this.ComicFile.CoverPath = $"{Libraries.LibraryConstants.CoversCachePath}{comicFile.Key}.jpg";
         this.ComicFile.CachePath = $"{Libraries.LibraryConstants.FilesCachePath}{comicFile.Key}";
         this.Set(comicFile);
         this._CoverPath = Libraries.LibraryConstants.DefaultCover;
         this._CachePath = string.Empty;
      }

      internal void Set(ComicFile comicFile)
      {
         this.ComicFile.Readed = comicFile.Readed;
         this.ComicFile.Rating = comicFile.Rating;
         this.ComicFile.ReadingDate = comicFile.ReadingDate;
         this.ComicFile.ReadingPage = comicFile.ReadingPage;
         this.ComicFile.ReadingPercent = comicFile.ReadingPercent;

         this._Readed = comicFile.Readed;
         this._Rating = comicFile.Rating;
         this._ReadingDate = comicFile.ReadingDate;
         this._ReadingPage = comicFile.ReadingPage;
         this._ReadingPercent = comicFile.ReadingPercent;
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
            this.CacheStatus = (string.IsNullOrEmpty(value) ? CacheStatusEnum.No : CacheStatusEnum.Yes);
            this.HasCache = !string.IsNullOrEmpty(value);
         }
      }

      CacheStatusEnum _CacheStatus;
      public CacheStatusEnum CacheStatus
      {
         get { return this._CacheStatus; }
         set { this.SetProperty(ref this._CacheStatus, value); }
      }


      bool _HasCache;
      public bool HasCache
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
            this.UpdateLibrary();
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
            this.ComicFile.ReadingPage = 0;
            this.ReadingDate = (value ? DateTime.Now : DateTime.MinValue);
            this.ReadingPercent = (value ? 1 : 0);
            this.UpdateLibrary();
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


      public ObservableList<ComicPageVM> Pages { get; internal set; }


      internal async Task UpdateLibrary()
      {
         try
         {
            var service = DependencyService.Get<Libraries.LibraryService>();
            if (service == null) { return; }
            var library = service.Libraries[this.ComicFile.LibraryKey];
            if (library == null) { return; }
            await service.UpdateLibrary(library);
         }
         catch { }
      }

   }
}
