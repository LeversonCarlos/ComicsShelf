using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ComicsShelf.Services
{
   public partial class LibraryService : IDisposable
   {
      private const string generalRecentFilesKey = "General.RecentFiles";
      private const string generalReadingFilesKey = "General.ReadingFiles";

      readonly Store.LibraryModel Library;
      readonly Notify.NotifyVM Notify;
      readonly Store.ILibraryStore Store;
      List<ComicFiles.ComicFileVM> LibraryFiles => this.Store.ComicFiles[this.Library.ID];

      public LibraryService(Store.LibraryModel Library)
      {
         try
         {
            this.Library = Library;
            this.Notify = new Notify.NotifyVM("General");
            this.Store = DependencyService.Get<Store.ILibraryStore>();

            if (!this.Store.ComicFiles.ContainsKey(generalRecentFilesKey))
            { this.Store.ComicFiles.Add(generalRecentFilesKey, new List<ComicFiles.ComicFileVM>()); }

            if (!this.Store.ComicFiles.ContainsKey(generalReadingFilesKey))
            { this.Store.ComicFiles.Add(generalReadingFilesKey, new List<ComicFiles.ComicFileVM>()); }

            if (!System.IO.Directory.Exists(Helpers.Constants.CoversCachePath))
            { System.IO.Directory.CreateDirectory(Helpers.Constants.CoversCachePath); }

            if (!System.IO.Directory.Exists(Helpers.Constants.FilesCachePath))
            { System.IO.Directory.CreateDirectory(Helpers.Constants.FilesCachePath); }

         }
         catch { }
      }

      public void Dispose()
      {
      }

   }
}
