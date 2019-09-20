using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace ComicsShelf.Services
{
   public partial class LibraryService : IDisposable
   {

      readonly Store.LibraryModel Library;
      readonly Notify.NotifyVM Notify;
      readonly Store.ILibraryStore Store;

      public LibraryService(Store.LibraryModel Library)
      {
         try
         {
            this.Library = Library;
            this.Notify = new Notify.NotifyVM("General");
            this.Store = DependencyService.Get<Store.ILibraryStore>();

            if (!this.Store.LibraryFiles.ContainsKey(ComicsShelf.Store.enLibraryFilesGroup.Libraries))
            { this.Store.LibraryFiles.Add(ComicsShelf.Store.enLibraryFilesGroup.Libraries, new List<ComicFiles.ComicFileVM>()); }

            if (!this.Store.LibraryFiles.ContainsKey(ComicsShelf.Store.enLibraryFilesGroup.RecentFiles))
            { this.Store.LibraryFiles.Add(ComicsShelf.Store.enLibraryFilesGroup.RecentFiles, new List<ComicFiles.ComicFileVM>()); }

            if (!this.Store.LibraryFiles.ContainsKey(ComicsShelf.Store.enLibraryFilesGroup.ReadingFiles))
            { this.Store.LibraryFiles.Add(ComicsShelf.Store.enLibraryFilesGroup.ReadingFiles, new List<ComicFiles.ComicFileVM>()); }

            if (!System.IO.Directory.Exists(Helpers.Constants.CoversCachePath))
            { System.IO.Directory.CreateDirectory(Helpers.Constants.CoversCachePath); }

            if (!System.IO.Directory.Exists(Helpers.Constants.FilesCachePath))
            { System.IO.Directory.CreateDirectory(Helpers.Constants.FilesCachePath); }

         }
         catch { }
      }

      private List<ComicFiles.ComicFileVM> GetLibraryFiles()
      {
         return this.Store.GetLibraryFiles(this.Library);
      }

      private void AddLibraryFiles(List<ComicFiles.ComicFileVM> files)
      {
         this.Store.LibraryFiles[ComicsShelf.Store.enLibraryFilesGroup.Libraries]
            .AddRange(files);
      }

      private void ReplaceLibraryFile(ComicFiles.ComicFile file)
      {
         this.Store.LibraryFiles[ComicsShelf.Store.enLibraryFilesGroup.Libraries]
            .RemoveAll(x => x.ComicFile.Key == file.Key);
         this.Store.LibraryFiles[ComicsShelf.Store.enLibraryFilesGroup.Libraries]
            .Add(new ComicFiles.ComicFileVM(file));
      }

      public void Dispose()
      {
      }

   }
}
