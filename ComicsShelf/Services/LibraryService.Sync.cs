using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ComicsShelf.Services
{

   partial class LibraryService
   {
      SemaphoreSlim fileSyncSemaphore = new SemaphoreSlim(1, 1);

      private async Task<bool> LoadSyncData()
      {
         try
         {
            await fileSyncSemaphore.WaitAsync();

            var engine = Engines.Engine.Get(this.Library.Type);
            var byteArray = await engine.LoadSyncData(this.Library);
            if (byteArray == null) { return true; }

            var syncData = Helpers.FileStream.Deserialize<List<LibrarySyncVM>>(byteArray);
            if (syncData == null) { return true; }
            syncData = syncData.Where(x => x.Readed || x.ReadingPage > 0 || x.Rating > 0).ToList();

            var comicFiles = this.Store.LibraryFiles[ComicsShelf.Store.enLibraryFilesGroup.Libraries].Where(x => x.ComicFile.LibraryKey == this.Library.ID);
            if (comicFiles == null) { return true; }

            if (comicFiles.Count() > 0 && syncData.Count > 0)
            {
               foreach (var syncFile in syncData)
               {
                  var comicFile = comicFiles.Where(x => x.ComicFile.Key == syncFile.Key).FirstOrDefault();
                  if (comicFile != null) { comicFile.Set(syncFile.ToComicFile()); }
               }
            }

            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
         finally { fileSyncSemaphore.Release(); }
      }

      private async Task<bool> SaveSyncData()
      {
         try
         {
            await fileSyncSemaphore.WaitAsync();

            var comicFiles = this.Store.LibraryFiles[ComicsShelf.Store.enLibraryFilesGroup.Libraries].Where(x => x.ComicFile.LibraryKey == this.Library.ID);
            if (comicFiles == null) { return true; }

            var syncData = comicFiles
               .Where(x => x.Readed || x.ReadingPage > 0 || x.Rating > 0)
               .Select(comicFile => LibrarySyncVM.FromComicFile(comicFile.ComicFile))
               .ToList();

            var byteArray = Helpers.FileStream.Serialize(syncData);
            if (byteArray == null) { return true; }

            var engine = Engines.Engine.Get(this.Library.Type);
            if (!await engine.SaveSyncData(this.Library, byteArray)) { return false; }

            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
         finally { fileSyncSemaphore.Release(); }
      }

   }

   public class LibrarySyncVM
   {
      public string Key { get; set; }
      public DateTime ReleaseDate { get; set; }
      public bool Readed { get; set; }
      public DateTime ReadingDate { get; set; }
      public short ReadingPage { get; set; }
      public double ReadingPercent { get; set; }
      public short Rating { get; set; }

      #region ToComicFile
      public ComicFiles.ComicFile ToComicFile()
      {
         return new ComicFiles.ComicFile
         {
            Key = this.Key,
            ReleaseDate = this.ReleaseDate,
            Readed = this.Readed,
            ReadingDate = this.ReadingDate,
            ReadingPage = this.ReadingPage,
            ReadingPercent = this.ReadingPercent,
            Rating = this.Rating
         };
      }
      #endregion

      #region FromComicFile
      public static LibrarySyncVM FromComicFile(ComicFiles.ComicFile comicFile)
      {
         return new LibrarySyncVM
         {
            Key = comicFile.Key,
            ReleaseDate = comicFile.ReleaseDate,
            Readed = comicFile.Readed,
            ReadingDate = comicFile.ReadingDate,
            ReadingPage = comicFile.ReadingPage,
            ReadingPercent = comicFile.ReadingPercent,
            Rating = comicFile.Rating
         };
      }
      #endregion

   }

}
